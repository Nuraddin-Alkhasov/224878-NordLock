using HMI.UserControls;
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
    [ExportView("P_M4_Temp")]
    public partial class P_M4_Temp : View
    {
		
        public P_M4_Temp()
        {
            this.InitializeComponent();
            
        }

        private void B1_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            Reg.Content = new P_M4_T_PD_P();
        }

        private void B2_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            Reg.Content = new P_M4_T_D_P();
        }
        private void B3_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            Reg.Content = new P_M4_T_KZ_P();
        }


        private void P_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Task obTask = Task.Run(() =>
            {
                Application.Current.Dispatcher.InvokeAsync((Action)delegate
                {
                    btn1.IsChecked = true;
                });
            });
        }

        private void Reg_Unloaded(object sender, RoutedEventArgs e)
        {
            btn1.IsChecked = false;
            Reg.Content = null;
        }
    }
}