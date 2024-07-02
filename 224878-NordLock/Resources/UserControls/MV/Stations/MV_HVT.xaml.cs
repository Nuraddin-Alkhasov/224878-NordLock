using HMI.Views.MainRegion.MachineOverview;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;

namespace HMI.UserControls
{
    public partial class MV_HVT : UserControl
    {
        public MV_HVT()
        {
            InitializeComponent();
        }
        IVariableService VS = ApplicationService.GetService<IVariableService>();

        IVariable hvtPosition;
        public string HVTPosition
        {
            set
            {
                hvtPosition = VS.GetVariable(value);
                hvtPosition.Change += hvtPosition_ValueChanged;
            }
        }
        double Oldpos = 0;
        private void hvtPosition_ValueChanged(object sender, VariableEventArgs e)
        {
            double pos = Math.Round(((float)e.Value) / 20.9586);

            if (Oldpos != pos)
            {
                HVT.Margin = new Thickness(3, 0, 0, pos + 3);
                Oldpos = pos;
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

    }
}
