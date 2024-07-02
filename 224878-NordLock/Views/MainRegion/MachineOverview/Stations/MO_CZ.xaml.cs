using HMI.UserControls;
using HMI.Views.DialogRegion;
using System;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Views.MainRegion.MachineOverview
{

    [ExportView("MO_CZ")]
    public partial class MO_CZ : VisiWin.Controls.View
    {

        public MO_CZ()
        {
            InitializeComponent();
        }

        private CZTray GetTray(int i, double margin)
        {
            CZTray temp = new CZTray()
            {
                IsTray = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Kühlzone.Tablett[" + i + "].Status.Tablett.Belegt",
                IsMaterial = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Kühlzone.Tablett[" + i + "].Status.Charge.Material vorhanden",
                IsDischarge= "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Kühlzone.Tablett[" + i + "].Status.Tablett.Function.Discharge",
                ActualCoatingLayer = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Kühlzone.Tablett[" + i + "].Status.Charge.Beschichtungen.Ist",
                SetCoatingLayer = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Kühlzone.Tablett[" + i + "].Status.Charge.Beschichtungen.Soll",
                Module = 4,
                M4_Station = 5,
                _CZTray = i,
                Type = "Tray",
                Header = "@Status.Text" + (89 + i).ToString(),
            };
            temp.Margin = new Thickness(margin, 224, 0, 0);
            return temp;
        }

        private void Trays_Loaded(object sender, RoutedEventArgs e)
        {
            Task obTaskx = Task.Run(async () =>
            {
                double margin = 1660;
                for (int i = 1; i <= 14; i++)
                {
                    margin = margin - 105;

                    await Task.Delay(50);
                    DoTask(i, margin);
                }
            });

        }

        private void DoTask(int i, double margin)
        {
            Task obTask = Task.Run(() =>
            {

                Application.Current.Dispatcher.InvokeAsync((Action)delegate
                {

                    CZTray temp = GetTray(i, margin);
                    margin = temp.Margin.Left;
                    temp.HorizontalAlignment = HorizontalAlignment.Left;
                    temp.VerticalAlignment = VerticalAlignment.Top;
                    Trays.Children.Add(temp);

                });
            });
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("MainRegion", "TrendChartView",
             new TrendData()
             {
                 ArchiveName = "CoolingZone",
                 TrendName_1 = "Trend1",
                 CurveTag_1 = "@TrendSystem.Text2",
                 TrendName_2 = "Trend2",
                 CurveTag_2 = "@TrendSystem.Text3",
                 Header = "@TrendSystem.Text9",
                 Min = 0,
                 Max = 50,
                 BackViewName = "MO_CZ"
             }
             );
        }
    }
}



