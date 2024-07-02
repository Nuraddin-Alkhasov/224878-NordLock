using HMI.Views.DialogRegion;
using HMI.Views.MainRegion;
using System;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Trend;
using VisiWin.UserManagement;

namespace HMI
{
    [ExportView("TrendChartView")]
    public partial class TrendChartView : VisiWin.Controls.View
    {
        public TrendChartView()
        {
            InitializeComponent();
        }
        public TrendData Trend;
        private void view_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                Trend = (TrendData)ApplicationService.ObjectStore.GetValue("TrendChartView_KEY");
                
                curve1.ArchiveName = curve2.ArchiveName = Trend.ArchiveName;
                curve1.TrendName = Trend.TrendName_1;
                curve1.Tag = Trend.CurveTag_1;
                curve2.TrendName = Trend.TrendName_2;
                ((TrendChartAdapter)this.DataContext).InitCurveInformations();
                curve2.Tag = Trend.CurveTag_2;
                head.LocalizableText = Trend.Header;

                int maxIfNull = 100;

                switch (Trend.ArchiveName) 
                {
                    case "Paint": maxIfNull = 50; break;
                    case "PreheatingZone": maxIfNull = 150; break;
                    case "Dryer": maxIfNull = 300; break;
                    case "CoolingZone": maxIfNull = 50; break;
                }

                ITrendService trendService = ApplicationService.GetService<ITrendService>();
                float a = (float)ApplicationService.GetVariableValue((trendService.GetArchive(Trend.ArchiveName)).GetTrend(Trend.TrendName_1).GetDefinition().TrendVariableName);
                float b = (float)ApplicationService.GetVariableValue((trendService.GetArchive(Trend.ArchiveName)).GetTrend(Trend.TrendName_2).GetDefinition().TrendVariableName);
                if (a < b)
                {
                    min.Value=curve1.MinValue = curve2.MinValue = Math.Floor(a - b * 0.05 < 0 ? 0 : a - b * 0.05);
                    max.Value=curve1.MaxValue = curve2.MaxValue = Math.Ceiling(b + b * 0.05) == 0 ? maxIfNull : Math.Ceiling(b + b * 0.05);

                }
                else
                {
                    min.Value = curve1.MinValue = curve2.MinValue = Math.Floor(b - a * 0.05 < 0 ? 0 : b - a * 0.05);
                    max.Value = curve1.MaxValue = curve2.MaxValue = Math.Ceiling(a + a * 0.05) == 0 ? maxIfNull : Math.Ceiling(a + a * 0.05);
                }

              //  curve2.ScaleSpacing = curve1.ScaleSpacing = Math.Ceiling((curve1.MaxValue - curve1.MinValue) / 10);
                
                ApplicationService.ObjectStore.Remove("TrendChartView_KEY");
            }
        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            ApplicationService.SetView("MainRegion", Trend.BackViewName);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IUserManagementService userService = ApplicationService.GetService<IUserManagementService>();
            if (userService.CurrentUser != null && userService.CurrentUser.RightNames.Contains("Trend"))
            {
                DialogView.Show("ExportView", "@TrendSystem.Views.Text8", DialogButton.Close);
            }

        }

        private void curve1_ScaleLayoutChanged(object sender, VisiWin.Controls.ScaleLayoutChangedEventArgs e)
        {
            if (curve1 != null && curve2 != null)
            {
                curve2.MinValue = curve1.MinValue;
                curve2.MaxValue = curve1.MaxValue;
            }
        }
    }
}
