using HMI.Module;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;
using VisiWin.ApplicationFramework;

namespace HMI.Views.MainRegion.Protocol
{
    public class Order : AdapterBase
    {
        public Order()
        {
            Id = -1;
            TimeStamp = "-";
            Data_1 = "";
            Data_2 = "";
            Data_3 = "";
            MR_Id = -1;
            MR = "";
            Charges = 0;
            User = "";
            Weight = 0;
            ChargesList = new ObservableCollection<Charge>();
        }

        public long Id { set; get; }

        public string TimeStamp { set; get; }

        public string Data_1 { set; get; }

        public string Data_2 { set; get; }

        public string Data_3 { set; get; }

        public long MR_Id { set; get; }

        public string MR { set; get; }
        public MR MRDetailed { set; get; }

        public long Charges { set; get; }

        public string User { set; get; }
        double weight { set; get; }
        public double Weight
        {
            get { return this.weight; }
            set
            {
                if (!Equals(value, this.weight))
                {
                    this.weight = value;
                    this.OnPropertyChanged("Weight");
                }
            }
        }

        ObservableCollection<Charge> chargesList;
        public ObservableCollection<Charge> ChargesList
        {
            get { return this.chargesList; }
            set
            {
                if (!Equals(value, this.chargesList))
                {
                    this.chargesList = value;
                    this.OnPropertyChanged("ChargesList");
                }
            }
        }
        public void SetChargesList()
        {
            Task.Run(() =>
            {
                ObservableCollection<Charge> ret_val = new ObservableCollection<Charge>();

                DataTable DT = (new LocalDBAdapter("SELECT * FROM Charges WHERE Order_Id = " + Id.ToString())).DB_Output();

                if (DT.Rows.Count > 0)
                {
                    double tempWeight = 0;
                    foreach (DataRow r in DT.Rows)
                    {
                        ret_val.Add(new Charge()
                        {
                            Id = (long)r["Id"],
                            Order_Id = (long)r["Order_Id"],
                            Box_Id = (long)r["Box_Id"],
                            ChargeNr = (long)r["Charge"],
                            Weight = (double)r["Weight"],
                            Optimized = Convert.ToBoolean(r["Optimized"]),
                            Runs = (long)r["Runs"],
                            Error = (long)r["Error"],
                            Start = r["Start"].ToString() == "" ? "" : ((DateTime)r["Start"]).ToString("dd.MM.yyyy HH:mm:ss"),
                            End = r["End"].ToString() == "" ? "" : ((DateTime)r["End"]).ToString("dd.MM.yyyy HH:mm:ss")
                        });
                        tempWeight += (double)r["Weight"];
                    }
                    Weight = tempWeight;
                    ChargesList = ret_val;
                }
            });
        }
    }
}
