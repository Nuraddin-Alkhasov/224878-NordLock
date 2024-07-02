using HMI.Module;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;

namespace HMI.Views.MainRegion.Protocol
{

    public class Run
    {
        public Run()
        {
            Id = -1;
            Charge_Id = -1;

            RunNr = 1;
            SetValues_Id = -1;
            ActualValues_Id = -1;

            Start = "-";
            Coating_S = "-";
            Coating_E = "-";
            Preheating_S = "-";
            Preheating_E = "-";
            Drying_S = "-";
            Drying_E = "-";
            Cooling_S = "-";
            Cooling_E = "-";
            End = "-";
        }

        public long Id { set; get; }
        public long Charge_Id { set; get; }
        public long RunNr { set; get; }
        public long SetValues_Id { set; get; }
        public long ActualValues_Id { set; get; }
        public string Start { set; get; }
        public string Coating_S { set; get; }
        public string Coating_E { set; get; }
        public string Preheating_S { set; get; }
        public string Preheating_E { set; get; }
        public string Drying_S { set; get; }
        public string Drying_E { set; get; }
        public string Cooling_S { set; get; }
        public string Cooling_E { set; get; }
        public string End { set; get; }
        public SetValues SetValues { set; get; }
        public ActualValues ActualValues { set; get; }
        private void SetSetValues()
        {
            Task.Run(() =>
            {
                SetValues ret_val = new SetValues();

                DataTable DT = (new LocalDBAdapter("SELECT * FROM SetValues WHERE Id = " + SetValues_Id.ToString())).DB_Output();

                if (DT.Rows.Count > 0)
                {
                    foreach (DataRow r in DT.Rows)
                    {
                        //ret_val.Add(new Run()
                        //{
                        //    Id = (long)r["Id"]
                            
                        //});
                    }
                }
                SetValues = ret_val;
            });
        }
        private void SetActualValues()
        {
            Task.Run(() =>
            {
                ActualValues ret_val = new ActualValues();

                DataTable DT = (new LocalDBAdapter("SELECT * FROM ActualValues WHERE Id = " + ActualValues_Id.ToString())).DB_Output();

                if (DT.Rows.Count > 0)
                {
                    foreach (DataRow r in DT.Rows)
                    {
                        //ret_val.Add(new Run()
                        //{
                        //    Id = (long)r["Id"]

                        //});
                    }
                }
                ActualValues = ret_val;
            });
        }
    }
}
