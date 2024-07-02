using HMI.Views.DialogRegion;
using VisiWin.ApplicationFramework;

namespace HMI.OperatingHours
{
	/// <summary>
	/// Interaction logic for HandmenüDashboard.xaml
	/// </summary>
	[ExportView("OH_M1_Main")]
	public partial class OH_M1_Main : VisiWin.Controls.View
	{
		public OH_M1_Main()
		{
			this.InitializeComponent();
        }

		private void NavigationButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			DialogView.Show("OH_M1_LD", "@Appbar.lblBetriebstunden", DialogButton.Close, DialogResult.Cancel);
		}

		private void NavigationButton_Click_1(object sender, System.Windows.RoutedEventArgs e)
		{
			DialogView.Show("OH_M1_HC", "@Appbar.lblBetriebstunden", DialogButton.Close, DialogResult.Cancel);
		}

		private void NavigationButton_Click_2(object sender, System.Windows.RoutedEventArgs e)
		{
			DialogView.Show("OH_M1_DC", "@Appbar.lblBetriebstunden", DialogButton.Close, DialogResult.Cancel);
		}

		private void NavigationButton_Click_3(object sender, System.Windows.RoutedEventArgs e)
		{
			DialogView.Show("OH_M1_BS", "@Appbar.lblBetriebstunden", DialogButton.Close, DialogResult.Cancel);
		}
	}
}