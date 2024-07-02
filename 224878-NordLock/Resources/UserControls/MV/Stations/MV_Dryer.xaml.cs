using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;

namespace HMI.UserControls
{
    public partial class MV_Dryer : UserControl
    {
        public MV_Dryer()
        {
            InitializeComponent();
        }
        IVariableService VS = ApplicationService.GetService<IVariableService>();

        IVariable VWN_ONachlauf;
        IVariable VWN_PHZNachlauf;
        IVariable VWN_OvenStatus;
        IVariable VWN_OvenEmptyA;
        IVariable VWN_OvenEmptyM;
        IVariable VWN_OvenDCycle;
        IVariable VWN_PHZPurge;
        IVariable VWN_OPurge;

        private bool loaded=false;
        public string PHZTemperature
        {
            set
            {
                PHZTemp.VariableName = value;
            }
        }
        public string PHZ_RPM
        {
            set
            {
                PHZUmluft.VariableName = value;
            }
        }

        public string OvenStatus
        {
            set
            {
                VWN_OvenStatus = VS.GetVariable(value);
                VWN_OvenStatus.Change += VWN_OvenStatus_Change;
            }
        }
        private void VWN_OvenStatus_Change(object sender, VariableEventArgs e)
        {
            switch ((short)e.Value) 
            {
                case 1: vh.Background = (System.Windows.Media.Brush)FindResource("FP_LightGreen_Gradient"); vh.IsBlinkEnabled = false; break;
                case 2: vh.Background = (System.Windows.Media.Brush)FindResource("FP_LightGreen_Gradient"); vh.IsBlinkEnabled = true; break;
                default: vh.Background = (System.Windows.Media.Brush)FindResource("FP_Red_Gradient"); vh.IsBlinkEnabled = false; break;
            }
           
        }

        public string PHZ_Nachlauf
        {
            set
            {
                VWN_PHZNachlauf = VS.GetVariable(value);
                VWN_PHZNachlauf.Change += VWN_PHZNachlauf_Change;
            }
        }
        private void VWN_PHZNachlauf_Change(object sender, VariableEventArgs e)
        {
            if ((bool)e.Value)
            {
                phzNL.Visibility=Visibility.Visible;
            }
            else
            {
                phzNL.Visibility = Visibility.Hidden;
            }
        }
        public string OvenTemperature
        {
            set
            {
                OTemp.VariableName = value;
            }
        }
        public string O_RPM
        {
            set
            {
                OUmluft.VariableName = value;
            }
        }
        public string O_Nachlauf
        {
            set
            {
                VWN_ONachlauf = VS.GetVariable(value);
                VWN_ONachlauf.Change += VWN_ONachlauf_Change;
            }
        }
        private void VWN_ONachlauf_Change(object sender, VariableEventArgs e)
        {
            if ((bool)e.Value)
            {
                oNL.Visibility = Visibility.Visible;
            }
            else
            {
                oNL.Visibility = Visibility.Hidden;
            }
        }
       
        public string OvenEmptyAuto
        {
            set
            {
                VWN_OvenEmptyA = VS.GetVariable(value);
                VWN_OvenEmptyA.Change += VWN_OvenEmptyA_Change;
            }
        }
        private void VWN_OvenEmptyA_Change(object sender, VariableEventArgs e)
        {
            if ((short)e.Value >= 1)
            {
                emptya.Visibility = Visibility.Visible;
            }
            else
            {
                emptya.Visibility = Visibility.Hidden;
            }
        }

    
        public string OvenEmptyManual
        {
            set
            {
                VWN_OvenEmptyM = VS.GetVariable(value);
                VWN_OvenEmptyM.Change += VWN_OvenEmptyManual_Change;
            }
        }
        private void VWN_OvenEmptyManual_Change(object sender, VariableEventArgs e)
        {
            if ((short)e.Value>=1)
            {
                emptym.Visibility = Visibility.Visible;
            }
            else
            {
                emptym.Visibility = Visibility.Hidden;
            }
        }

        public string OvenDoubleCycle
        {
            set
            {
                VWN_OvenDCycle = VS.GetVariable(value);
                VWN_OvenDCycle.Change += VWN_OvenDCycle_Change;
            }
        }
        private void VWN_OvenDCycle_Change(object sender, VariableEventArgs e)
        {
            if ((short)e.Value >= 1)
            {
                takt.Visibility = Visibility.Visible;
            }
            else
            {
                takt.Visibility = Visibility.Hidden;
            }
        }
        public string O_Purge
        {
            set
            {
                VWN_OPurge = VS.GetVariable(value);
                VWN_OPurge.Change += VWN_OPurge_Change;
            }
        }
        private void VWN_OPurge_Change(object sender, VariableEventArgs e)
        {
            if ((bool)e.Value)
            {
                oPT.Visibility = Visibility.Visible;
            }
            else
            {
                oPT.Visibility = Visibility.Hidden;
            }
        }
        public string PHZ_Purge
        {
            set
            {
                VWN_PHZPurge = VS.GetVariable(value);
                VWN_PHZPurge.Change += VWN_PHZPurge_Change;
            }
        }
        private void VWN_PHZPurge_Change(object sender, VariableEventArgs e)
        {
            if ((bool)e.Value)
            {
                phzPT.Visibility = Visibility.Visible;
            }
            else
            {
                phzPT.Visibility = Visibility.Hidden;
            }
        }

        public string OCycleMin
        {
            set
            {
                Tmin.VariableName = value;
            }
        }
        public string OCycleSec
        {
            set
            {
                Tsec.VariableName = value;
            }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!loaded)
            {
                Task obTask = Task.Run(async () =>
                {
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
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("MainRegion", "MO_Oven");
        }

    }
}
