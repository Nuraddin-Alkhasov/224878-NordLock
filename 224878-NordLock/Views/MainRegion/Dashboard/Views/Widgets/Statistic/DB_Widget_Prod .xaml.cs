using System.ComponentModel.Composition;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using System;
using System.ComponentModel;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;
using System.Data;
using HMI.Module;
using VisiWin.Language;
using System.Threading.Tasks;
using System.Windows;

namespace HMI.Dashboard
{
    /// <summary>
    /// Interaction logic for DashboardWidgetBar.xaml
    /// </summary>
    [ExportDashboardWidget("DB_Widget_Prod", "Dashboard.Text15", "@Dashboard.Text13", 1, 2)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class DB_Widget_Prod : View
    {
      
      

        public DB_Widget_Prod()
        {
            InitializeComponent();

            BackgroundWorker BGW = new BackgroundWorker();
            BGW.DoWork += BGW_DoWork;
            BGW.RunWorkerCompleted += BGW_RunWorkerCompleted;
            BGW.RunWorkerAsync();

           
           
        }
        double[] DataFromSQL = new double[2];
        private void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            DataTable D1 = (new LocalDBAdapter("SELECT SUM(Weight) as Weight " +
                                    "FROM Charges " +
                                    "WHERE Start >= '" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd ") + "00:00:00' AND Start<='" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd ") + "23:59:59'; ")).DB_Output();
            DataTable D2 = (new LocalDBAdapter("SELECT SUM(Weight) as Weight " +
                                                 "FROM Charges " +
                                                 "WHERE Start >= '" + DateTime.Now.ToString("yyyy-MM-dd ") + "00:00:00' AND Start<='" + DateTime.Now.ToString("yyyy-MM-dd ") + "23:59:59'; ")).DB_Output();
            if (D1.Rows.Count == 0)
            { DataFromSQL[0] = 0.0; }
            else 
            {
                if (D1.Rows[0]["Weight"] != System.DBNull.Value)
                {
                    DataFromSQL[0] = Math.Round(Convert.ToDouble(D1.Rows[0]["Weight"]),1);
                }
                else { DataFromSQL[0] = 0.0; }
            }

            if (D2.Rows.Count == 0)
            { DataFromSQL[1] = 0.0; }
            else 
            {
                if (D2.Rows[0]["Weight"] != System.DBNull.Value)
                {
                    DataFromSQL[1] = Math.Round(Convert.ToDouble(D2.Rows[0]["Weight"]),1);
                }
                else { DataFromSQL[1] = 0.0; }
            }
        }

        private void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (pieeee != null) 
            {
                PointLabel = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);
                pieeee.ChartLegend.FontSize = 14;
                pieeee.ChartLegend.Foreground = Brushes.White;
                DataContext = this;
                ILanguageService textService = ApplicationService.GetService<ILanguageService>();

                pieeee.Series[0].Values[0] = DataFromSQL[0];
                pieeee.Series[1].Values[0] = DataFromSQL[1];

                notToday.Title = textService.GetText("@Dashboard.Text26");
                Today.Title = textService.GetText("@Dashboard.Text1");

            }
        }

        public Func<ChartPoint, string> PointLabel { get; set; }

        private void Chart_OnDataClick(object sender, ChartPoint chartpoint)
        {
            var chart = (LiveCharts.Wpf.PieChart)chartpoint.ChartView;

            //clear selected slice.
            foreach (PieSeries series in chart.Series)
                series.PushOut = 0;

            var selectedSeries = (PieSeries)chartpoint.SeriesView;
            selectedSeries.PushOut = 8;
        }
    }
}
