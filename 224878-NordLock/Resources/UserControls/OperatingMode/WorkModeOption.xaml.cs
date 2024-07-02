using HMI.Views.MessageBoxRegion;
using System;
using System.Security.RightsManagement;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;
using VisiWin.Language;

namespace HMI.UserControls
{
    public partial class WorkModeOption : UserControl
    {
        IVariableService VS = ApplicationService.GetService<IVariableService>();
        ILanguageService TS = ApplicationService.GetService<ILanguageService>();

        public WorkModeOption()
        {
            InitializeComponent();
            TS.LanguageChanged += TS_LanguageChanged;
        }

        string _header = "";
        public string Header 
        {
            set 
            {
                _header = value;
                H.Header = TS.GetText(value);
            }
        }
        private void TS_LanguageChanged(object sender, LanguageChangedEventArgs e)
        {
            H.Header = TS.GetText(_header);
        }

        IVariable VW_Status;
        public string Status_VW
        {
            set
            {
                VW_Status = VS.GetVariable(value);
                VW_Status.Change += Status_Change;
            }
        }
        private void Status_Change(object sender, VariableEventArgs e)
        {
            switch ((short)e.Value)
            {
                case 0: btnstart.IsDefault = false; btnstart.IsBlinkEnabled = false; break;
                case 1: btnstart.IsDefault = true; btnstart.IsBlinkEnabled = false; break;
                case 2: btnstart.IsDefault = false; btnstart.IsBlinkEnabled = true; break;
            }
        }

        IVariable VW_isRelease;
        public string isRelease_VW
        {
            set
            {
                VW_isRelease = VS.GetVariable(value);
                VW_isRelease.Change += isRelease_Change;
            }
        }
        private void isRelease_Change(object sender, VariableEventArgs e)
        {
            if ((bool)e.Value)
            {
               btnstart.IsEnabled = true;
            }
            else 
            {
                btnstart.IsEnabled = false;
            }
        }

        public string Start_VW { get; set; }
        

        public string Start_Text
        {
            set
            {
                btnstart.LocalizableText = value;
            }
        }

        public string Stop_VW
        {
            set
            {
                btnstop.VariableName = value;
            }
        }

        public string Stop_Text
        {
            set
            {
                btnstop.LocalizableText = value;
            }
        }

        public string AuthorizationRight 
        {
            set
            {
                btnstart.AuthorizationRight = value;
                btnstop.AuthorizationRight = value;
            }
        }

        public bool isWithCheck { set; get; }
        private void btnstart_Click(object sender, RoutedEventArgs e)
        {
            if (isWithCheck)
            {
                if (MessageBoxView.Show("@MessageBox.Text3", _header, MessageBoxButton.YesNo, MessageBoxResult.No, MessageBoxIcon.Question) == MessageBoxResult.Yes) 
                {
                    SetValue();
                } 
                
            }
            else 
            {
                SetValue();
            }
        }

        private void SetValue() 
        {
            Task taskA = Task.Run(() =>
            {
                ApplicationService.SetVariableValue(Start_VW, true);
            });
            taskA.ContinueWith(async x =>
            {
                await Task.Delay(800);
                ApplicationService.SetVariableValue(Start_VW, false);
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }
}
