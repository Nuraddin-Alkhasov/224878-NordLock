using HMI.Interfaces;
using HMI.Services.Custom_Objects;
using System.ComponentModel.Composition;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;

namespace HMI.Services
{

    [ExportService(typeof(Service_Protocol))]
    [Export(typeof(Service_Protocol))]
    class Service_Protocol : ServiceBase, IProtocol
    {
    

        Feeding F;
        Coating C;
        Preheating PHZ;
        Drying D;
        Cooling CZ;
        Return R;
        Unloading U;

        public Service_Protocol()
        {
            if (ApplicationService.IsInDesignMode)
                return;
        }

        #region OnProject

        // Hier stehen noch keine VisiWin Funktionen zur Verfügung
        protected override void OnLoadProjectStarted()
        {
            base.OnLoadProjectStarted();
        }

        // Hier kann auf die VisiWin Funktionen zugegriffen werden
        protected override void OnLoadProjectCompleted()
        {
           

            F = new Feeding
            {
                StationName="Feeding",

                VN_NewCharge = "NL.PLC.Blocks.1 Modul 1.04 Basket filling station.DB KBD HMI.PC.Protkollierung.NewCharge", // trigger bit

                VN_Order_Id = "NL.PLC.Blocks.1 Modul 1.04 Basket filling station.DB KBD PD.Header.Order Id", //order id
                VN_Charge = "NL.PLC.Blocks.1 Modul 1.04 Basket filling station.DB KBD PD.Status.Charge.Chargen Nummer",  //charge
                VN_Run = "NL.PLC.Blocks.1 Modul 1.04 Basket filling station.DB KBD PD.Status.Charge.Aktueller Durchlauf",
               
                VN_Weight = "NL.PLC.Blocks.1 Modul 1.04 Basket filling station.DB KBD PD.Status.Charge.Chargen Gewicht",
                VN_Optimized = "NL.PLC.Blocks.1 Modul 1.04 Basket filling station.DB KBD PD.Status.Charge.Restmengenoptimiert",
                VN_Box_Id = "NL.PLC.Blocks.1 Modul 1.04 Basket filling station.DB KBD PD.Status.Charge.Box.Nummer",

            };

            C = new Coating
            {
                StationName = "Coating",

                VN_Start = "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.PC.Protokollierung.Start",
                VN_End = "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.PC.Protokollierung.End",
                VN_Error = "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.PC.Protokollierung.Sammelstörung",

                VN_Order_Id = "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung PD.Header.Order Id",
                VN_Charge = "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung PD.Status.Charge.Chargen Nummer",
                VN_Run = "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung PD.Status.Charge.Aktueller Durchlauf",

                VN_PaintTemp= "NL.PLC.Blocks.2 Modul 2.05 Tauchbecken.DB LTB HMI.Actual value.Dipping Vat.Lack Temperatur",
                VN_AverageTemperature = "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung PD.Status.Charge.Temperaturen.Beschichtung.Durchlauf",
                ProtocolSetValuesActive = true,
                ProtocolActualValuesActive = true,


                StartEr = 4348,
                EndEr = 5067

            };

            PHZ = new Preheating
            {
                StationName = "Preheating",

                VN_Start = "NL.PLC.Blocks.3 Modul 3.14 Bänder.02 Band 1.DB Korb Auskippen Band 1 HMI.PC.Protkollierung.Start",
                VN_End = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.01 Vorzone.DB Vorzone HMI.PC.Protkollierung.End",
                VN_Error = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.01 Vorzone.DB Vorzone HMI.PC.Protkollierung.Sammelstörung",

                VN_Order_Id_MC1 = "NL.PLC.Blocks.3 Modul 3.14 Bänder.02 Band 1.DB Korb Auskippen Band 1 PD.Header.Order Id", //order id
                VN_Charge_MC1 = "NL.PLC.Blocks.3 Modul 3.14 Bänder.02 Band 1.DB Korb Auskippen Band 1 PD.Status.Charge.Chargen Nummer",
                VN_Run_MC1 = "NL.PLC.Blocks.3 Modul 3.14 Bänder.02 Band 1.DB Korb Auskippen Band 1 PD.Status.Charge.Aktueller Durchlauf",
                VN_isMaterial_MC1= "NL.PLC.Blocks.3 Modul 3.14 Bänder.02 Band 1.DB Korb Auskippen Band 1 PD.Status.Charge.Material vorhanden",

                VN_Order_Id_MC2 = "NL.PLC.Blocks.3 Modul 3.14 Bänder.04 Band 2.DB Ofen Band 2 PD.Header.Order Id", //order id
                VN_Charge_MC2 = "NL.PLC.Blocks.3 Modul 3.14 Bänder.04 Band 2.DB Ofen Band 2 PD.Status.Charge.Chargen Nummer",
                VN_isMaterial_MC2= "NL.PLC.Blocks.3 Modul 3.14 Bänder.04 Band 2.DB Ofen Band 2 PD.Status.Charge.Material vorhanden",

                VN_Order_Id_E = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Ofen.Tablett[16].Header.Order Id", //order id
                VN_Charge_E = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Ofen.Tablett[16].Status.Charge.Chargen Nummer",
                VN_Run_E = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Ofen.Tablett[16].Status.Charge.Aktueller Durchlauf",
                VW_Station = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Ofen.Tablett",


                ProtocolSetValuesActive = true,
                ProtocolActualValuesActive = true,
                VN_AverageTemperature = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Ofen.Tablett[16].Status.Charge.Temperaturen.Vorzone.Durchlauf",

                StartEr = 1857,
                EndEr = 1968
            };
            PHZ.StartDataColector();

            D = new Drying
            {
                StationName = "Drying",

                VN_Start = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.03 Trockner.DB Trockner HMI.PC.Protkollierung.Start",
                VN_End = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.03 Trockner.DB Trockner HMI.PC.Protkollierung.End",
                VN_Error = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.03 Trockner.DB Trockner HMI.PC.Protkollierung.Sammelstörung",

                VN_Order_Id_S = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Ofen.Tablett[18].Header.Order Id", //order id
                VN_Charge_S = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Ofen.Tablett[18].Status.Charge.Chargen Nummer",
                VN_Run_S = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Ofen.Tablett[18].Status.Charge.Aktueller Durchlauf",

                VN_Order_Id_E = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Ofen.Tablett[47].Header.Order Id", //order id
                VN_Charge_E = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Ofen.Tablett[47].Status.Charge.Chargen Nummer",
                VN_Run_E = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Ofen.Tablett[47].Status.Charge.Aktueller Durchlauf",
                
                ProtocolSetValuesActive = true,
                ProtocolActualValuesActive = true,
                VW_Temperature= "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.03 Trockner.DB Trockner HMI.Istwerte.Temperatur",
                VW_Station= "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Ofen.Tablett",
                StartId=18,
                EndId=46,
                VN_AverageTemperature = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Ofen.Tablett[47].Status.Charge.Temperaturen.Trockner.Durchlauf",
                
                StartEr = 2065,
                EndEr = 2128
            };
            D.StartDataColector();

            CZ = new Cooling
            {
                StationName = "Cooling",

                VN_Start = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.04 Kühlzone.DB Kühlzone HMI.PC.Protkollierung.Start",
                VN_End = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.04 Kühlzone.DB Kühlzone HMI.PC.Protkollierung.End",
                VN_Error = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.04 Kühlzone.DB Kühlzone HMI.PC.Protkollierung.Sammelstörung",

                VN_Order_Id_S = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Kühlzone.Tablett[3].Header.Order Id", //order id
                VN_Charge_S = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Kühlzone.Tablett[3].Status.Charge.Chargen Nummer",
                VN_Run_S = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Kühlzone.Tablett[3].Status.Charge.Aktueller Durchlauf",

                VN_Order_Id_E = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Kühlzone.Tablett[13].Header.Order Id", //order id
                VN_Charge_E = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Kühlzone.Tablett[13].Status.Charge.Chargen Nummer",
                VN_Run_E = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Kühlzone.Tablett[13].Status.Charge.Aktueller Durchlauf",
                
                ProtocolSetValuesActive = true,
                ProtocolActualValuesActive = true,
                VW_Temperature = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.04 Kühlzone.DB Kühlzone HMI.Istwerte.Temperatur",
                VW_Station = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Kühlzone.Tablett",
                StartId = 3,
                EndId = 12,
                VN_AverageTemperature = "NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Kühlzone.Tablett[13].Status.Charge.Temperaturen.Kühlzone.Durchlauf",
                
                StartEr = 2177,
                EndEr = 2256
            };
            CZ.StartDataColector();
            R = new Return
            {
                StationName = "Return",

                VN_NewRun = "NLM4.PLC.Blocks.4 Modul 4.02 HVT.Fahrwagen.02 Fahrantrieb.DB HVT Fahrwagen HMI.PC.Protkollierung.NewRun", // trigger bit

                VN_Order_Id = "NLM4.PLC.Blocks.4 Modul 4.02 HVT.Fahrwagen.00 Allgemein.DB HVT Fahrwagen Korb PD.Header.Order Id", //order id
                VN_Charge = "NLM4.PLC.Blocks.4 Modul 4.02 HVT.Fahrwagen.00 Allgemein.DB HVT Fahrwagen Korb PD.Status.Charge.Chargen Nummer",
                VN_Run = "NLM4.PLC.Blocks.4 Modul 4.02 HVT.Fahrwagen.00 Allgemein.DB HVT Fahrwagen Korb PD.Status.Charge.Aktueller Durchlauf",
            };

            U = new Unloading
            {
                VN_EndCharge = "NLM4.PLC.Blocks.4 Modul 4.09 Entladen.02 Entladen Hub.DB Entladen Hub HMI.PC.Protkollierung.End", // trigger bit
                VN_Discharged = "NLM4.PLC.Blocks.4 Modul 4.09 Entladen.01 Box.DB Entladen Box PD.Status.Box.Discharged",
                VN_Order_Id = "NLM4.PLC.Blocks.4 Modul 4.09 Entladen.01 Box.DB Entladen Box PD.Header.Order Id", //order id
                VN_Charge = "NLM4.PLC.Blocks.4 Modul 4.09 Entladen.01 Box.DB Entladen Box PD.Status.Charge.Chargen Nummer",
                VN_Run = "NLM4.PLC.Blocks.4 Modul 4.09 Entladen.01 Box.DB Entladen Box PD.Status.Charge.Aktueller Durchlauf",
            };
            base.OnLoadProjectCompleted();
        }

        #region - - - Event Handlers - - -   

        #endregion

        #region - - - Private Methods - - -   



        #endregion

        // Hier stehen noch die VisiWin Funktionen zur Verfügung
        protected override void OnUnloadProjectStarted()
        {
            try
            {
                PHZ.Stop();
                D.Stop();
                CZ.Stop();
            }
            catch { }
            base.OnUnloadProjectStarted();
        }

        // Hier sind keine VisiWin Funktionen mehr verfügbar. Bei C/S ist die Verbindung zum Server schon getrennt.
        protected override void OnUnloadProjectCompleted()
        {
          
            base.OnUnloadProjectCompleted();
        }
        #endregion
    }
}
