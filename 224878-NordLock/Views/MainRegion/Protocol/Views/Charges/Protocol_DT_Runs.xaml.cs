using System.Windows.Controls;
using System.Windows.Input;
using VisiWin.ApplicationFramework;

namespace HMI.Views.MainRegion.Protocol
{
	/// <summary>
	/// Interaction logic for BSStepEdit.xaml
	/// </summary>
	[ExportView("Protocol_DT_Runs")]
	public partial class Protocol_DT_Runs : VisiWin.Controls.View
	{

        public Protocol_DT_Runs()
		{
			this.InitializeComponent();
        }
		private void dgv_runs_PreviewTouchDown(object sender, TouchEventArgs e)
		{
			dgv_runs.UnselectAllCells();
			((DataGridRow)sender).IsSelected = true;
		}
	}
}