using HMI.Reporting;
using HMI.Views.MainRegion.Protocol.Custom_Objects;
using VisiWin.ApplicationFramework;

namespace HMI.Views.MainRegion.Protocol
{
	/// <summary>
	/// Interaction logic for ProtocolPN.xaml
	/// </summary>
	[ExportView("Protocol_PN")]
	public partial class Protocol_PN : VisiWin.Controls.View
	{
		public Protocol_PN()
		{
			this.InitializeComponent();
            IRegionService iRS = ApplicationService.GetService<IRegionService>();
            PC = (Protocol_Charges)iRS.GetView("Protocol_Charges");
        }
        readonly Protocol_Charges PC;
        private void Pn_protocol_SelectedPanoramaRegionChanged(object sender, VisiWin.Controls.SelectedPanoramaRegionChangedEventArgs e)
        {
            if (pn_protocol.SelectedPanoramaRegionIndex == 0)
            {
                header.LocalizableText = "@Appbar.lblAuftraege";

                ProtocolAdapter a = ApplicationService.GetAdapter(nameof(ProtocolAdapter)) as ProtocolAdapter;
                a.SelectedRun = null;
            }
            else
            {
                header.LocalizableText = "@Protocol.Text6";
                if (PC.pn_carge_run.SelectedPanoramaRegionIndex != 0)
                {
                    PC.pn_carge_run.ScrollPrevious();
                }
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var adapter = ApplicationService.GetAdapter(nameof(ReportViewAdapter)) as ReportViewAdapter;
            if (pn_protocol.SelectedPanoramaRegionIndex == 0)
            {
                adapter?.OpenView("DialogRegion", (t) => OrdersReport.GetReportConfiguration());
            }
            else
            {
                
                if (PC.pn_carge_run.SelectedPanoramaRegionIndex == 0)
                {
                    adapter?.OpenView("DialogRegion", (t) => ChargesReport.GetReportConfiguration());
                }
                else 
                {
                    adapter?.OpenView("DialogRegion", (t) => RunsReport.GetReportConfiguration());
                }
            }
        }
    }
}