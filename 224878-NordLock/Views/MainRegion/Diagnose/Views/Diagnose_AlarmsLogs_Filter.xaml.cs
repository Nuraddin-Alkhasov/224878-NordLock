using VisiWin.ApplicationFramework;

namespace HMI.Diagnose
{
	/// <summary>
	/// Interaction logic for AlarmHistoryFilterView.xaml
	/// </summary>
    [ExportView("Diagnose_AlarmsLogs_Filter")]
	public partial class Diagnose_AlarmsLogs_Filter : VisiWin.Controls.View
	{
		public Diagnose_AlarmsLogs_Filter()
		{

			this.InitializeComponent();
		}

        private void Btn2_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            btn2.IsSelected = true;
        }

        private void Btn1_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            btn1.IsSelected = true;
        }

        private void View_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                HistoricalAlarmFilterAdapter a = (HistoricalAlarmFilterAdapter)this.DataContext;
                a.SetTimeSpan(a.TimeSpanFilterTypes[a.SelectedTimeSpanFilterTypeIndex].FilterType);
            }
        }
    }
}