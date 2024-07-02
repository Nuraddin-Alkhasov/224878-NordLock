using HMI.Views.MessageBoxRegion;
using System.Windows;
using System.Windows.Controls;
using VisiWin.ApplicationFramework;
using VisiWin.Helper;
using VisiWin.Language;
using VisiWin.Logging;

namespace HMI.UserControls
{
    public partial class H_Service : UserControl
    {
        public H_Service()
        {
            InitializeComponent();
        }

        public string StepPositive
        {
            set
            {
                step_p.VariableName = value;
            }
        }
        public string StepPositiveFast
        {
            set
            {
                step_p_f.VariableName = value;
            }
        }
        public string StepNegative
        {
            set
            {
                step_n.VariableName = value;
            }
        }
        public string StepNegativeFast
        {
            set
            {
                step_n_f.VariableName = value;
            }
        }
        public string ActualValue
        {
            set
            {
                if (value != "-")
                    actualValue.VariableName = value;
                else actualValue.Visibility = Visibility.Hidden;
            }
        }
        public string ActualValueUnit
        {
            set
            {
                if (value != "-")
                    actualValue.LocalizableUnitText = value;
            }
        }
        string _SetReferenceVar;
        public string SetReferenceVar 
        {
            get { return _SetReferenceVar; }
            set 
            {
                if (value != "-")
                    _SetReferenceVar = value;
                else reference.Visibility = Visibility.Hidden;
            }
        
        }
        public string ReferenceLogText { set; get; }

        string _DelReferenceVar;
        public string DeleteReferenceVar
        {
            get { return _DelReferenceVar; }
            set
            {
                if (value != "-")
                    _DelReferenceVar = value;
                else del_reference.Visibility = Visibility.Hidden;
            }

        }
        public string DelReferenceLogText { set; get; }

        private void ref_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxView.Show("@HandMenu.Text17", "@HandMenu.Text18", MessageBoxButton.YesNo, icon: MessageBoxIcon.Question) == MessageBoxResult.Yes)
            { 
                ApplicationService.SetVariableValue(SetReferenceVar, 1);
                ILanguageService textService = ApplicationService.GetService<ILanguageService>();

                string txt = textService.GetText(ReferenceLogText);
                ILoggingService loggingService = ApplicationService.GetService<ILoggingService>();
                loggingService.Log("Service", "New Reference", txt, FastDateTime.Now);
            }
        }

        private void delref_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxView.Show("@HandMenu.Text34", "@HandMenu.Text18", MessageBoxButton.YesNo, icon: MessageBoxIcon.Question) == MessageBoxResult.Yes)
            {
                ApplicationService.SetVariableValue(DeleteReferenceVar, 1);
                ILanguageService textService = ApplicationService.GetService<ILanguageService>();

                string txt = textService.GetText(DelReferenceLogText);
                ILoggingService loggingService = ApplicationService.GetService<ILoggingService>();
                loggingService.Log("Service", "New Reference", txt, FastDateTime.Now);
            }
        }
    }
}
