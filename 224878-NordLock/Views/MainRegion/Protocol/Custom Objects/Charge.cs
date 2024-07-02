using HMI.Module;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;
using VisiWin.ApplicationFramework;

namespace HMI.Views.MainRegion.Protocol
{

    public class Charge : AdapterBase
    {
        public Charge()
        {
            Id = -1;
            Order_Id = -1;
            Box_Id = 1;
            ChargeNr = 1;
            Weight = 0;
            Optimized = false;
            Runs = 0;
            Error = 0;

            Start = "-";
            End = "-";

            ErrorList = new ObservableCollection<Error>();
            RunList = new ObservableCollection<Run>();
            RemarkList = new ObservableCollection<Remark>();
        }

        public long Id { set; get; }

        public long Order_Id { set; get; }
        public long Box_Id { set; get; }

        public long ChargeNr { set; get; }
        public double Weight { set; get; }
        public bool Optimized { set; get; }

        public long Runs { set; get; }

        public long Error { set; get; }

        public string Start { set; get; }
        
        public string End { set; get; }
        
        ObservableCollection<Error> errorList;
        public ObservableCollection<Error> ErrorList
        {
            get { return this.errorList; }
            set
            {
                this.errorList = value;
                this.OnPropertyChanged("ErrorList");
            }
        }
        ObservableCollection<Run> runList;
        public ObservableCollection<Run> RunList
        {
            get { return this.runList; }
            set
            {
                this.runList = value;
                this.OnPropertyChanged("RunList");
            }
        }
        public ObservableCollection<Remark> RemarkList { set; get; }

        public void SetErrorList()
        {
            Task.Run(() =>
            {
                ObservableCollection<Error> ret_val = new ObservableCollection<Error>();

                DataTable DT = (new LocalDBAdapter("SELECT * FROM Errors WHERE Charge_Id = " + Id.ToString())).DB_Output();

                if (DT.Rows.Count > 0)
                {
                    foreach (DataRow r in DT.Rows)
                    {
                        ret_val.Add(new Error()
                        {
                            Id = (long)r["Id"],
                            Charge_Id = (long)r["Charge_Id"],
                            TimeStamp = ((DateTime)r["TimeStamp"]).ToString("dd.MM.yyyy HH:mm:ss"),
                            Text = (string)r["Text"],
                            Comment = (string)r["Comment"],
                            User = (string)r["User"]
                        }); 
                    }
                }
                ErrorList = ret_val;
            });
        }

        public void SetRunList()
        {
            Task.Run(() =>
            {
                ObservableCollection<Run> ret_val = new ObservableCollection<Run>();

                DataTable DT = (new LocalDBAdapter("SELECT * FROM Runs WHERE Charge_Id = " + Id.ToString())).DB_Output();

                if (DT.Rows.Count > 0)
                {
                    foreach (DataRow r in DT.Rows)
                    {
                        ret_val.Add(new Run() 
                        {
                            Id = (long)r["Id"],
                            Charge_Id = (long)r["Charge_Id"],
                            RunNr = (long)r["Run"],
                            SetValues_Id = (long)r["SetValues_Id"],
                            ActualValues_Id = (long)r["ActualValues_Id"],
                            Start = r["Start"].ToString() == "" ? "" : ((DateTime)r["Start"]).ToString("dd.MM.yyyy HH:mm:ss"),
                            Coating_S = r["Coating_S"].ToString() == "" ? "" : ((DateTime)r["Coating_S"]).ToString("dd.MM.yyyy HH:mm:ss"),
                            Coating_E = r["Coating_E"].ToString() == "" ? "" : ((DateTime)r["Coating_E"]).ToString("dd.MM.yyyy HH:mm:ss"),
                            Preheating_S = r["Preheating_S"].ToString() == "" ? "" : ((DateTime)r["Preheating_S"]).ToString("dd.MM.yyyy HH:mm:ss"),
                            Preheating_E = r["Preheating_E"].ToString() == "" ? "" : ((DateTime)r["Preheating_E"]).ToString("dd.MM.yyyy HH:mm:ss"),
                            Drying_S = r["Drying_S"].ToString() == "" ? "" : ((DateTime)r["Drying_S"]).ToString("dd.MM.yyyy HH:mm:ss"),
                            Drying_E = r["Drying_E"].ToString() == "" ? "" : ((DateTime)r["Drying_E"]).ToString("dd.MM.yyyy HH:mm:ss"),
                            Cooling_S = r["Cooling_S"].ToString() == "" ? "" : ((DateTime)r["Cooling_S"]).ToString("dd.MM.yyyy HH:mm:ss"),
                            Cooling_E = r["Cooling_E"].ToString() == "" ? "" : ((DateTime)r["Cooling_E"]).ToString("dd.MM.yyyy HH:mm:ss"),
                            End = r["End"].ToString() == "" ? "" : ((DateTime)r["End"]).ToString("dd.MM.yyyy HH:mm:ss"),
                        });
                    }
                }
                RunList = ret_val;
            });
        }

        private void SetRemarkList()
        {
            Task.Run(() =>
            {
                ObservableCollection<Remark> ret_val = new ObservableCollection<Remark>();

                DataTable DT = (new LocalDBAdapter("SELECT * FROM Remarks WHERE Charge_Id = " + Id.ToString())).DB_Output();

                if (DT.Rows.Count > 0)
                {
                    foreach (DataRow r in DT.Rows)
                    {
                        ret_val.Add(new Remark() 
                        {
                            Id = (long)r["Id"],
                            Charge_Id = (long)r["Charge_Id"],
                            TimeStamp = (string)r["TimeStamp"],
                            Text = (string)r["Text"],
                            Comment = (string)r["Comment"],
                            User = (string)r["User"]
                        });
                    }
                }
                RemarkList = ret_val;
            });
        }
    }
}
