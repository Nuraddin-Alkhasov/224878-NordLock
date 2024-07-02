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
    [ExportView("P_M1_KBD")]
    public partial class P_M1_KBD : View
    {
		
        public P_M1_KBD()
        {
            this.InitializeComponent();
            
        }

        private void P_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Task obTask = Task.Run(() =>
            {
                Application.Current.Dispatcher.InvokeAsync((Action)delegate
                {
                    Reg.Content = new P_M1_KBD_P();
                });
            });
        }

        private void Reg_Unloaded(object sender, RoutedEventArgs e)
        {
            Reg.Content = null;
        }
    }
}