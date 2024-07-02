using System.Windows;
using VisiWin.ApplicationFramework;

namespace HMI.Diagnose
{
	/// <summary>
	/// Interaction logic for View2.xaml
	/// </summary>
	[ExportView("Diagnose_PN")]
	public partial class Diagnose_PN : VisiWin.Controls.View
	{

		
		public Diagnose_PN()
		{
			this.InitializeComponent();
		
		}

        private void Pn_diagnose_SelectedPanoramaRegionChanged(object sender, VisiWin.Controls.SelectedPanoramaRegionChangedEventArgs e)
        {
            if (pn_diagnose.SelectedPanoramaRegionIndex == 0)
            {
                header.LocalizableText = "@HistoricalAlarms.Text6";
            }
            else
            {
                header.LocalizableText = "@HistoricalAlarms.Text7";
            }
        }
    }
}