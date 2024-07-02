using System.Windows.Controls;
using System.Windows.Input;
using VisiWin.ApplicationFramework;

namespace HMI.Views.MainRegion.Protocol
{
	/// <summary>
	/// Interaction logic for BSStepEdit.xaml
	/// </summary>
	[ExportView("Protocol_DT_Charges")]
	public partial class Protocol_DT_Charges : VisiWin.Controls.View
	{

        public Protocol_DT_Charges()
		{
			this.InitializeComponent();
        }
		private void dgv_charges_PreviewTouchDown(object sender, TouchEventArgs e)
		{
			dgv_charges.UnselectAllCells();
			((DataGridRow)sender).IsSelected = true;

		}
	}
}