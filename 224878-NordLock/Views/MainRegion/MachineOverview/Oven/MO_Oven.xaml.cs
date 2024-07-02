using HMI.UserControls;
using HMI.Views.MessageBoxRegion;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;


namespace HMI.Views.MainRegion.MachineOverview
{

    [ExportView("MO_Oven")]
    public partial class MO_Oven : VisiWin.Controls.View
    {

        public MO_Oven()
        {
            InitializeComponent();


        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (new SP
            {
                Module = 3,
                M1_Station = 0,
                M2_Station = 0,
                M3_Station = 9,
                M4_Station = 0,
                OvenTray = 0,
                TB_Shelve = 0,
                TB_Level = 0,
                Header = "@Status.Text35",
                Type = "Basket"
            }).Execute();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            (new SP
            {
                Module = 3,
                M1_Station = 0,
                M2_Station = 0,
                M3_Station = 10,
                M4_Station = 0,
                OvenTray = 0,
                TB_Shelve = 0,
                TB_Level = 0,
                Header = "@Status.Text36",
                Type = "-"
            }).Execute();
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            (new SP
            {
                Module = 3,
                M1_Station = 0,
                M2_Station = 0,
                M3_Station = 11,
                M4_Station = 0,
                OvenTray = 0,
                TB_Shelve = 0,
                TB_Level = 0,
                Header = "@Status.Text39",
                Type = "-"
            }).Execute();
        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("MainRegion", "TrendChartView",
              new TrendData()
              {
                  ArchiveName = "PreheatingZone",
                  TrendName_1 = "Trend1",
                  CurveTag_1 = "@TrendSystem.Text2",
                  TrendName_2 = "Trend2",
                  CurveTag_2 = "@TrendSystem.Text3",
                  Header = "@TrendSystem.Text7",
                  Min = 0,
                  Max = 150,
                  BackViewName = "MO_Oven"
              }
              );
        }
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("MainRegion", "TrendChartView",
              new TrendData()
              {
                  ArchiveName = "Dryer",
                  TrendName_1 = "Trend1",
                  CurveTag_1 = "@TrendSystem.Text2",
                  TrendName_2 = "Trend2",
                  CurveTag_2 = "@TrendSystem.Text3",
                  Header = "@TrendSystem.Text8",
                  Min = 0,
                  Max = 300,
                  BackViewName = "MO_Oven"
              }
              );
        }

        double margin = 49;
        int i = 0;
        OvenTray OT;
        private void Trays_Loaded(object sender, RoutedEventArgs e)
        {
            BackgroundWorker BGW = new BackgroundWorker();
            BGW.DoWork += BGW_DoWorkAsync;
            BGW.RunWorkerCompleted += BGW_RunWorkerCompleted;
            BGW.RunWorkerAsync(new OvenTrayPosition(i, margin));
        }

        private void Trays_Unloaded(object sender, RoutedEventArgs e)
        {
            Task obTask = Task.Run(async () =>
            {
                await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                {
                    for (int i = Trays.Children.Count - 1; i >= 0; i--)
                    {
                        Trays.Children.RemoveAt(i);
                    }
                });
            });

        }
        private OvenTray GetTray(int i, double margin)
        {
            OvenTray temp = new OvenTray()
            {
                IsTray = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Ofen.Tablett[" + i + "].Status.Tablett.Belegt",
                IsMaterial = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Ofen.Tablett[" + i + "].Status.Charge.Material vorhanden",
                ActualCoatingLayer = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Ofen.Tablett[" + i + "].Status.Charge.Beschichtungen.Ist",
                SetCoatingLayer = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Ofen.Tablett[" + i + "].Status.Charge.Beschichtungen.Soll",
                IsDischarge = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Ofen.Tablett[" + i + "].Status.Tablett.Function.Discharge",
                Module = 4,
                M4_Station = 3,
                _OvenTray = i,
                Type = "Tray",
                Header = "@Status.Text" + (40 + i).ToString(),
            };
            temp.Margin = new Thickness(margin, 497, 0, 0);
            return temp;
        }

        private void BGW_DoWorkAsync(object sender, DoWorkEventArgs e)
        {
            OvenTrayPosition arg = (OvenTrayPosition)e.Argument;
            Dispatcher.InvokeAsync(delegate
            {
                OT = GetTray(arg.i, arg.margin);
                OT.HorizontalAlignment = HorizontalAlignment.Left;
                OT.VerticalAlignment = VerticalAlignment.Top;
            });
        }

        private void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (OT != null)
            {
                Trays.Children.Add(OT);
                OT = null;
            }
            else
            {
                throw new NotImplementedException();
            }

            if (i <= 47 && this.IsVisible)
            {

                BackgroundWorker BGW = new BackgroundWorker();
                BGW.DoWork += BGW_DoWorkAsync;
                BGW.RunWorkerCompleted += BGW_RunWorkerCompleted;
                BGW.RunWorkerAsync(new OvenTrayPosition(i += 1, margin += 36));
            }
            else
            {
                i = 0;
                margin = 49;
            }

        }

        private void emptyoven_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxView.Show("@MessageBox.Text3", "@MainView.Text70", MessageBoxButton.YesNo, MessageBoxResult.No, MessageBoxIcon.Question) == MessageBoxResult.Yes)
            {
                Task taskA = Task.Run(() =>
                {
                    ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Produktionsmodus.Leerfahren.Manuell Start", true);
                });
                taskA.ContinueWith(async x =>
                {
                    await Task.Delay(800);
                    ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Produktionsmodus.Leerfahren.Manuell Start", false);
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
            }


        }


        public class OvenTrayPosition
        {
            public OvenTrayPosition(int _i, double _margin)
            {
                i = _i;
                margin = _margin;
            }
            public int i { set; get; }
            public double margin { set; get; }

        }
    }
}




