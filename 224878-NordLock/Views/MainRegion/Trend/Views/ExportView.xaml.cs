using HMI.Adapter;
using System;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Views.MainRegion
{
    /// <summary>
    /// Interaction logic for ExportView.xaml
    /// </summary>
    [ExportView("ExportView")]
    public partial class ExportView : View
    {
        public ExportView()
        {
            this.InitializeComponent();
        }

        private void ExportView_View_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                IRegionService iRS = ApplicationService.GetService<IRegionService>();
                TrendChartView TCV = (TrendChartView)iRS.GetView("TrendChartView");

                ((TrendExportAdapter)this.DataContext).SelectedArchiveName = TCV.Trend.ArchiveName;
                   
            }
        }
    }
}