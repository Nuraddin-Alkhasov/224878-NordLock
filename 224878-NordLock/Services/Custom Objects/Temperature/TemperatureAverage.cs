using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using VisiWin.ApplicationFramework;

namespace HMI.Services.Custom_Objects
{
    public class TemperatureAverage
    {

        ObservableCollection<Material> Materials = new ObservableCollection<Material>();
        public string VW_Temperature { get; set; }
        public string VW_Station { get; set; }
        public int StartId { get; set; }
        public int EndId { get; set; }
        public void DoWork()
        {
            try 
            {
                float Temperature = (float)ApplicationService.GetVariableValue(VW_Temperature);

                for (int i = StartId; i <= EndId; i++)
                {
                    if ((bool)ApplicationService.GetVariableValue(VW_Station + "[" + i + "].Status.Charge.Material vorhanden"))
                    {
                        uint OrderId = (uint)ApplicationService.GetVariableValue(VW_Station + "[" + i + "].Header.Order Id");
                        short Charge = (short)ApplicationService.GetVariableValue(VW_Station + "[" + i + "].Status.Charge.Chargen Nummer");

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
            catch 
            {
            
            }
           
        }

        bool CheckIfExist(uint _OrderId, short _Charge)
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

        Material GetMaterial(uint _OrderId, short _Charge)
        {
            return Materials.Where(x => x.OrderId == _OrderId && x.Charge == _Charge).ToArray().Length > 0 ? Materials.Where(x => x.OrderId == _OrderId && x.Charge == _Charge).ToArray()[0] : null;
        }
        public double[] GetTemperature(uint _OrderId, short _Charge)
        {
            Material x = GetMaterial(_OrderId, _Charge);
            double[] retval = new double[] { 0, 0, 0 };
            if (x != null)
            {
                retval[0] = (double)Math.Round(x.Temperatures.Min(), 1);
                retval[1] = (double)Math.Round(x.AverageTemperature, 1);
                retval[2] = (double)Math.Round(x.Temperatures.Max(), 1);

                Materials.Remove(x);

            }
            return retval;
        }
        
    }
}
