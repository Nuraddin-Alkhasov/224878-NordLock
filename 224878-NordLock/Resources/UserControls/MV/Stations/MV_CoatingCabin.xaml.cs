using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;

namespace HMI.UserControls
{
    public partial class MV_CoatingCabin : UserControl
    {
        public MV_CoatingCabin()
        {
            InitializeComponent();
        }
        IVariableService VS = ApplicationService.GetService<IVariableService>();

        IVariable doorStatus;
        IVariable VWN_isLTB;
        IVariable VWN_PT;
        public string LTBTemperature
        {
            set
            {
                ltbTemp.VariableName = value;
            }
        }

        public string isLTB
        {
            set
            {
                VWN_isLTB = VS.GetVariable(value);
                VWN_isLTB.Change += VWN_isLTB_Change;
            }
        }
        private void VWN_isLTB_Change(object sender, VariableEventArgs e)
        {
            if ((bool)e.Value)
            {
                Task obTask = Task.Run(async () =>
                {
                    await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                    {
                        LTB.BeginAnimation(UIElement.OpacityProperty, SetOpacity(1, 1));
                    });
                });
            }
            else
            {
                Task obTask = Task.Run(async () =>
                {
                    await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                    {
                        LTB.BeginAnimation(UIElement.OpacityProperty, SetOpacity(0, 1));
                    });
                });
            }
        }
        public string DoorStatus
        {
            set
            {
                doorStatus = VS.GetVariable(value);
                doorStatus.Change += doorStatus_ValueChanged;
            }
        }

        private void doorStatus_ValueChanged(object sender, VariableEventArgs e)
        {
            if ((bool)e.Value)
            {
                Door.SymbolResourceKey = "CoatingDoorOpen";
            }
            else
            {
                Door.SymbolResourceKey = "CoatingDoorClosed";
            }
        }


        public string PurgeStatus
        {
            set
            {
                VWN_PT = VS.GetVariable(value);
                VWN_PT.Change += PurgeStatus_ValueChanged;
            }
        }

        private void PurgeStatus_ValueChanged(object sender, VariableEventArgs e)
        {
            if ((bool)e.Value)
            {
                PT.Visibility=Visibility.Visible;
            }
            else
            {
                PT.Visibility = Visibility.Hidden;
            }
        }
        private bool loaded=false;

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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("MainRegion", "MO_CoatingCabin");
        }
    }
}
