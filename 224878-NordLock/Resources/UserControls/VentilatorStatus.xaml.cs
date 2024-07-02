using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;

namespace HMI.UserControls
{
    public partial class VentilatorStatus : UserControl
    {
        IVariableService VS = ApplicationService.GetService<IVariableService>();
        
        IVariable VStatus;
        IVariable VPurge;
        IVariable VPS1;
        IVariable VPS2;
        public VentilatorStatus()
        {
            InitializeComponent();
        }
        public string LocalizableHeaderText
        {
            set
            {
                Header.LocalizableText = value;
            }
        }
        public string VentilatorOnVariable
        {
            set
            {
                V_On.VariableName = value;
            }
        }
        public string VentilatorOffVariable
        {
            set
            {
                V_Off.VariableName = value;
            }
        }
        public string VentilatorStatusVariable
        {
            set
            {
                VStatus = VS.GetVariable(value);
                VStatus.Change += VStatus_Change;
            }
        }
        public string NewStartVariableMin
        {
            set
            {
                NSTime_Min.VariableName = value;
            }
        }
        public string NewStartVariableSec
        {
            set
            {
                NSTime_Sec.VariableName = value;
            }
        }
        public string PurgeVariableMin
        {
            set
            {
                PTime_Min.VariableName = value;
            }
        }
        public string PurgeVariableSec
        {
            set
            {
                PTime_Sec.VariableName = value;
            }
        }
        public string PurgeStatusVariable
        {
            set
            {
                VPurge = VS.GetVariable(value);
                VPurge.Change += VPurge_Change;
            }
        }

        public string PS1StatusVariable
        {
            set
            {
                if (value != "-")
                {
                    VPS1 = VS.GetVariable(value);
                    VPS1.Change += VPS1_Change;
                }
                else { PS1.Visibility = Visibility.Hidden; }
               
            }
        }
        public string PS2StatusVariable
        {
            set
            {
                if (value != "-")
                {
                    VPS2 = VS.GetVariable(value);
                    VPS2.Change += VPS2_Change;
                }
                else { PS2.Visibility = Visibility.Hidden; }
            }
        }


        private void VStatus_Change(object sender, VariableEventArgs e)
        {
            switch ((short)e.Value) 
            {
                case 0:
                    V_On.IsDefault = false;
                    V_On.IsBlinkEnabled = false;
                    V_Off.IsDefault = true;
                    break;
                case 1:
                    V_On.IsDefault = true;
                    V_On.IsBlinkEnabled = false;
                    V_Off.IsDefault = false;
                    break;
                case 2:
                    V_On.IsDefault = true;
                    V_On.IsBlinkEnabled = true;
                    V_Off.IsDefault = false;
                    break;
            }
        }
        private void VPurge_Change(object sender, VariableEventArgs e)
        {
            if((bool)e.Value)
                Purge.Visibility = Visibility.Visible;
            else
                Purge.Visibility = Visibility.Hidden; 
            
        }
        private void VPS1_Change(object sender, VariableEventArgs e)
        {
            if ((bool)e.Value)
            {
                PS1.Background = (LinearGradientBrush)Application.Current.Resources["FP_Green_Gradient"];
                PS1.IsBlinkEnabled = false;
            }
            else
            {
                PS1.Background = (LinearGradientBrush)Application.Current.Resources["FP_Yellow_Gradient"];
                PS1.IsBlinkEnabled = true;
            } 
        }
        private void VPS2_Change(object sender, VariableEventArgs e)
        {
            if ((bool)e.Value)
            {
                PS2.Background = (LinearGradientBrush)Application.Current.Resources["FP_Green_Gradient"];
                PS2.IsBlinkEnabled = false;
            }
            else
            {
                PS2.Background = (LinearGradientBrush)Application.Current.Resources["FP_Yellow_Gradient"];
                PS2.IsBlinkEnabled = true;
            }
        }

        private bool loaded=false;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!loaded)
            {
                Task obTask = Task.Run(async () =>
                {
                    await Task.Delay(100);
                    await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                     {
                         A.BeginAnimation(UIElement.OpacityProperty, SetOpacity(1, 1));
                     });
              });

                loaded = true;
            }
        }
        private DoubleAnimation SetOpacity(Double _O, int _T)
        {
            return new DoubleAnimation
            {
                To = _O,
                Duration = TimeSpan.FromSeconds(_T),
            };
        }
    }
}
