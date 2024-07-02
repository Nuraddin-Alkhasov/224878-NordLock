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
    public partial class MV_HNT : UserControl
    {
        public MV_HNT()
        {
            InitializeComponent();
        }
        IVariableService VS = ApplicationService.GetService<IVariableService>();

        IVariable hntPosition;
        public string HNTPosition
        {
            set
            {
                hntPosition = VS.GetVariable(value);
                hntPosition.Change += hntPosition_ValueChanged;
            }
        }
        double Oldpos = 0;
        private void hntPosition_ValueChanged(object sender, VariableEventArgs e)
        {
            double pos = Math.Round(((float)e.Value) / 21.4575);

            if (Oldpos != pos)
            {
                HNT.Margin = new Thickness(3, pos + 2, 0, 0);
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            (new SP
            {
                Module = 4,
                M1_Station = 0,
                M2_Station = 0,
                M3_Station = 0,
                M4_Station = 4,
                OvenTray = 0,
                TB_Shelve = 0,
                TB_Level = 0,
                Header = "@Status.Text89",
                Type = "Tray"
            }).Execute();
        }
    }
}
