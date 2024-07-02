using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;

namespace HMI.UserControls
{
    public partial class MV_KZ : UserControl
    {
        public MV_KZ()
        {
            InitializeComponent();
        }
        IVariableService VS = ApplicationService.GetService<IVariableService>();

        IVariable doorStatus;

        public string CZTemperature
        {
            set
            {
                CZTemp.VariableName = value;
            }
        }
        public string CZUmluft
        {
            set
            {
                czUmluft.VariableName = value;
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
                Door.SymbolResourceKey = "KZDoorClosed";
            }
            else
            {
                Door.SymbolResourceKey = "KZDoorOpen";
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
            ApplicationService.SetView("MainRegion", "MO_CZ");
        }
    }
}
