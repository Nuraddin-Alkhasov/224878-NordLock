using System;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
namespace HMI.Parameter
{
    /// <summary>
    /// Interaction logic for DigitalIOView.xaml
    /// </summary>
    [ExportView("P_M4_TD")]
    public partial class P_M4_TD : View
    {
		
        public P_M4_TD()
        {
            this.InitializeComponent();
            
        }
        private void G_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            Reg.Content = new P_M4_TD_G();
        }
        private void P_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            Reg.Content = new P_M4_TD_P();
        }

        private void B_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            Reg.Content = new P_M4_TD_B();
        }

        

        private void P_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Task obTask = Task.Run(() =>
            {
                Application.Current.Dispatcher.InvokeAsync((Action)delegate
                {
                    btnH_G.IsChecked = true;
                });
            });
        }

        private void Reg_Unloaded(object sender, RoutedEventArgs e)
        {
            btnH_G.IsChecked = false;
            Reg.Content = null;
        }
    }
}