using HMI.UserControls;
using System;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Parameter
{
    /// <summary>
    /// Interaction logic for ButtonsView.xaml
    /// </summary>
    [ExportView("P_M2_Paint_1_5")]
 
    public partial class P_M2_Paint_1_5 : View
    {
        public P_M2_Paint_1_5()
        {
            this.InitializeComponent();
        }

        private void P_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Task obTask = Task.Run(async () =>
            {
                for (int i = 1; i <= 5; i++)
                {
                  
                    await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                    {
                        PaintType PT = new PaintType()
                        {
                            Header = "@Parameter.Lacktyp.Text"+ (33+i).ToString(),
                            Name = "NL.PLC.Blocks.50 HMI.00 Allgemein.Lacktyp Namen.Lacktyp Name[" + i + "]",
                            paintType = "NL.PLC.Blocks.50 HMI.00 Allgemein.Lacktyp Namen.Base / Top Coat[" + i + "]",
                            IsSolvent = "NL.PLC.Blocks.50 HMI.00 Allgemein.Lacktyp Namen.Lösemittel im Lack[" + i + "]",
                            WatchDog = "NL.PLC.Blocks.50 HMI.00 Allgemein.Lacktyp Namen.Niveau Überwachung[" + i + "]",
                            MaxCoating = "NL.PLC.Blocks.50 HMI.00 Allgemein.Lacktyp Namen.Soll BS Zyklen je Korb[" + i + "]",
                            Pump = "NL.PLC.Blocks.50 HMI.00 Allgemein.Lacktyp Namen.Pumpe[" + i + "].Ein / Aus",
                            PumpOn = "NL.PLC.Blocks.50 HMI.00 Allgemein.Lacktyp Namen.Pumpe[" + i + "].Ein - Zeit",
                            PumpOff = "NL.PLC.Blocks.50 HMI.00 Allgemein.Lacktyp Namen.Pumpe[" + i + "].Aus - Zeit",
                            OfenUL = "NL.PLC.Blocks.50 HMI.00 Allgemein.Lacktyp Namen.Ofentemp BS Start[" + i + "].Diff OG",
                            OfenProcess = "NL.PLC.Blocks.50 HMI.00 Allgemein.Lacktyp Namen.Ofentemp BS Start[" + i + "].Prozess",
                            OfenLL = "NL.PLC.Blocks.50 HMI.00 Allgemein.Lacktyp Namen.Ofentemp BS Start[" + i + "].Diff UG",
                            CoolingZoneUL = "NL.PLC.Blocks.50 HMI.00 Allgemein.Lacktyp Namen.Kühlung[" + i + "].Diff OG",
                            CoolingZoneProcess = "NL.PLC.Blocks.50 HMI.00 Allgemein.Lacktyp Namen.Kühlung[" + i + "].Prozess",
                            CoolingZoneLL = "NL.PLC.Blocks.50 HMI.00 Allgemein.Lacktyp Namen.Kühlung[" + i + "].Diff UG"
                        };
                        P.Children.Add(PT);
                    });
                    await Task.Delay(500);
                }
           }); 
        }

        private void P_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Task obTask = Task.Run(async () =>
            {
                await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                {
                    P.Children.Clear();
                });
            });

        }
    }
}