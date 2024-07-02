using HMI.Views.DialogRegion;
using VisiWin.ApplicationFramework;

namespace HMI.OperatingHours
{
	/// <summary>
	/// Interaction logic for HandmenüDashboard.xaml
	/// </summary>
	[ExportView("OH_M4_Main")]
	public partial class OH_M4_Main : VisiWin.Controls.View
	{
		public OH_M4_Main()
		{
			this.InitializeComponent();
        }

		private void NavigationButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			DialogView.Show("OH_M4_Mani", "@Appbar.lblBetriebstunden", DialogButton.Close, DialogResult.Cancel);
		}

		private void NavigationButton_Click_1(object sender, System.Windows.RoutedEventArgs e)
		{
			DialogView.Show("OH_M4_HVT", "@Appbar.lblBetriebstunden", DialogButton.Close, DialogResult.Cancel);
		}

		private void NavigationButton_Click_2(object sender, System.Windows.RoutedEventArgs e)
		{
			DialogView.Show("OH_M4_HNT", "@Appbar.lblBetriebstunden", DialogButton.Close, DialogResult.Cancel);
		}

		private void NavigationButton_Click_3(object sender, System.Windows.RoutedEventArgs e)
		{
			DialogView.Show("OH_M4_TD", "@Appbar.lblBetriebstunden", DialogButton.Close, DialogResult.Cancel);
		}

		private void NavigationButton_Click_4(object sender, System.Windows.RoutedEventArgs e)
		{
			DialogView.Show("OH_M4_TKZ", "@Appbar.lblBetriebstunden", DialogButton.Close, DialogResult.Cancel);

		}

		private void NavigationButton_Click_5(object sender, System.Windows.RoutedEventArgs e)
		{
			DialogView.Show("OH_M4_Ventilators", "@Appbar.lblBetriebstunden", DialogButton.Close, DialogResult.Cancel);
		}

		private void NavigationButton_Click_6(object sender, System.Windows.RoutedEventArgs e)
		{
			DialogView.Show("OH_M4_Heating", "@Appbar.lblBetriebstunden", DialogButton.Close, DialogResult.Cancel);
		}
	}
}