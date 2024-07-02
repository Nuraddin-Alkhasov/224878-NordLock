using System;
using System.Linq;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using VisiWin.ApplicationFramework;
using VisiWin.Commands;
using System.ComponentModel.Composition;
using System.Threading;
using VisiWin.Language;

namespace HMI
{
    internal enum InternalDialogButtons
    {
        OK = 0,
        OKCancel = 1,
        YesNoCancel = 3,
        YesNo = 4,
        Custom = 7,
        Close = 8
    }

    public enum InternalDialogResult
    {
        None = 0,
        OK = 1,
        Cancel = 2,
        Left = 3,
        Middle = 4,
        Right = 5,
        Yes = 6,
        No = 7
    }

    public enum MessageBoxIcon
    {
        None,
        Error,
        Hand,
        Stop,
        Question,
        Exclamation,
        Warning,
        Information,
        Asterisk
    }

    internal class DialogParams
    {
        internal string headerText { get; set; }
        internal string content { get; set; }
        internal InternalDialogButtons type { get; set; }
        internal InternalDialogResult defaultResult { get; set; }
        internal MessageBoxIcon icon { get; set; }
        internal bool modal { get; set; }
        internal string leftButtonText { get; set; }
        internal string middleButtonText { get; set; }
        internal string rightButtonText { get; set; }
    }

    /// <summary>
    ///	Dialog-Adapter
    /// </summary>
    [ExportAdapter("DialogAdapter")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DialogAdapter : AdapterBase
    {
        #region Felder

        /// <summary>
        /// Welche Knöpfe hat der Dialog
        /// </summary>
        private InternalDialogButtons Type = InternalDialogButtons.OK;
        private ILanguageService textService;
        private InputManager inputManager;
        private bool keyboardIsHandled;


        /// <summary>
        /// Zugriff auf den ObjectStore
        /// </summary>
        private string osKey = "";

        #endregion

        #region Initialisierung

        public DialogAdapter()
        {
            this.textService = ApplicationService.GetService<ILanguageService>();
            this.inputManager = InputManager.Current;

            //	Kommandos anbinden
            this.CloseDialogCommand = new ActionCommand(OnDialogClosingCommand);
        }

        protected override void OnViewLoaded(object sender, ViewLoadedEventArg e)
        {
            //	die Parameter für den anzuzeigenden Dialog stecken im ObjectStore unter dem Namen der View
            var exportViewAttribute = e.View.GetType().GetCustomAttributes(typeof(ExportViewAttribute), false).FirstOrDefault() as ExportViewAttribute;
            string viewName = (exportViewAttribute == null) ? "DialogView" : exportViewAttribute.ViewName;

            DialogParams dbParams = ApplicationService.ObjectStore.GetValue(viewName + "_KEY") as DialogParams;
            if (dbParams != null)
            {
                //	Viewname kopieren für das Ergebnis
                this.osKey = viewName;

                //	Kopfzeile
                this.HeaderLocalizableText = (string.IsNullOrEmpty(dbParams.headerText) || !dbParams.headerText.StartsWith("@")) ? "" : dbParams.headerText;
                this.HeaderText = (string.IsNullOrEmpty(dbParams.headerText) || dbParams.headerText.StartsWith("@")) ? "" : dbParams.headerText;

                //	Inhalt
                if (string.IsNullOrEmpty(dbParams.content))
                {
                    this.LocalizableContent = string.Empty;
                    this.Content = string.Empty;
                }
                else
                {
                    if (dbParams.content.StartsWith("@"))
                    {
                        this.LocalizableContent = dbParams.content;
                        this.Content = this.textService.GetText(dbParams.content);
                    }
                    else
                    {
                        this.LocalizableContent = string.Empty;
                        this.Content = dbParams.content;
                    }
                }

                //	welches Icon soll angezeigt werden?
                DataTemplate mbIcon = null;
                switch (dbParams.icon)
                {
                    case MessageBoxIcon.Asterisk: mbIcon = Application.Current.TryFindResource("MBAsterixIcon") as DataTemplate; break;
                    case MessageBoxIcon.Error: mbIcon = Application.Current.TryFindResource("MBErrorIcon") as DataTemplate; break;
                    case MessageBoxIcon.Exclamation: mbIcon = Application.Current.TryFindResource("MBExclamationIcon") as DataTemplate; break;
                    case MessageBoxIcon.Hand: mbIcon = Application.Current.TryFindResource("MBHandIcon") as DataTemplate; break;
                    case MessageBoxIcon.Information: mbIcon = Application.Current.TryFindResource("MBInformationIcon") as DataTemplate; break;
                    case MessageBoxIcon.Question: mbIcon = Application.Current.TryFindResource("MBQuestionIcon") as DataTemplate; break;
                    case MessageBoxIcon.Stop: mbIcon = Application.Current.TryFindResource("MBStopIcon") as DataTemplate; break;
                    case MessageBoxIcon.Warning: mbIcon = Application.Current.TryFindResource("MBWarningIcon") as DataTemplate; break;
                }
                if (mbIcon == null)
                {
                    this.IconVisible = Visibility.Collapsed;
                }
                else
                {
                    this.Icon = mbIcon;
                    this.IconVisible = Visibility.Visible;
                }

                //	welcher Knöpfe sollen angezeigt werden?
                this.Type = dbParams.type;
                switch (this.Type)
                {
                    case InternalDialogButtons.OK:
                        this.LeftButtonVisible = Visibility.Collapsed;
                        this.MiddleButtonVisible = Visibility.Visible;
                        this.RightButtonVisible = Visibility.Collapsed;
                        this.MiddleButtonLocalizableText = "@[ClientSystem].Dialogs.Common.cmdOK";
                        this.MiddleButtonText = "";
                        this.MiddleButtonIsDefault = (dbParams.defaultResult == InternalDialogResult.OK) ? true : false;
                        this.CancelButtonVisible = Visibility.Collapsed;
                        break;

                    case InternalDialogButtons.OKCancel:
                        this.LeftButtonVisible = Visibility.Visible;
                        this.LeftButtonLocalizableText = "@[ClientSystem].Dialogs.Common.cmdOK";
                        this.LeftButtonText = "";
                        this.LeftButtonIsDefault = (dbParams.defaultResult == InternalDialogResult.OK) ? true : false;
                        this.MiddleButtonVisible = Visibility.Collapsed;
                        this.RightButtonVisible = Visibility.Visible;
                        this.RightButtonLocalizableText = "@[ClientSystem].Dialogs.Common.cmdCancel";
                        this.RightButtonText = "";
                        this.RightButtonIsDefault = (dbParams.defaultResult == InternalDialogResult.Cancel) ? true : false;
                        this.CancelButtonVisible = Visibility.Collapsed;
                        break;

                    case InternalDialogButtons.YesNo:
                        this.LeftButtonVisible = Visibility.Visible;
                        this.LeftButtonLocalizableText = "@[ClientSystem].Dialogs.Common.cmdYes";
                        this.LeftButtonText = "";
                        this.LeftButtonIsDefault = (dbParams.defaultResult == InternalDialogResult.Yes) ? true : false;
                        this.MiddleButtonVisible = Visibility.Collapsed;
                        this.RightButtonVisible = Visibility.Visible;
                        this.RightButtonLocalizableText = "@[ClientSystem].Dialogs.Common.cmdNo";
                        this.RightButtonText = "";
                        this.RightButtonIsDefault = (dbParams.defaultResult == InternalDialogResult.No) ? true : false;
                        this.CancelButtonVisible = Visibility.Collapsed;
                        break;

                    case InternalDialogButtons.YesNoCancel:
                        this.LeftButtonVisible = Visibility.Visible;
                        this.LeftButtonLocalizableText = "@[ClientSystem].Dialogs.Common.cmdYes";
                        this.LeftButtonText = "";
                        this.LeftButtonIsDefault = (dbParams.defaultResult == InternalDialogResult.Yes) ? true : false;
                        this.MiddleButtonVisible = Visibility.Visible;
                        this.MiddleButtonLocalizableText = "@[ClientSystem].Dialogs.Common.cmdNo";
                        this.MiddleButtonText = "";
                        this.MiddleButtonIsDefault = (dbParams.defaultResult == InternalDialogResult.No) ? true : false;
                        this.RightButtonVisible = Visibility.Visible;
                        this.RightButtonLocalizableText = "@[ClientSystem].Dialogs.Common.cmdCancel";
                        this.RightButtonText = "";
                        this.RightButtonIsDefault = (dbParams.defaultResult == InternalDialogResult.Cancel) ? true : false;
                        this.CancelButtonVisible = Visibility.Collapsed;
                        break;

                    case InternalDialogButtons.Close:
                        this.LeftButtonVisible = Visibility.Collapsed;
                        this.MiddleButtonVisible = Visibility.Collapsed;
                        this.RightButtonVisible = Visibility.Collapsed;
                        this.CancelButtonVisible = Visibility.Visible;
                        break;

                 case InternalDialogButtons.Custom:

                        if (string.IsNullOrEmpty(dbParams.leftButtonText))
                        {
                            this.LeftButtonVisible = Visibility.Collapsed;
                        }
                        else
                        {
                            this.LeftButtonVisible = Visibility.Visible;
                            this.LeftButtonLocalizableText = (dbParams.leftButtonText.StartsWith("@")) ? dbParams.leftButtonText : "";
                            this.LeftButtonText = (dbParams.leftButtonText.StartsWith("@")) ? "" : dbParams.leftButtonText;
                            this.LeftButtonIsDefault = (dbParams.defaultResult == InternalDialogResult.Left) ? true : false;
                        }
                        if (string.IsNullOrEmpty(dbParams.middleButtonText))
                        {
                            this.MiddleButtonVisible = Visibility.Collapsed;
                        }
                        else
                        {
                            this.MiddleButtonVisible = Visibility.Visible;
                            this.MiddleButtonLocalizableText = (dbParams.middleButtonText.StartsWith("@")) ? dbParams.middleButtonText : "";
                            this.MiddleButtonText = (dbParams.middleButtonText.StartsWith("@")) ? "" : dbParams.middleButtonText;
                            this.MiddleButtonIsDefault = (dbParams.defaultResult == InternalDialogResult.Middle) ? true : false;
                        }
                        if (string.IsNullOrEmpty(dbParams.rightButtonText))
                        {
                            this.RightButtonVisible = Visibility.Collapsed;
                        }
                        else
                        {
                            this.RightButtonVisible = Visibility.Visible;
                            this.RightButtonLocalizableText = (dbParams.rightButtonText.StartsWith("@")) ? dbParams.rightButtonText : "";
                            this.RightButtonText = (dbParams.rightButtonText.StartsWith("@")) ? "" : dbParams.rightButtonText;
                            this.RightButtonIsDefault = (dbParams.defaultResult == InternalDialogResult.Right) ? true : false;
                        }
                        this.CancelButtonIsEnabled = true;
                        break;
                }

                //	modal oder nicht?
                this.Modal = dbParams.modal;

                //	ObjectStore aufräumen
                ApplicationService.ObjectStore.Remove(this.osKey + "_KEY");

                //	Keyboard-Überwachung installieren?
                if ((this.inputManager != null) && (this.CancelButtonIsEnabled || this.LeftButtonIsDefault || this.MiddleButtonIsDefault || this.RightButtonIsDefault))
                {
                    this.inputManager.PostProcessInput += inputManager_PostProcessInput;
                    this.keyboardIsHandled = true;
                }
            }

            base.OnViewLoaded(sender, e);
        }

        /// <summary>
        /// Hotkeys Enter und ESC verwalten
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void inputManager_PostProcessInput(object sender, ProcessInputEventArgs e)
        {
            var input = e.StagingItem.Input;

            //	eine höherliegende Instanz kann das Keyboard schon verarbeitet haben,
            //	z.B. MessageBox über Dialog
            if (!input.Handled && (input is KeyEventArgs))
            {
                KeyEventArgs kea = input as KeyEventArgs;
                if (kea.RoutedEvent == Keyboard.KeyDownEvent)
                {
                    switch (kea.Key)
                    {
                        case Key.Return:
                            if (this.LeftButtonIsDefault)
                            {
                                OnDialogClosingCommand("Left");
                                input.Handled = true;
                            }
                            else if (this.MiddleButtonIsDefault)
                            {
                                OnDialogClosingCommand("Middle");
                                input.Handled = true;
                            }
                            else if (this.RightButtonIsDefault)
                            {
                                OnDialogClosingCommand("Right");
                                input.Handled = true;
                            }
                            break;

                        case Key.Escape:
                            if (this.CancelButtonIsEnabled)
                            {
                                OnDialogClosingCommand("Cancel");
                                input.Handled = true;
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// die zugehörige View wird entladen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnViewUnloaded(object sender, ViewUnloadedEventArg e)
        {
            //	eventuell Keyboardhandler abmelden
            if (this.keyboardIsHandled)
            {
                if (this.inputManager != null)
                    this.inputManager.PostProcessInput -= inputManager_PostProcessInput;
                this.keyboardIsHandled = false;
            }
            this.Content = null;

            base.OnViewUnloaded(sender, e);
        }

        #endregion Initialisierung

        #region Befehle

        /// <summary>
        /// Dialog schließen
        /// </summary>
        public ICommand CloseDialogCommand { get; set; }
        private void OnDialogClosingCommand(object parameter)
        {
            //	je nach Dialog-Typ haben die Knöpfe unterschiedliche Bedeutung
            switch (Type)
            {
                case InternalDialogButtons.OK:
                    ApplicationService.ObjectStore.SetValue(this.osKey + "_RESULT", InternalDialogResult.OK);
                    break;

                case InternalDialogButtons.OKCancel:
                    ApplicationService.ObjectStore.SetValue(this.osKey + "_RESULT", (parameter.ToString() == "Left") ? InternalDialogResult.OK : InternalDialogResult.Cancel);
                    break;

                case InternalDialogButtons.YesNo:
                    switch (parameter.ToString())
                    {
                        case "Left": ApplicationService.ObjectStore.SetValue(this.osKey + "_RESULT", InternalDialogResult.Yes); break;
                        case "Right": ApplicationService.ObjectStore.SetValue(this.osKey + "_RESULT", InternalDialogResult.No); break;
                        default: ApplicationService.ObjectStore.SetValue(this.osKey + "_RESULT", InternalDialogResult.Cancel); break;
                    }
                    break;

                case InternalDialogButtons.YesNoCancel:
                    switch (parameter.ToString())
                    {
                        case "Left": ApplicationService.ObjectStore.SetValue(this.osKey + "_RESULT", InternalDialogResult.Yes); break;
                        case "Middle": ApplicationService.ObjectStore.SetValue(this.osKey + "_RESULT", InternalDialogResult.No); break;
                        default: ApplicationService.ObjectStore.SetValue(this.osKey + "_RESULT", InternalDialogResult.Cancel); break;
                    }
                    break;

                case InternalDialogButtons.Custom:
                    switch (parameter.ToString())
                    {
                        case "Left": ApplicationService.ObjectStore.SetValue(this.osKey + "_RESULT", InternalDialogResult.Left); break;
                        case "Middle": ApplicationService.ObjectStore.SetValue(this.osKey + "_RESULT", InternalDialogResult.Middle); break;
                        case "Right": ApplicationService.ObjectStore.SetValue(this.osKey + "_RESULT", InternalDialogResult.Right); break;
                        default: ApplicationService.ObjectStore.SetValue(this.osKey + "_RESULT", InternalDialogResult.Cancel); break;
                    }
                    break;

                default:
                    ApplicationService.ObjectStore.SetValue(this.osKey + "_RESULT", MessageBoxResult.Cancel);
                    break;
            }
        }

        #endregion Befehle

        #region Bindungen

        /// <summary>
        /// Kopfzeile als fester Text
        /// </summary>
        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register("HeaderText", typeof(string), typeof(DialogAdapter), new UIPropertyMetadata(""));

        /// <summary>
        /// Kopfzeile als Indextext
        /// </summary>
        public string HeaderLocalizableText
        {
            get { return (string)GetValue(HeaderLocalizableTextProperty); }
            set { SetValue(HeaderLocalizableTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderLocalizableText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderLocalizableTextProperty =
            DependencyProperty.Register("HeaderLocalizableText", typeof(string), typeof(DialogAdapter), new PropertyMetadata(""));

        /// <summary>
        /// der Inhalt als Indextext
        /// </summary>
        public string LocalizableContent
        {
            get { return (string)GetValue(LocalizableContentProperty); }
            set { SetValue(LocalizableContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LocalizableContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LocalizableContentProperty =
            DependencyProperty.Register("LocalizableContent", typeof(string), typeof(DialogAdapter), new PropertyMetadata(""));

        /// <summary>
        /// der Inhalt
        /// </summary>
        public string Content
        {
            get { return (string)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(string), typeof(DialogAdapter), new PropertyMetadata(""));

        /// <summary>
        /// angezeigtes Icon
        /// </summary>
        public DataTemplate Icon
        {
            get { return (DataTemplate)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(DataTemplate), typeof(DialogAdapter), new PropertyMetadata(null));


        /// <summary>
        /// Icon sichtbar?
        /// </summary>
        public Visibility IconVisible
        {
            get { return (Visibility)GetValue(IconVisibleProperty); }
            set { SetValue(IconVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconVisibleProperty =
            DependencyProperty.Register("IconVisible", typeof(Visibility), typeof(DialogAdapter), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// CancelButton ist sichtbar?
        /// </summary>
        public Visibility CancelButtonVisible
        {
            get { return (Visibility)GetValue(CancelButtonVisibleProperty); }
            set { SetValue(CancelButtonVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CancelButtonVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CancelButtonVisibleProperty =
            DependencyProperty.Register("CancelButtonVisible", typeof(Visibility), typeof(DialogAdapter), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// CancelButton ist aktiviert?
        /// </summary>
        public bool CancelButtonIsEnabled
        {
            get { return (bool)GetValue(CancelButtonIsEnabledProperty); }
            set { SetValue(CancelButtonIsEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CancelButtonIsEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CancelButtonIsEnabledProperty =
            DependencyProperty.Register("CancelButtonIsEnabled", typeof(bool), typeof(DialogAdapter), new PropertyMetadata(true));

        /// <summary>
        /// modaler Dialog?
        /// </summary>
        public bool Modal
        {
            get { return (bool)GetValue(ModalProperty); }
            set { SetValue(ModalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Modal.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModalProperty =
            DependencyProperty.Register("Modal", typeof(bool), typeof(DialogAdapter), new UIPropertyMetadata(true));


        /// <summary>
        /// Linker Knopf sichtbar?
        /// </summary>
        public Visibility LeftButtonVisible
        {
            get { return (Visibility)GetValue(LeftButtonVisibleProperty); }
            set { SetValue(LeftButtonVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftButtonVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftButtonVisibleProperty =
            DependencyProperty.Register("LeftButtonVisible", typeof(Visibility), typeof(DialogAdapter), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Beschriftung linker Knopf als fester Text
        /// </summary>
        public string LeftButtonText
        {
            get { return (string)GetValue(LeftButtonTextProperty); }
            set { SetValue(LeftButtonTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftButtonText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftButtonTextProperty =
            DependencyProperty.Register("LeftButtonText", typeof(string), typeof(DialogAdapter), new PropertyMetadata(""));

        /// <summary>
        /// Beschriftung linker Knopf als Indextext
        /// </summary>
        public String LeftButtonLocalizableText
        {
            get { return (String)GetValue(LeftButtonLocalizableTextProperty); }
            set { SetValue(LeftButtonLocalizableTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftButtonLocalizableText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftButtonLocalizableTextProperty =
            DependencyProperty.Register("LeftButtonLocalizableText", typeof(String), typeof(DialogAdapter), new PropertyMetadata(""));

        /// <summary>
        /// der linke ist der Default-Knopf
        /// </summary>
        public bool LeftButtonIsDefault
        {
            get { return (bool)GetValue(LeftButtonIsDefaultProperty); }
            set { SetValue(LeftButtonIsDefaultProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftButtonIsDefault.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftButtonIsDefaultProperty =
            DependencyProperty.Register("LeftButtonIsDefault", typeof(bool), typeof(DialogAdapter), new PropertyMetadata(false));


        /// <summary>
        /// mittlerer Knopf sichtbar?
        /// </summary>
        public Visibility MiddleButtonVisible
        {
            get { return (Visibility)GetValue(MiddleButtonVisibleProperty); }
            set { SetValue(MiddleButtonVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MiddleButtonVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MiddleButtonVisibleProperty =
            DependencyProperty.Register("MiddleButtonVisible", typeof(Visibility), typeof(DialogAdapter), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Beschriftung linker Knopf als fester Text
        /// </summary>
        public string MiddleButtonText
        {
            get { return (string)GetValue(MiddleButtonTextProperty); }
            set { SetValue(MiddleButtonTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MiddleButtonText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MiddleButtonTextProperty =
            DependencyProperty.Register("MiddleButtonText", typeof(string), typeof(DialogAdapter), new PropertyMetadata(""));

        /// <summary>
        /// Beschriftung mittlerer Knopf als Indextext
        /// </summary>
        public string MiddleButtonLocalizableText
        {
            get { return (string)GetValue(MiddleButtonLocalizableTextProperty); }
            set { SetValue(MiddleButtonLocalizableTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MiddleButtonLocalizableText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MiddleButtonLocalizableTextProperty =
            DependencyProperty.Register("MiddleButtonLocalizableText", typeof(string), typeof(DialogAdapter), new PropertyMetadata(""));

        /// <summary>
        /// der mittlere ist der Default-Knopf
        /// </summary>
        public bool MiddleButtonIsDefault
        {
            get { return (bool)GetValue(MiddleButtonIsDefaultProperty); }
            set { SetValue(MiddleButtonIsDefaultProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftButtonIsDefault.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MiddleButtonIsDefaultProperty =
            DependencyProperty.Register("MiddleButtonIsDefault", typeof(bool), typeof(DialogAdapter), new PropertyMetadata(false));


        /// <summary>
        /// rechter Knopf sichtbar?
        /// </summary>
        public Visibility RightButtonVisible
        {
            get { return (Visibility)GetValue(RightButtonVisibleProperty); }
            set { SetValue(RightButtonVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightButtonVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightButtonVisibleProperty =
            DependencyProperty.Register("RightButtonVisible", typeof(Visibility), typeof(DialogAdapter), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Beschriftung rechter Knopf als fester Text
        /// </summary>
        public string RightButtonText
        {
            get { return (string)GetValue(RightButtonTextProperty); }
            set { SetValue(RightButtonTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightButtonText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightButtonTextProperty =
            DependencyProperty.Register("RightButtonText", typeof(string), typeof(DialogAdapter), new PropertyMetadata(""));

        /// <summary>
        /// Beschriftung rechter Knopf als Indextext
        /// </summary>
        public String RightButtonLocalizableText
        {
            get { return (String)GetValue(RightButtonLocalizableTextProperty); }
            set { SetValue(RightButtonLocalizableTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightButtonLocalizableText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightButtonLocalizableTextProperty =
            DependencyProperty.Register("RightButtonLocalizableText", typeof(String), typeof(DialogAdapter), new PropertyMetadata(""));

        /// <summary>
        /// der rechte ist der Default-Knopf
        /// </summary>
        public bool RightButtonIsDefault
        {
            get { return (bool)GetValue(RightButtonIsDefaultProperty); }
            set { SetValue(RightButtonIsDefaultProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftButtonIsDefault.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightButtonIsDefaultProperty =
            DependencyProperty.Register("RightButtonIsDefault", typeof(bool), typeof(DialogAdapter), new PropertyMetadata(false));

        #endregion Bindungen

        #region DoEvents Simulation

        /// <summary>
        /// DoEvents Simulation von VB6, praktisch zum Warten und trotzdem können die UI-Elemente bedient werden
        /// </summary>
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);

            Thread.Sleep(10);
        }

        public static object ExitFrame(object f)
        {
            ((DispatcherFrame)f).Continue = false;
            return null;
        }

        #endregion DoEvents Simulation
    }
}