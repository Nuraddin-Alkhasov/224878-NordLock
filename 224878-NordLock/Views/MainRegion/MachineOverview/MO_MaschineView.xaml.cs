using HMI.UserControls;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Views.MainRegion.MachineOverview
{

    [ExportView("MO_MaschineView")]
    public partial class MO_MaschineView : VisiWin.Controls.View
    {

        public MO_MaschineView()
        {
            InitializeComponent();
        }

        
        
        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            BackgroundWorker BGW1 = new BackgroundWorker();
            BGW1.DoWork += BGW1_DoWork;
            BGW1.RunWorkerCompleted += BGW1_RunWorkerCompleted;
            BGW1.RunWorkerAsync();          
        }
        #region - - - Dryer - - -

        MV_Dryer Dryer;
        private void BGW1_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.InvokeAsync(delegate
            {
                Dryer = new MV_Dryer
                {
                    PHZTemperature = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.01 Vorzone.DB Vorzone HMI.Istwerte.Temperatur",
                    PHZ_RPM= "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.01 Vorzone.DB Vorzone HMI.Istwerte.Drehzahl_Umluft",
                    PHZ_Nachlauf= "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.01 Vorzone.DB Vorzone HMI.Istwerte.Nachlauf Aktiv",
                    OvenStatus= "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.00 Allgemein.DB Heizung / Ventilatoren Allgemein HMI.Actual value.Heizung Ventilatoren Status",
                    OvenTemperature = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.03 Trockner.DB Trockner HMI.Istwerte.Temperatur",
                    O_RPM = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.03 Trockner.DB Trockner HMI.Istwerte.Drehzahl Umluft",
                    O_Nachlauf = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.03 Trockner.DB Trockner HMI.Istwerte.Nachlauf Aktiv",
                    OvenEmptyAuto= "NLM4.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Produktionsmodus.Leerfahren.Automatisch Status",
                    OvenEmptyManual= "NLM4.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Produktionsmodus.Leerfahren.Manuell Status",
                    OvenDoubleCycle= "NLM4.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Produktionsmodus.Doppeltakt.Status",
                    O_Purge= "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.03 Trockner.DB Trockner HMI.Istwerte.Lüfter_Abluft.Spühlzeit aktiv",
                    PHZ_Purge= "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.01 Vorzone.DB Vorzone HMI.Istwerte.Lüfter Abluft.Spühlzeit aktiv",
                    OCycleMin= "NLM4.PLC.Blocks.4 Modul 4.03 Trockner Transport.02 Kettentransport.DB Trockner Transport HMI.Istwert.Mindest Taktzeit.Minute",
                    OCycleSec= "NLM4.PLC.Blocks.4 Modul 4.03 Trockner Transport.02 Kettentransport.DB Trockner Transport HMI.Istwert.Mindest Taktzeit.Sekunde",
                    Margin = new Thickness(440, 386, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                };
            });
        }
        private void BGW1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LayoutRoot.Children.Add(Dryer);
            BackgroundWorker BGW2 = new BackgroundWorker();
            BGW2.DoWork += BGW2_DoWork;
            BGW2.RunWorkerCompleted += BGW2_RunWorkerCompleted;
            BGW2.RunWorkerAsync();
        }

        #endregion
        #region - - - HVT - - -

        MV_HVT HVT;
        private void BGW2_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.InvokeAsync(delegate
            {
                HVT = new MV_HVT()
                {
                    HVTPosition = "NLM4.PLC.Blocks.4 Modul 4.02 HVT.Fahrwagen.02 Fahrantrieb.DB HVT Fahrwagen HMI.Actual value.Aktuelle Position in mm"
                };
                HVT.Margin = new Thickness(360, 477, 0, 0);
                HVT.HorizontalAlignment = HorizontalAlignment.Left;
                HVT.VerticalAlignment = VerticalAlignment.Top;
            });
        }
        private void BGW2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LayoutRoot.Children.Add(HVT);
            BackgroundWorker BGW3 = new BackgroundWorker();
            BGW3.DoWork += BGW3_DoWork;
            BGW3.RunWorkerCompleted += BGW3_RunWorkerCompleted;
            BGW3.RunWorkerAsync();
        }



        #endregion
        #region - - - KZ - - -

        MV_KZ KZ;
        private void BGW3_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.InvokeAsync(delegate
            {
                KZ = new MV_KZ()
                {
                    CZTemperature= "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.04 Kühlzone.DB Kühlzone HMI.Istwerte.Temperatur",
                    CZUmluft= "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.04 Kühlzone.DB Kühlzone HMI.Istwerte.Drehzahl Umluft",
                    DoorStatus = "NLM4.PLC.Blocks.4 Modul 4.05 Kühlzone Transport.02 Kettentransport.DB Kühlzone Transport HMI.Istwert.Wartungstüre zu"
                };
                KZ.Margin = new Thickness(1331, 553, 0, 0);
                KZ.HorizontalAlignment = HorizontalAlignment.Left;
                KZ.VerticalAlignment = VerticalAlignment.Top;
            });
        }
        private void BGW3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LayoutRoot.Children.Add(KZ);
            BackgroundWorker BGW4 = new BackgroundWorker();
            BGW4.DoWork += BGW4_DoWork;
            BGW4.RunWorkerCompleted += BGW4_RunWorkerCompleted;
            BGW4.RunWorkerAsync();
        }

        #endregion
        #region - - - HNT - - -

        MV_HNT HNT;
        private void BGW4_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.InvokeAsync(delegate
            {
                HNT = new MV_HNT()
                {
                    HNTPosition = "NLM4.PLC.Blocks.4 Modul 4.04 HNT.Fahrwagen.02 Fahrantrieb.DB HNT Fahrwagen HMI.Actual value.Aktuelle Position in mm"
                };
                HNT.Margin = new Thickness(1710, 479, 0, 0);
                HNT.HorizontalAlignment = HorizontalAlignment.Left;
                HNT.VerticalAlignment = VerticalAlignment.Top;
            });
        }

        private void BGW4_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LayoutRoot.Children.Add(HNT);
            BackgroundWorker BGW5 = new BackgroundWorker();
            BGW5.DoWork += BGW5_DoWork;
            BGW5.RunWorkerCompleted += BGW5_RunWorkerCompleted;
            BGW5.RunWorkerAsync();
        }



        #endregion
        #region - - - Trays KZ[1] & Oven[48]- - -
        MVTrayVertical TrayVertical1;
        MVTrayVertical TrayVertical2;
        private void BGW5_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.InvokeAsync(delegate
            {
                TrayVertical1 = new MVTrayVertical()
                {
                    IsTray = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Kühlzone.Tablett[1].Status.Tablett.Belegt",
                    IsMaterial = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Kühlzone.Tablett[1].Status.Charge.Material vorhanden",
                    ActualCoatingLayer = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Kühlzone.Tablett[1].Status.Charge.Beschichtungen.Ist",
                    SetCoatingLayer = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Kühlzone.Tablett[1].Status.Charge.Beschichtungen.Soll",
                    IsDischarge = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Kühlzone.Tablett[1].Status.Tablett.Function.Discharge",
                    Module = 4,
                    M4_Station = 5,
                    CZTray = 1,
                    Header = "@Status.Text90",
                    Type = "Tray"
                };
                TrayVertical1.Margin = new Thickness(1711, 638, 0, 0);
                TrayVertical1.HorizontalAlignment = HorizontalAlignment.Left;
                TrayVertical1.VerticalAlignment = VerticalAlignment.Top;
            });

            Dispatcher.InvokeAsync(delegate
            {
                TrayVertical2 = new MVTrayVertical()
                {
                    IsTray = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Ofen.Tablett[48].Status.Tablett.Belegt",
                    IsMaterial = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Ofen.Tablett[48].Status.Charge.Material vorhanden",
                    ActualCoatingLayer = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Ofen.Tablett[48].Status.Charge.Beschichtungen.Ist",
                    SetCoatingLayer = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Ofen.Tablett[48].Status.Charge.Beschichtungen.Soll",
                    IsDischarge = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Ofen.Tablett[48].Status.Tablett.Function.Discharge",

                    Module = 4,
                    M4_Station = 3,
                    OvenTray = 48,
                    Header = "@Status.Text88",
                    Type = "Tray"
                };
                TrayVertical2.Margin = new Thickness(1713, 482, 0, 0);
                TrayVertical2.HorizontalAlignment = HorizontalAlignment.Left;
                TrayVertical2.VerticalAlignment = VerticalAlignment.Top;
            });
        }

        private void BGW5_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LayoutRoot.Children.Add(TrayVertical1);
            LayoutRoot.Children.Add(TrayVertical2);
            BackgroundWorker BGW6 = new BackgroundWorker();
            BGW6.DoWork += BGW6_DoWork;
            BGW6.RunWorkerCompleted += BGW6_RunWorkerCompleted;
            BGW6.RunWorkerAsync();
        }


        #endregion
        #region - - - M4 Mani - - -
        MV_M4Mani M4Mani;
        private void BGW6_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.InvokeAsync(delegate
            {
                M4Mani = new MV_M4Mani()
                {
                    ManipulatorStatus = "NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.00 Allgemein.DB Mani04 Allgemein HMI.PC.Status Bildanzeige Mani",
                    ManipulatorPosition = "NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.01 Mani Fahr.DB Mani04 Fahr HMI.Actual value.Aktuelle Position in mm"
                };
                M4Mani.Margin = new Thickness(354, 543, 0, 0);
                M4Mani.HorizontalAlignment = HorizontalAlignment.Left;
                M4Mani.VerticalAlignment = VerticalAlignment.Top;
            });
        }

        private void BGW6_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LayoutRoot.Children.Add(M4Mani);
            BackgroundWorker BGW7 = new BackgroundWorker();
            BGW7.DoWork += BGW7_DoWork;
            BGW7.RunWorkerCompleted += BGW7_RunWorkerCompleted;
            BGW7.RunWorkerAsync();
        }

        #endregion
        #region - - - M4 Mani Tray - - -
        MVTrayVertical MVTrayVertical3;
        private void BGW7_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.InvokeAsync(delegate
            {
                MVTrayVertical3 = new MVTrayVertical()
                {
                    IsTray = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Kühlzone.Tablett[14].Status.Tablett.Belegt",
                    IsMaterial = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Kühlzone.Tablett[14].Status.Charge.Material vorhanden",
                    ActualCoatingLayer = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Kühlzone.Tablett[14].Status.Charge.Beschichtungen.Ist",
                    SetCoatingLayer = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Kühlzone.Tablett[14].Status.Charge.Beschichtungen.Soll",
                    Module = 4,
                    M4_Station = 5,
                    CZTray = 14,
                    Header = "@Status.Text103",
                    Type = "Tray"
                };
                MVTrayVertical3.Margin = new Thickness(1337, 638, 0, 0);
                MVTrayVertical3.HorizontalAlignment = HorizontalAlignment.Left;
                MVTrayVertical3.VerticalAlignment = VerticalAlignment.Top;
            });

        }

        private void BGW7_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LayoutRoot.Children.Add(MVTrayVertical3);
            BackgroundWorker BGW8 = new BackgroundWorker();
            BGW8.DoWork += BGW8_DoWork;
            BGW8.RunWorkerCompleted += BGW8_RunWorkerCompleted;
            BGW8.RunWorkerAsync();
        }

        #endregion
        #region - - - UnloadStation - - -
        MV_UnloadStation UnloadStation;
        private void BGW8_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.InvokeAsync(delegate
            {
                UnloadStation = new MV_UnloadStation()
                {
                    IsBox = "NLM4.PLC.Blocks.4 Modul 4.09 Entladen.01 Box.DB Entladen Box PD.Status.Box.Belegt",
                    IsMaterial = "NLM4.PLC.Blocks.4 Modul 4.09 Entladen.01 Box.DB Entladen Box PD.Status.Charge.Material vorhanden",
                    Weight = "NLM4.PLC.Blocks.4 Modul 4.09 Entladen.02 Entladen Hub.DB Entladen Hub HMI.Actual value.Gewicht"
                };
                UnloadStation.Margin = new Thickness(706, 820, 0, 0);
                UnloadStation.HorizontalAlignment = HorizontalAlignment.Left;
                UnloadStation.VerticalAlignment = VerticalAlignment.Top;
            });
        }

        private void BGW8_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LayoutRoot.Children.Add(UnloadStation);
            BackgroundWorker BGW9 = new BackgroundWorker();
            BGW9.DoWork += BGW9_DoWork;
            BGW9.RunWorkerCompleted += BGW9_RunWorkerCompleted;
            BGW9.RunWorkerAsync();
        }


        #endregion
        #region - - - Shalves - - -
        MV_Shalves Shalves;
        private void BGW9_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.InvokeAsync(delegate
            {
                Shalves = new MV_Shalves()
                {
                    QSDoor1Status = "NLM4.PLC.Blocks.4 Modul 4.10 Qualität.00 Allgemein.DB Qualität Allgemein HMI.Actual value.Status Türe 2 unten",
                    QSDoor2Status = "NLM4.PLC.Blocks.4 Modul 4.10 Qualität.00 Allgemein.DB Qualität Allgemein HMI.Actual value.Status Türe 1 oben",
                    QualityStatus= "NLM4.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Produktionsmodus.Qualitätsprüfung.Status",
                    TBReturnStatus= "NLM4.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Produktionsmodus.Lager Rückführen.Status"
                };
                Shalves.Margin = new Thickness(461, 701, 0, 0);
                Shalves.HorizontalAlignment = HorizontalAlignment.Left;
                Shalves.VerticalAlignment = VerticalAlignment.Top;
            });
        }

        private void BGW9_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LayoutRoot.Children.Add(Shalves);
            BackgroundWorker BGW10 = new BackgroundWorker();
            BGW10.DoWork += BGW10_DoWork;
            BGW10.RunWorkerCompleted += BGW10_RunWorkerCompleted;
            BGW10.RunWorkerAsync();
        }


        #endregion
        #region - - - Shalves NavigationButton  - - -
        NavigationButton NB1;
        NavigationButton NB2;
        private void BGW10_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.InvokeAsync(delegate
            {
                NB1 = new NavigationButton
                {
                    Margin = new Thickness(465, 535, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Height = 280,
                    Width = 648,
                    RegionName = "MainRegion",
                    ViewName = "MO_TrayBuffer",
                    Style = (Style)FindResource("B_Station")
                };
            });

            Dispatcher.InvokeAsync(delegate
            {
                NB2 = new NavigationButton
                {
                    Margin = new Thickness(461, 0, 0, 183),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Height = 81,
                    Width = 124,
                    RegionName = "MainRegion",
                    ViewName = "MO_QS",
                    Style = (Style)FindResource("B_Station")
                };
            });
        }

        private void BGW10_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LayoutRoot.Children.Add(NB1);
            LayoutRoot.Children.Add(NB2);
            BackgroundWorker BGW11 = new BackgroundWorker();
            BGW11.DoWork += BGW11_DoWork;
            BGW11.RunWorkerCompleted += BGW11_RunWorkerCompleted;
            BGW11.RunWorkerAsync();
        }
        #endregion
        #region - - - KR - - -
        MV_KR KR;
        private void BGW11_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.InvokeAsync(delegate
            {
                KR = new MV_KR
                {
                    Margin = new Thickness(61, 344, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                };
            });
        }

        private void BGW11_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LayoutRoot.Children.Add(KR);
            BackgroundWorker BGW12 = new BackgroundWorker();
            BGW12.DoWork += BGW12_DoWork;
            BGW12.RunWorkerCompleted += BGW12_RunWorkerCompleted;
            BGW12.RunWorkerAsync();
        }
        #endregion
        #region - - - KA - - -
        MV_KA KA;
        private void BGW12_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.InvokeAsync(delegate
            {
                KA = new MV_KA
                {
                    Margin = new Thickness(336, 423, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                };
            });
        }

        private void BGW12_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LayoutRoot.Children.Add(KA);
            BackgroundWorker BGW13 = new BackgroundWorker();
            BGW13.DoWork += BGW13_DoWork;
            BGW13.RunWorkerCompleted += BGW13_RunWorkerCompleted;
            BGW13.RunWorkerAsync();
        }
        #endregion
        #region - - - KBDMani - - -
        MV_KBDMani KBDMani;
        private void BGW13_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.InvokeAsync(delegate
            {
                KBDMani = new MV_KBDMani()
                {
                    ManipulatorPosition = "NL.PLC.Blocks.3 Modul 3.08 Manipulator.00 Allgemein.DB Mani Allgemein HMI.Actual value.Status Bildanzeige Mani"
                };
                KBDMani.Margin = new Thickness(134, 390, 0, 0);
                KBDMani.Height = 310;
                KBDMani.HorizontalAlignment = HorizontalAlignment.Left;
                KBDMani.VerticalAlignment = VerticalAlignment.Top;
            });
        }

        private void BGW13_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LayoutRoot.Children.Add(KBDMani);
            BackgroundWorker BGW14 = new BackgroundWorker();
            BGW14.DoWork += BGW14_DoWork;
            BGW14.RunWorkerCompleted += BGW14_RunWorkerCompleted;
            BGW14.RunWorkerAsync();
        }
        #endregion
        #region - - - KA Basket 1&2 - - -
        MVBasket MVBasket1;
        MVBasket MVBasket2;
        private void BGW14_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.InvokeAsync(delegate
            {
                MVBasket1 = new MVBasket()
                {
                    IsBasket = "NL.PLC.Blocks.3 Modul 3.12 Korb Auskippen.00 Allgemein.DB Korb Auskippen PD.Status.Korb.Belegt links",
                    IsMaterial = "NL.PLC.Blocks.3 Modul 3.12 Korb Auskippen.00 Allgemein.DB Korb Auskippen PD.Status.Charge.Material vorhanden",
                    ActualCoatingLayer = "NL.PLC.Blocks.3 Modul 3.12 Korb Auskippen.00 Allgemein.DB Korb Auskippen PD.Status.Charge.Beschichtungen.Ist"
                };
                MVBasket1.Margin = new Thickness(370, 444, 0, 0);
                MVBasket1.Width = 38;
                MVBasket1.Height = 46;
                MVBasket1.HorizontalAlignment = HorizontalAlignment.Left;
                MVBasket1.VerticalAlignment = VerticalAlignment.Top;
            });

            Dispatcher.InvokeAsync(delegate
            {
                MVBasket2 = new MVBasket()
                {
                    IsBasket = "NL.PLC.Blocks.3 Modul 3.12 Korb Auskippen.00 Allgemein.DB Korb Auskippen PD.Status.Korb.Belegt rechts",
                    IsMaterial = "NL.PLC.Blocks.3 Modul 3.12 Korb Auskippen.00 Allgemein.DB Korb Auskippen PD.Status.Charge.Material vorhanden",
                    ActualCoatingLayer = "NL.PLC.Blocks.3 Modul 3.12 Korb Auskippen.00 Allgemein.DB Korb Auskippen PD.Status.Charge.Beschichtungen.Ist",
                    IsDischarge = "NL.PLC.Blocks.3 Modul 3.12 Korb Auskippen.00 Allgemein.DB Korb Auskippen PD.Status.Korb.Ausschleusen",
                    IsClean = "NL.PLC.Blocks.3 Modul 3.12 Korb Auskippen.00 Allgemein.DB Korb Auskippen PD.Status.Korb.Korb Reinigung iO",

                    SetCoatingLayer = "NL.PLC.Blocks.3 Modul 3.12 Korb Auskippen.00 Allgemein.DB Korb Auskippen PD.Status.Charge.Beschichtungen.Soll"
                };
                MVBasket2.Margin = new Thickness(370, 475, 0, 0);
                MVBasket2.Width = 38;
                MVBasket2.Height = 46;
                MVBasket2.HorizontalAlignment = HorizontalAlignment.Left;
                MVBasket2.VerticalAlignment = VerticalAlignment.Top;
            });
        }

        private void BGW14_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LayoutRoot.Children.Add(MVBasket1);
            LayoutRoot.Children.Add(MVBasket2);
            BackgroundWorker BGW15 = new BackgroundWorker();
            BGW15.DoWork += BGW15_DoWork;
            BGW15.RunWorkerCompleted += BGW15_RunWorkerCompleted;
            BGW15.RunWorkerAsync();
        }
        #endregion
        #region - - - B1&B2&B3 - - -
        Button B1;
        Button B2;
        Button B3;
        private void BGW15_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.InvokeAsync(delegate
            {
                B1 = new Button
                {
                    Margin = new Thickness(370, 0, 0, 479),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Height = 77,
                    Width = 38
                };
                B1.Click += Button_Click;
                B1.Style = (Style)FindResource("B_Material");
            });

            Dispatcher.InvokeAsync(delegate
            {
                B2 = new Button
                {
                    Margin = new Thickness(348, 565, 0, 230),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Width = 43
                };
                B2.Click += Button_Click_1;
                B2.Style = (Style)FindResource("B_Material");
            });

            Dispatcher.InvokeAsync(delegate
            {
                B3 = new Button
                {
                    Margin = new Thickness(392, 565, 0, 230),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Width = 43
                };
                B3.Click += Button_Click_2;
                B3.Style = (Style)FindResource("B_Material");
            });
        }

        private void BGW15_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LayoutRoot.Children.Add(B1);
            LayoutRoot.Children.Add(B2);
            LayoutRoot.Children.Add(B3);
            BackgroundWorker BGW16 = new BackgroundWorker();
            BGW16.DoWork += BGW16_DoWork;
            BGW16.RunWorkerCompleted += BGW16_RunWorkerCompleted;
            BGW16.RunWorkerAsync();
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
                Module = 4,
                M1_Station = 0,
                M2_Station = 0,
                M3_Station = 0,
                M4_Station = 1,
                OvenTray = 0,
                TB_Shelve = 0,
                TB_Level = 0,
                Header = "@Status.Text37",
                Type = "Basket"
            }).Execute();
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            (new SP
            {
                Module = 4,
                M1_Station = 0,
                M2_Station = 0,
                M3_Station = 0,
                M4_Station = 2,
                OvenTray = 0,
                TB_Shelve = 0,
                TB_Level = 0,
                Header = "@Status.Text38",
                Type = "Tray"
            }).Execute();
        }
        #endregion
        #region - - - BC - - -
        MV_BC BC;
        private void BGW16_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.InvokeAsync(delegate
            {
                BC = new MV_BC()
                {
                    DoorStatus = "NL.PLC.Blocks.3 Modul 3.10 Reinigung.00 Allgemein.DB Reinigung Allgemein HMI.Actual value.Türe"
                };
                BC.Margin = new Thickness(39, 478, 0, 0);
                BC.HorizontalAlignment = HorizontalAlignment.Left;
                BC.VerticalAlignment = VerticalAlignment.Top;
            });
        }

        private void BGW16_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LayoutRoot.Children.Add(BC);
            BackgroundWorker BGW17 = new BackgroundWorker();
            BGW17.DoWork += BGW17_DoWork;
            BGW17.RunWorkerCompleted += BGW17_RunWorkerCompleted;
            BGW17.RunWorkerAsync();
        }
        #endregion
        #region - - - CoatingCabin - - -
        MV_CoatingCabin CoatingCabin;
        private void BGW17_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.InvokeAsync(delegate
            {
                CoatingCabin = new MV_CoatingCabin()
                {
                    LTBTemperature = "NL.PLC.Blocks.2 Modul 2.05 Tauchbecken.DB LTB HMI.Actual value.Dipping Vat.Lack Temperatur",
                    isLTB = "NL.PLC.Blocks.2 Modul 2.05 Tauchbecken.DB LTB HMI.Actual value.Dipping Vat.Vorhanden",
                    DoorStatus = "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.Actual value.Tür",
                    PurgeStatus= "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.Status.Abluft.Spühlzeit aktiv"
                };
                CoatingCabin.Margin = new Thickness(102, 568, 0, 0);
                CoatingCabin.HorizontalAlignment = HorizontalAlignment.Left;
                CoatingCabin.VerticalAlignment = VerticalAlignment.Top;
            });
        }

        private void BGW17_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LayoutRoot.Children.Add(CoatingCabin);
            BackgroundWorker BGW18 = new BackgroundWorker();
            BGW18.DoWork += BGW18_DoWork;
            BGW18.RunWorkerCompleted += BGW18_RunWorkerCompleted;
            BGW18.RunWorkerAsync();
        }
        #endregion
        #region - - - Lift & Tilt - - -
        MV_LD LD;
        private void BGW18_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.InvokeAsync(delegate
            {
                LD = new MV_LD
                {
                    Margin = new Thickness(189, 190, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Mod1Status = "NLM4.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Produktionsmodus.Neu befüllen.Gesperrt wegen Rückführung",
                    Mod2Status= "NLM4.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Produktionsmodus.Neu befüllen.Status"
                };
            });
        }

        private void BGW18_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LayoutRoot.Children.Add(LD);
        }
        #endregion

        private void LayoutRoot_Unloaded(object sender, RoutedEventArgs e)
        {
            Task obTask = Task.Run(async () =>
            {
                await Task.Delay(100);
                await Dispatcher.InvokeAsync(delegate
                {
                    LayoutRoot.Children.Clear();
                });
            });
        }
    }
}



