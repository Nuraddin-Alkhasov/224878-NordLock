using HMI.Diagnose;
using HMI.User;
using HMI.Views.MainRegion.Protocol;
using HMI.Views.MainRegion.Recipe;
using System.Windows;
using System.Windows.Input;
using VisiWin.ApplicationFramework;
using VisiWin.Commands;
using VisiWin.Language;

namespace HMI
{
    [ExportAdapter("AppbarViewAdapter")]
    public class AppbarViewAdapter : AdapterBase
    {
       
        private readonly ILanguageService languageService;
        readonly IRegionService iRS = ApplicationService.GetService<IRegionService>();

        public AppbarViewAdapter()
        {
            this.languageService = ApplicationService.GetService<ILanguageService>();
            languageService.LanguageChanged += LanguageService_LanguageChanged;

            this.OpenCommand = new ActionCommand(this.OpenCommandExecuted);
            this.CloseCommand = new ActionCommand(this.CloseCommandExecuted);

            this.DashboardViewOpenCommand = new ActionCommand(this.DashboardViewOpenCommandExecuted);
            this.MainViewOpenCommand = new ActionCommand(this.MainViewOpenCommandExecuted);
            this.HandModeViewOpenCommand = new ActionCommand(this.HandModeViewOpenCommandExecuted);
            this.RecipeViewOpenCommand = new ActionCommand(this.RecipeViewOpenCommandExecuted);
            this.ParameterViewOpenCommand = new ActionCommand(this.ParameterViewOpenCommandExecuted);
            this.ProtocolViewOpenCommand = new ActionCommand(this.ProtocolViewOpenCommandExecuted);
            this.LoggingViewOpenCommand = new ActionCommand(this.LoggingViewOpenCommandExecuted);
            this.OHViewOpenCommand = new ActionCommand(this.OHViewOpenCommandExecuted);
            this.UMViewOpenCommand = new ActionCommand(this.UMViewOpenCommandExecuted);
            this.DiagnoseViewOpenCommand = new ActionCommand(this.DiagnoseViewOpenCommandExecuted);

          
            this.ChangeLanguageCommand = new ActionCommand(this.ChangeLanguageCommandExecuted);
            this.LogInOutCommand = new ActionCommand(this.LogInOutCommandExecuted);
            this.ExitCommand = new ActionCommand(this.ExitCommandExecuted);
        }

        private void LanguageService_LanguageChanged(object sender, LanguageChangedEventArgs e)
        {
            var name = languageService.CurrentLanguage.Text;
            CurrentLanguage = name.Substring(0, name.IndexOf('(') - 1);
        }

        private Visibility openButtonVisibility = Visibility.Visible;
        public Visibility OpenButtonVisibility
        {
            get { return this.openButtonVisibility; }
            set
            {
                this.openButtonVisibility = value;
                this.OnPropertyChanged("OpenButtonVisibility");
            }
        }

        private Visibility closeButtonVisibility = Visibility.Collapsed;
        public Visibility CloseButtonVisibility
        {
            get { return this.closeButtonVisibility; }
            set
            {
                this.closeButtonVisibility = value;
                this.OnPropertyChanged("CloseButtonVisibility");
            }
        }
       
        private string ltDashBoardButton = "@Appbar.Text3";
        public string LTDashBoardButton
        {
            get { return this.ltDashBoardButton; }
            set
            {
                this.ltDashBoardButton = value;
                this.OnPropertyChanged("LTDashBoardButton");
            }
        }

        private string ltMainViewButton = "@Appbar.Text3";
        public string LTMainViewButton
        {
            get { return this.ltMainViewButton; }
            set
            {
                this.ltMainViewButton = value;
                this.OnPropertyChanged("LTMainViewButton");
            }
        }

        private string ltHMViewButton = "@Appbar.Text3";
        public string LTHMViewButton
        {
            get { return this.ltHMViewButton; }
            set
            {
                this.ltHMViewButton = value;
                this.OnPropertyChanged("LTHMViewButton");
            }
        }

        private string ltRecipeViewButton = "@Appbar.Text3";
        public string LTRecipeViewButton
        {
            get { return this.ltRecipeViewButton; }
            set
            {
                this.ltRecipeViewButton = value;
                this.OnPropertyChanged("LTRecipeViewButton");
            }
        }

        private string ltParameterViewButton = "@Appbar.Text3";
        public string LTParameterViewButton
        {
            get { return this.ltParameterViewButton; }
            set
            {
                this.ltParameterViewButton = value;
                this.OnPropertyChanged("LTParameterViewButton");
            }
        }
        private string ltProtocolViewButton = "@Appbar.Text3";
        public string LTProtocolViewButton
        {
            get { return this.ltProtocolViewButton; }
            set
            {
                this.ltProtocolViewButton = value;
                this.OnPropertyChanged("LTProtocolViewButton");
            }
        }

        private string ltLoggingViewButton = "@Appbar.Text3";
        public string LTLoggingViewButton
        {
            get { return this.ltLoggingViewButton; }
            set
            {
                this.ltLoggingViewButton = value;
                this.OnPropertyChanged("LTLoggingViewButton");
            }
        }

        private string ltOHViewButton = "@Appbar.Text3";
        public string LTOHViewButton
        {
            get { return this.ltOHViewButton; }
            set
            {
                this.ltOHViewButton = value;
                this.OnPropertyChanged("LTOHViewButton");
            }
        }

        private string ltUMViewButton = "@Appbar.Text3";
        public string LTUMViewButton
        {
            get { return this.ltUMViewButton; }
            set
            {
                this.ltUMViewButton = value;
                this.OnPropertyChanged("LTUMViewButton");
            }
        }

        private string ltDiagnoseViewButton = "@Appbar.Text3";
        public string LTDiagnoseViewButton
        {
            get { return this.ltDiagnoseViewButton; }
            set
            {
                this.ltDiagnoseViewButton = value;
                this.OnPropertyChanged("LTDiagnoseViewButton");
            }
        }

        private string currentLanguage = "";
        public string CurrentLanguage
        {
            get { return this.currentLanguage; }
            set
            {
                this.currentLanguage = value;
                this.OnPropertyChanged("CurrentLanguage");
            }
        }
        private string currentUser = "";
        public string CurrentUser
        {
            get { return this.currentUser; }
            set
            {
                this.currentUser = value;
                this.OnPropertyChanged("CurrentUser");
            }
        }

        private string powerOff = "";
        public string PowerOff
        {
            get { return this.powerOff; }
            set
            {
                this.powerOff = value;
                this.OnPropertyChanged("PowerOff");
            }
        }


        #region - - - Commands - - -
        public ICommand OpenCommand { get; set; }
        private void OpenCommandExecuted(object parameter)
        {
            CloseButtonVisibility = Visibility.Visible;
            OpenButtonVisibility = Visibility.Collapsed;

            LTDashBoardButton = "@Appbar.Text1";
            LTMainViewButton = "@Appbar.lblAnlageübersicht";
            LTHMViewButton = "@Appbar.lblHandmenü";
            LTRecipeViewButton  = "@Appbar.lblRezeptverwaltung";
            LTParameterViewButton = "@Appbar.lblParameter";
            LTProtocolViewButton = "@Appbar.lblAuftraege";
            LTLoggingViewButton = "@Appbar.lbllogs";
            LTOHViewButton = "@Appbar.lblBetriebstunden";
            LTUMViewButton = "@Appbar.lblBenutzerübersicht";
            LTDiagnoseViewButton = "@Appbar.lblMeldungen";

            var name = languageService.CurrentLanguage.Text;
            CurrentLanguage = name.Substring(0, name.IndexOf('(') - 1);
            CurrentUser = ApplicationService.GetVariableValue("__CURRENT_USER.FULLNAME").ToString();
            PowerOff = "@Appbar.Exit";
        }

        public ICommand CloseCommand { get; set; }
        private void CloseCommandExecuted(object parameter)
        {
            CloseButtonVisibility = Visibility.Collapsed;
            OpenButtonVisibility = Visibility.Visible;

            LTDashBoardButton = "@Appbar.Text3";
            LTMainViewButton = "@Appbar.Text3";
            LTHMViewButton = "@Appbar.Text3";
            LTRecipeViewButton = "@Appbar.Text3";
            LTParameterViewButton = "@Appbar.Text3";
            LTProtocolViewButton = "@Appbar.Text3";
            LTLoggingViewButton = "@Appbar.Text3";
            LTOHViewButton = "@Appbar.Text3";
            LTUMViewButton = "@Appbar.Text3";
            LTDiagnoseViewButton = "@Appbar.Text3";


            CurrentLanguage = "";
            CurrentUser = "";
            PowerOff = "@Appbar.Text3";
        }
        public ICommand DashboardViewOpenCommand { get; set; }
        private void DashboardViewOpenCommandExecuted(object parameter)
        {
            ApplicationService.SetView("MainRegion", "DB_DashboardView");
        }
        public ICommand MainViewOpenCommand { get; set; }
        private void MainViewOpenCommandExecuted(object parameter)
        {
            ApplicationService.SetView("MainRegion", "MO_MainView");
        }
        public ICommand HandModeViewOpenCommand { get; set; }
        private void HandModeViewOpenCommandExecuted(object parameter)
        {
            ApplicationService.SetView("MainRegion", "Hand_Main");
        }
        public ICommand RecipeViewOpenCommand { get; set; }
        private void RecipeViewOpenCommandExecuted(object parameter)
        {
            Recipe_PN R_PN = (Recipe_PN)iRS.GetView("Recipe_PN");

            if (iRS.GetCurrentViewName("MainRegion") == "Recipe_PN")
            {
                switch (R_PN.pn_recipe.SelectedPanoramaRegionIndex)
                {
                    case 0: R_PN.pn_recipe.ScrollNext(true); break;
                    case 2: R_PN.pn_recipe.ScrollPrevious(true); break;
                }
            }
            else
            {
                ApplicationService.SetView("MainRegion", "Recipe_PN");
            }
        }
        public ICommand ParameterViewOpenCommand { get; set; }
        private void ParameterViewOpenCommandExecuted(object parameter)
        {
            ApplicationService.SetView("MainRegion", "Parameter_Main");
        }
        public ICommand ProtocolViewOpenCommand { get; set; }
        private void ProtocolViewOpenCommandExecuted(object parameter)
        {
            Protocol_PN P_PN = (Protocol_PN)iRS.GetView("Protocol_PN");
            if (iRS.GetCurrentViewName("MainRegion") == "Protocol_PN")
            {
                if (P_PN.pn_protocol.SelectedPanoramaRegionIndex == 0)
                {
                    P_PN.pn_protocol.ScrollNext(true);
                }
                else
                {
                    P_PN.pn_protocol.ScrollPrevious(true);
                }
            }
            else
            {
                ApplicationService.SetView("MainRegion", "Protocol_PN");
            }
        }
        public ICommand LoggingViewOpenCommand { get; set; }
        private void LoggingViewOpenCommandExecuted(object parameter)
        {
            ApplicationService.SetView("MainRegion", "LoggingView");
        }
        public ICommand OHViewOpenCommand { get; set; }
        private void OHViewOpenCommandExecuted(object parameter)
        {
            ApplicationService.SetView("MainRegion", "OperatingHours_Main");
        }
        public ICommand UMViewOpenCommand { get; set; }
        private void UMViewOpenCommandExecuted(object parameter)
        {
            User_PN U_PN = (User_PN)iRS.GetView("User_PN");
            if (iRS.GetCurrentViewName("MainRegion") == "User_PN")
            {
                if (U_PN.pn_benutzerverwaltung.SelectedPanoramaRegionIndex == 0)
                {
                    U_PN.pn_benutzerverwaltung.ScrollNext(true);
                }
                else
                {
                    U_PN.pn_benutzerverwaltung.ScrollPrevious(true);
                }
            }
            else
            {
                ApplicationService.SetView("MainRegion", "User_PN");
            }

        }
        public ICommand DiagnoseViewOpenCommand { get; set; }
        private void DiagnoseViewOpenCommandExecuted(object parameter)
        {
            Diagnose_PN D_PN = (Diagnose_PN)iRS.GetView("Diagnose_PN");
            if (iRS.GetCurrentViewName("MainRegion") == "Diagnose_PN")
            {
                if (D_PN.pn_diagnose.SelectedPanoramaRegionIndex == 0)
                {
                    D_PN.pn_diagnose.ScrollNext(true);
                }
                else
                {
                    D_PN.pn_diagnose.ScrollPrevious(true);
                }
            }
            else
            {
                ApplicationService.SetView("MainRegion", "Diagnose_PN");
            }
        }

        public ICommand ChangeLanguageCommand { get; set; }
        private void ChangeLanguageCommandExecuted(object parameter)
        {
            switch (languageService.CurrentLanguage.LCID)
            {
                case 1031: this.languageService.ChangeLanguageAsync(1033); break;
                case 1033: this.languageService.ChangeLanguageAsync(1053); break;
                case 1053: this.languageService.ChangeLanguageAsync(1031); break;
                default: this.languageService.ChangeLanguageAsync(1031); break;
            }
        }

        public ICommand LogInOutCommand { get; set; }
        private void LogInOutCommandExecuted(object parameter)
        {
            ApplicationService.SetView("DialogRegion", "User_LogOnOff");
        }

        public ICommand ExitCommand { get; set; }
        private void ExitCommandExecuted(object parameter)
        {
            System.Windows.Application.Current.Shutdown();
        }

        
        #endregion


    }
}