using HMI.Module;

using HMI.Views.MainRegion.Recipe;
using HMI.Views.MainRegion.Recipe.Custom_Objects;
using HMI.Views.MessageBoxRegion;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisiWin.ApplicationFramework;

namespace HMI.Views.MainRegion.Protocol
{
	/// <summary>
	/// Interaction logic for ChargeView.xaml
	/// </summary>
	[ExportView("Protocol_Charges")]
	public partial class Protocol_Charges : VisiWin.Controls.View
	{
		public Protocol_Charges()
		{
			this.InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (pn_carge_run.SelectedPanoramaRegionIndex == 0)
			{
				pn_carge_run.ScrollNext();
			}
			else
			{
				pn_carge_run.ScrollPrevious();
			}
		}

		private void pn_carge_run_SelectedPanoramaRegionChanged(object sender, VisiWin.Controls.SelectedPanoramaRegionChangedEventArgs e)
		{
			if (pn_carge_run.SelectedPanoramaRegionIndex == 0)
			{
				btn.LocalizableText = "@Protocol.Text15";
				Gb_header.LocalizableHeaderText = "@Protocol.Text6";
			}
			else
			{
				btn.LocalizableText = "@Protocol.Text6";
				Gb_header.LocalizableHeaderText = "@Protocol.Text15";
			}
		}
		private void dgv_errors_PreviewTouchDown(object sender, TouchEventArgs e)
		{
			dgv_errors.UnselectAllCells();
			((DataGridRow)sender).IsSelected = true;
		}
	}
}