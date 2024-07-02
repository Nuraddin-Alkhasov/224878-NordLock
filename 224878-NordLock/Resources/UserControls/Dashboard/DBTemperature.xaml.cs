using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;
using VisiWin.Language;
using Color = System.Windows.Media.Color;

namespace HMI.UserControls
{
    public partial class DBTemperature : UserControl
    {
        public DBTemperature()
        {
            InitializeComponent();
        }


        #region - - - Fields - - -

        IVariableService VS = ApplicationService.GetService<IVariableService>();
        IVariable T;
        public string VN_Temperature { set { T = VS.GetVariable(value); } }
        public string StationName { get; set; }

        DispatcherTimer getTemp;
        public List<string> Labels { get; set; }
        public SeriesCollection LastHourSeries { get; set; }
        public double TempRange { get; set; }


        #endregion

        #region - - - Methods - - -
        public void DoWork() 
        {
            if (T != null)
            {
                Header.LocalizableText = StationName;
                ILanguageService textService = ApplicationService.GetService<ILanguageService>();
                ActualTemp.Text = Math.Round(Convert.ToDouble(VS.GetValue(T.Name, true)), 1).ToString();

                LastHourSeries = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = textService.GetText(StationName),
                        AreaLimit = -10,
                        StrokeThickness = 5,
                        PointGeometrySize = 0,
                        LineSmoothness = 1,
                        Stroke = new SolidColorBrush(Color.FromRgb(255,188,73)),
                        Values = new ChartValues<double>()
                    }
                };
                Labels = new List<string>();

                DataContext = this;

                getTemp = new DispatcherTimer
                {
                    Interval = new TimeSpan(0, 0, 0, 0, 100)
                };

                getTemp.Tick += new System.EventHandler(getTempTick);
                getTemp.Start();
            }
        }

        private void getTempTick(object sender, EventArgs e)
        {
            Labels.Add(DateTime.Now.ToLongTimeString());
            LastHourSeries[0].Values.Add(Math.Round(Convert.ToDouble(T.Value), 1));

            ActualTemp.Text = LastHourSeries[0].Values[LastHourSeries[0].Values.Count - 1].ToString();

            ObservableCollection<double> temp = CopyArray(LastHourSeries[0].Values);
            Double maxValue = Convert.ToDouble((from x in temp select x).Max());
            Double minValue = Convert.ToDouble((from x in temp select x).Min());

            if (Labels.Count == 21)
            {
                if (Convert.ToDouble(LastHourSeries[0].Values[0]) == minValue)
                {
                    Labels.RemoveAt(1);
                    LastHourSeries[0].Values.RemoveAt(1);
                }
                else
                {
                    Labels.RemoveAt(0);
                    LastHourSeries[0].Values.RemoveAt(0);
                }
            }

            oy.MaxValue = Math.Ceiling(maxValue + maxValue * TempRange) == 0 ? 50 : Math.Ceiling(maxValue + maxValue * TempRange);
            oy.MinValue = Math.Floor(minValue - maxValue * TempRange < 0 ? 0 : minValue - maxValue * TempRange);
            oySeparator.Step = Math.Ceiling((oy.MaxValue - oy.MinValue) / 10);
            if (Labels.Count < 21)
            {
                getTemp.Stop();
                getTemp = new DispatcherTimer
                {
                    Interval = new TimeSpan(0, 0, 0, 0, Labels.Count * 100)
                };
                getTemp.Tick += new System.EventHandler(getTempTick);
                getTemp.Start();
            }

            if (this.IsVisible == false)
                getTemp.Stop();

        }
        private ObservableCollection<double> CopyArray(IChartValues array)
        {
            ObservableCollection<double> returnValue = new ObservableCollection<double>();

            foreach (object o in array)
            {
                returnValue.Add(Convert.ToDouble(o));
            }

            return returnValue;
        }

        #endregion
    }
}
