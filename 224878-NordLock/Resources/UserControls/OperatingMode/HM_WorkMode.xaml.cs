using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;

namespace HMI.UserControls
{
    public partial class HM_WorkMode : UserControl
    {
       
        string var_Automaric = "";
        string var_Manual = "";
        string var_SetUP = "";

        public string PowerOn
        {
            set
            { 
                PWO = VS.GetVariable(value);
                PWO.Change += PWO_Change;
            }
        }

        public string WorkingModeStatus
        {
            set
            {
                WM = VS.GetVariable(value);
                WM.Change += WorkingMode_ValueChanged;
            }
        }

        public string Automatic
        {
            set
            {
                var_Automaric = value;
            }
        }

        public string Hand
        {
            set
            {
                var_Manual = value;
            }
        }

        public string SetUp
        {
            set
            {
                var_SetUP = value;
            }
        }

        

        IVariableService VS = ApplicationService.GetService<IVariableService>();
        IVariable WM;
        IVariable PWO;

        public HM_WorkMode()
        {
            InitializeComponent();
           
        }

        private void PWO_Change(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue && (short)e.Value == 2)
            {
                if (!(bool)ApplicationService.GetVariableValue(var_Automaric) && !(bool)ApplicationService.GetVariableValue(var_Manual) && !(bool)ApplicationService.GetVariableValue(var_SetUP))
                {
                    ApplicationService.SetVariableValue(var_Automaric, false);
                    ApplicationService.SetVariableValue(var_Manual, false);
                    ApplicationService.SetVariableValue(var_SetUP, true);
                }
            }
        }

        private void WorkingMode_Click(object sender, RoutedEventArgs e)
        {
            switch (WM.Value.ToString())
            {
                case "1":
                    ApplicationService.SetVariableValue(var_Automaric, false);
                    ApplicationService.SetVariableValue(var_Manual, false);
                    ApplicationService.SetVariableValue(var_SetUP, true);
                    break;
                default:
                    ApplicationService.SetVariableValue(var_Automaric, false);
                    ApplicationService.SetVariableValue(var_SetUP, false);
                    ApplicationService.SetVariableValue(var_Manual, true);
                    break;
            }
        }

        private void WorkingMode_ValueChanged(object sender, VariableEventArgs e)
        {
            switch ((short)e.Value)
            {
                default:                    //no mode
                    WorkingMode.LocalizableText = "@Lists.OperatingMode.Text1";
                    WorkingMode.Tag = "0";
                    break;
                case 1:                    //hand 
                    WorkingMode.LocalizableText = "@Lists.OperatingMode.Text2";
                    WorkingMode.Tag = "1";
                    break;
                case 2:                    //set up
                    WorkingMode.LocalizableText = "@Lists.OperatingMode.Text3";
                    WorkingMode.Tag = "2";
                    break;
                case 3:                    //auto
                    WorkingMode.LocalizableText = "@Lists.OperatingMode.Text4";
                    WorkingMode.Tag = "3";
                    break;
            }
        } 
    }
}
