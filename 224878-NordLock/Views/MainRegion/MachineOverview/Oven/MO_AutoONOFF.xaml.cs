using VisiWin.ApplicationFramework;

namespace HMI.Views.MainRegion.MachineOverview
{

	[ExportView("MO_AutoONOFF")]
	public partial class MO_AutoONOFF : VisiWin.Controls.View
	{

        public MO_AutoONOFF()
		{
			this.InitializeComponent();
          
        }

        private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ApplicationService.SetView("DialogRegion", "EmptyView");
        }

        private void LeftButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ApplicationService.SetVariableValue("NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.00 Allgemein.DB Zeitschaltuhr HMI.Aktiv", true);
        }

        private void RightButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ApplicationService.SetVariableValue("NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.00 Allgemein.DB Zeitschaltuhr HMI.Aktiv", false);
        }
    }
}