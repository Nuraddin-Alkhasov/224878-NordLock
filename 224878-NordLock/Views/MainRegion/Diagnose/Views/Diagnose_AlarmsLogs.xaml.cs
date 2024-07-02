using System.Windows;
using VisiWin.ApplicationFramework;

namespace HMI.Diagnose
{
	/// <summary>
	/// Interaction logic for AlarmHistoryView.xaml
	/// </summary>
    [ExportView("Diagnose_AlarmsLogs")]
	public partial class Diagnose_AlarmsLogs : VisiWin.Controls.View
	{
		public Diagnose_AlarmsLogs()
		{
			this.InitializeComponent();
		}

     
        private void PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {

        }
    }
}