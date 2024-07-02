using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using VisiWin.ApplicationFramework;

namespace HMI.Services.Custom_Objects
{
    public class PHZTemperatureAverage
    {

        ObservableCollection<Material> Materials = new ObservableCollection<Material>();

        public void DoWork()
        {
            float Temperature = (float)ApplicationService.GetVariableValue("NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.01 Vorzone.DB Vorzone HMI.Istwerte.Temperatur");

            if ((bool)ApplicationService.GetVariableValue("NL.PLC.Blocks.3 Modul 3.14 Bänder.02 Band 1.DB Korb Auskippen Band 1 PD.Status.Charge.Material vorhanden"))
            {
                uint OrderId = (uint)ApplicationService.GetVariableValue("NL.PLC.Blocks.3 Modul 3.14 Bänder.02 Band 1.DB Korb Auskippen Band 1 PD.Header.Order Id");
                short Charge = (short)ApplicationService.GetVariableValue("NL.PLC.Blocks.3 Modul 3.14 Bänder.02 Band 1.DB Korb Auskippen Band 1 PD.Status.Charge.Chargen Nummer");

                if (CheckIfExist(OrderId, Charge))
                {
                    Material x = GetMaterial(OrderId, Charge);
                    x.Temperatures.Add(Temperature);
                }
                else
                {
                    Materials.Add(new Material(OrderId, Charge, Temperature));
                }
            }


            if ((bool)ApplicationService.GetVariableValue("NL.PLC.Blocks.3 Modul 3.14 Bänder.04 Band 2.DB Ofen Band 2 PD.Status.Charge.Material vorhanden"))
            {
                uint OrderId = (uint)ApplicationService.GetVariableValue("NL.PLC.Blocks.3 Modul 3.14 Bänder.04 Band 2.DB Ofen Band 2 PD.Header.Order Id");
                short Charge = (short)ApplicationService.GetVariableValue("NL.PLC.Blocks.3 Modul 3.14 Bänder.04 Band 2.DB Ofen Band 2 PD.Status.Charge.Chargen Nummer");

                if (CheckIfExist(OrderId, Charge))
                {
                    Material x = GetMaterial(OrderId, Charge);
                    x.Temperatures.Add(Temperature);
                }
                else
                {
                    Materials.Add(new Material(OrderId, Charge, Temperature));
                }
            }

            for (int i = 12; i <= 15; i++)
            {
                if ((bool)ApplicationService.GetVariableValue("NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Ofen.Tablett[" + i + "].Status.Charge.Material vorhanden"))
                {
                    uint OrderId = (uint)ApplicationService.GetVariableValue("NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Ofen.Tablett[" + i + "].Header.Order Id");
                    short Charge = (short)ApplicationService.GetVariableValue("NLM4.PLC.Blocks.7 Tracking / Kommunikation.DB Tracking Ofen.Tablett[" + i + "].Status.Charge.Chargen Nummer");

                    if (CheckIfExist(OrderId, Charge))
                    {
                        Material x = GetMaterial(OrderId, Charge);
                        x.Temperatures.Add(Temperature);
                    }
                    else
                    {
                        Materials.Add(new Material(OrderId, Charge, Temperature));
                    }
                }
            }   
        }

        public bool CheckIfExist(uint _OrderId, short _Charge)
        {
            if (Materials.Where(x => x.OrderId == _OrderId && x.Charge == _Charge).ToArray().Length != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Material GetMaterial(uint _OrderId, short _Charge)
        {
            return Materials.Where(x => x.OrderId == _OrderId && x.Charge == _Charge).ToArray().Length > 0 ? Materials.Where(x => x.OrderId == _OrderId && x.Charge == _Charge).ToArray()[0] : null;
        }

        public double [] GetTemperature(uint _OrderId, short _Charge)
        {
            Material x = GetMaterial(_OrderId, _Charge);
            double[] retval = new double[] { 0, 0, 0 };
            if (x != null)
            {
                retval[0] = (double)Math.Round(x.Temperatures.Min(), 1);
                retval[1]= (double)Math.Round(x.AverageTemperature, 1);
                retval[2] = (double)Math.Round(x.Temperatures.Max(), 1);

                Materials.Remove(x);
                
            }
            return retval;
        }
      

    }
}
