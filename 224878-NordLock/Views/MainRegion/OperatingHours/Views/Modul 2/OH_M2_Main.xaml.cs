using HMI.Views.DialogRegion;
using VisiWin.ApplicationFramework;

namespace HMI.OperatingHours
{
	/// <summary>
	/// Interaction logic for HandmenüDashboard.xaml
	/// </summary>
	[ExportView("OH_M2_Main")]
	public partial class OH_M2_Main : VisiWin.Controls.View
	{
		public OH_M2_Main()
		{
			this.InitializeComponent();
        }

		private void NavigationButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			DialogView.Show("OH_M2_Centrifuge", "@Appbar.lblBetriebstunden", DialogButton.Close, DialogResult.Cancel);
		}

		private void NavigationButton_Click_1(object sender, System.Windows.RoutedEventArgs e)
		{
			DialogView.Show("OH_M2_Tilt", "@Appbar.lblBetriebstunden", DialogButton.Close, DialogResult.Cancel);
		}

		private void NavigationButton_Click_2(object sender, System.Windows.RoutedEventArgs e)
		{
			DialogView.Show("OH_M2_LTB", "@Appbar.lblBetriebstunden", DialogButton.Close, DialogResult.Cancel);
		}

		private void NavigationButton_Click_3(object sender, System.Windows.RoutedEventArgs e)
		{
			DialogView.Show("OH_M2_Ventilators", "@Appbar.lblBetriebstunden", DialogButton.Close, DialogResult.Cancel);
		}
	}
}