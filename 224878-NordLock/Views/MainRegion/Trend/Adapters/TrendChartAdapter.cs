using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisiWin.ApplicationFramework;
using VisiWin.Commands;
using VisiWin.Controls;


namespace HMI
{
    [ExportAdapter("TrendChartAdapter")]
    public class TrendChartAdapter : AdapterBase
    {
        private TrendChart trendChart;


        public TrendChartAdapter()
        {
            this.Resolutions = new ObservableCollection<TimeSpan>()
                               {
                                   TimeSpan.FromMinutes(2),
                                   TimeSpan.FromMinutes(5),
                                   TimeSpan.FromMinutes(15),
                                   TimeSpan.FromMinutes(30),
                                   TimeSpan.FromMinutes(60),
                                   TimeSpan.FromMinutes(5*60),
                               };

            if (ApplicationService.IsInDesignMode)
                return;

            this.LoadChartInstanceCommand = new ActionCommand(this.LoadChartInstanceCommandExecuted);
            //this.ShowReportCommand = new ActionCommand(this.ShowReportCommandExecuted);
        }


        public TrendCurveInformationCollection CurveInformations { get; } = new TrendCurveInformationCollection();

        public ObservableCollection<TimeSpan> Resolutions { get; }


        public ICommand LoadChartInstanceCommand { get; }

        //public ICommand ShowReportCommand { get; }


        private void LoadChartInstanceCommandExecuted(object parameter)
        {
            var chart = parameter as TrendChart;
            if (chart == null)
                return;

            if (this.trendChart == chart)
                return;

            this.trendChart = chart;

            this.trendChart.CurvesContainers.CollectionChanged += CurvesContainers_CollectionChanged;
            this.InitCurveInformations();

            this.trendChart.Markers.CollectionChanged += Markers_CollectionChanged;
            AttachMarkers(this.trendChart.Markers);
        }

        //private void ShowReportCommandExecuted(object parameter)
        //{
        //    var curve = parameter as TrendCurve2;
        //    var trendCurvesContainerDateTime = this.trendChart.CurvesContainers.First(c => c is TrendCurvesContainerDateTime) as TrendCurvesContainerDateTime;
        //    ChartScaleDateTime scaleDateTime = trendCurvesContainerDateTime?.Scale as ChartScaleDateTime;
        //    if (scaleDateTime == null)
        //        return;

        //    var scalePoints = scaleDateTime.ScalePoints;

        //    var minTimePoint = ((ScalePoint<DateTime>)scalePoints.First(p => p.IsMinMaxValue)).Value;
        //    var maxTimePoint = ((ScalePoint<DateTime>)scalePoints.Last(p => p.IsMinMaxValue)).Value;

        //    Func<CancellationToken, Task<ReportConfiguration>> config = (t) => TrendReport.GetReportConfiguration(curve, minTimePoint, maxTimePoint, t);

        //    var adapter = ApplicationService.GetAdapter("ReportViewAdapter") as ReportViewAdapter;
        //    if (adapter != null)
        //    {
        //        adapter.OpenView("MainRegion", config);
        //    }
        //}


        private void CurvesContainers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                AttachCurvesContainers(e.OldItems);

            if (e.NewItems != null)
                DetachCurvesContainers(e.NewItems);

            this.InitCurveInformations();
        }

        private void AttachCurvesContainers(IEnumerable curvesContainers)
        {
            foreach (ICurvesContainer curvesContainer in curvesContainers)
                curvesContainer.CurvesCollectionChanged += CurvesContainer_CurvesCollectionChanged;
        }

        private void DetachCurvesContainers(IEnumerable curvesContainers)
        {
            foreach (ICurvesContainer curvesContainer in curvesContainers)
                curvesContainer.CurvesCollectionChanged -= CurvesContainer_CurvesCollectionChanged;
        }

        private void CurvesContainer_CurvesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.InitCurveInformations();
        }

        public void InitCurveInformations()
        {
            this.CurveInformations.Clear();

            if (this.trendChart == null)
                return;

           var curvesContainer = this.trendChart.CurvesContainers[0];
            
            TrendCurve2 iW = curvesContainer.Curves[0] as TrendCurve2;
            TrendCurve2 sW = curvesContainer.Curves[1] as TrendCurve2;

            this.CurveInformations.Add(iW);
            this.CurveInformations.Add(sW);
            
        }


        private void Markers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                DetachMarkers(e.OldItems);

            if (e.NewItems != null)
                AttachMarkers(e.NewItems);
        }

        private void AttachMarkers(IEnumerable markers)
        {
            foreach (ChartMarker marker in markers)
            {
                marker.MarkerPointsChanged += Marker_MarkerPointsChanged;
                UpdateCurveInformation(marker);
            }
        }

        private void DetachMarkers(IEnumerable markers)
        {
            foreach (ChartMarker marker in markers)
                marker.MarkerPointsChanged -= Marker_MarkerPointsChanged;
        }

        private void Marker_MarkerPointsChanged(object sender, RoutedEventArgs e)
        {
            this.UpdateCurveInformation(sender as ChartMarker);
        }

        private void UpdateCurveInformation(ChartMarker marker)
        {
											   
            if (marker == null)
                return;

            // Nur für den selektierten Marker die Werte anzeigen.
            if (!marker.IsSelected)
                return;

            foreach (var markerPoints in marker.MarkerPoints.GroupBy(mp => mp.Curve))
            {
                ICurve actualCurve = markerPoints.Key;
                var firstMarkerPoint = markerPoints.First();
                if (marker.Orientation == Orientation.Horizontal)
                {
                    this.CurveInformations[actualCurve].MarkedXValues = firstMarkerPoint.XValue.Value.ToString();
                    this.CurveInformations[actualCurve].MarkedYValues = string.Join("; ", markerPoints.OrderBy(mp => mp.YValue.RawValue).Select(mp => mp.YValue.ValueFormatted));
                }
                else
                {
                    this.CurveInformations[actualCurve].MarkedXValues = string.Join("; ", markerPoints.OrderBy(mp => mp.XValue.RawValue).Select(mp => mp.XValue.ValueFormatted));
                    this.CurveInformations[actualCurve].MarkedYValues = firstMarkerPoint.YValue.ValueFormatted;
                }
            }
        }

    }
}
