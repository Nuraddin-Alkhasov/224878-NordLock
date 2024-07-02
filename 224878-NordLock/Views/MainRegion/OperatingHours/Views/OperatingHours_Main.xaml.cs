using HMI.Views.DialogRegion;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Views.MainRegion.OperatingHours
{
    /// <summary>
    /// Interaction logic for ButtonsView.xaml
    /// </summary>
    [ExportView("OperatingHours_Main")]
   
    public partial class OperatingHours_Main : View
    {
       
        public OperatingHours_Main()
        {
            this.InitializeComponent();
        }

        //private void NavigationButton_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    DialogView.Show(((VisiWin.Controls.NavigationButton)sender).Tag.ToString(), "@Appbar.lblBetriebstunden", DialogButton.Close, DialogResult.Cancel);
        //}
    }
}