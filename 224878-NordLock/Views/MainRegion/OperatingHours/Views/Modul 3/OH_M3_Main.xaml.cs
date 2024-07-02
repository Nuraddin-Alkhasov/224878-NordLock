using HMI.Views.DialogRegion;
using VisiWin.ApplicationFramework;

namespace HMI.OperatingHours
{
	/// <summary>
	/// Interaction logic for HandmenüDashboard.xaml
	/// </summary>
	[ExportView("OH_M3_Main")]
	public partial class OH_M3_Main : VisiWin.Controls.View
	{
		public OH_M3_Main()
		{
			this.InitializeComponent();
        }

		private void NavigationButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			DialogView.Show("OH_M3_Mani", "@Appbar.lblBetriebstunden", DialogButton.Close, DialogResult.Cancel);
		}

		private void NavigationButton_Click_1(object sender, System.Windows.RoutedEventArgs e)
		{
			DialogView.Show("OH_M3_KA", "@Appbar.lblBetriebstunden", DialogButton.Close, DialogResult.Cancel);

		}

		private void NavigationButton_Click_2(object sender, System.Windows.RoutedEventArgs e)
		{
			DialogView.Show("OH_M3_MC1", "@Appbar.lblBetriebstunden", DialogButton.Close, DialogResult.Cancel);
		}

		private void NavigationButton_Click_3(object sender, System.Windows.RoutedEventArgs e)
		{
			DialogView.Show("OH_M3_MC2", "@Appbar.lblBetriebstunden", DialogButton.Close, DialogResult.Cancel);
		}

		private void NavigationButton_Click_4(object sender, System.Windows.RoutedEventArgs e)
		{
			DialogView.Show("OH_M3_BC", "@Appbar.lblBetriebstunden", DialogButton.Close, DialogResult.Cancel);
		}
	}
}