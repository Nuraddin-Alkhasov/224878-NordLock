using HMI.Views.MainRegion.MachineOverview;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;

namespace HMI.UserControls
{
    /// <summary>
    /// Interaction logic for UserControl2.xaml
    /// </summary>
    public partial class Tracking : UserControl

    {
        public Tracking()
        {
            InitializeComponent();
        }

        public string Order
        {
            set
            {
                order.Text = value;
            }
        }

        private bool loaded = false;

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
