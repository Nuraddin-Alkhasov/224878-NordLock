using HMI.Module;
using HMI.UserControls;
using HMI.Views.MainRegion.Recipe;
using HMI.Views.MessageBoxRegion;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VisiWin.ApplicationFramework;
using VisiWin.Commands;
using VisiWin.DataAccess;
using VisiWin.Recipe;

namespace HMI.Views.MainRegion.MachineOverview
{

    [ExportAdapter("StatusTBAdapter")]
    public class StatusTBAdapter : AdapterBase
    {
        public StatusTBAdapter()
        {
            this.ShowMRData = new ActionCommand(this.ShowMRDataCommandExecuted);

            this.UpdateData = new ActionCommand(this.UpdateDataCommandExecuted);
            this.DeleteData = new ActionCommand(this.DeleteDataCommandExecuted);
            this.Close = new ActionCommand(this.CloseCommandExecuted);
        }


        #region - - - Properties - - -
        string Shelve;
        string Level;
        VWRecipe VWR;
        private Order currentOrder = new Order();
        public Order CurrentOrder
        {
            get { return currentOrder; }
            set
            {
                if (value != null)
                {
                    this.currentOrder = value;
                    this.OnPropertyChanged("CurrentOrder");
                }
            }
        }
        private string currentSL = "A | 5";
        public string CurrentSL
        {
            get { return currentSL; }
            set
            {
                if (value != null)
                {
                    this.currentSL = value;
                    this.OnPropertyChanged("CurrentSL");
                }
            }
        }
        private string boxNumber = "0";
        public string BoxNumber
        {
            get { return boxNumber; }
            set
            {
                if (value != null)
                {
                    this.boxNumber = value;
                    this.OnPropertyChanged("BoxNumber");
                }
            }
        }
        private string run = "0";
        public string Run
        {
            get { return run; }
            set
            {
                if (value != null)
                {
                    this.run = value;
                    this.OnPropertyChanged("Run");
                }
            }
        }
        private string charge = "0";
        public string Charge
        {
            get { return charge; }
            set
            {
                if (value != null)
                {
                    this.charge = value;
                    this.OnPropertyChanged("Charge");
                }
            }
        }
        private string weight = "0.0";
        public string Weight
        {
            get { return weight; }
            set
            {
                if (value != null)
                {
                    this.weight = value;
                    this.OnPropertyChanged("Weight");
                }
            }
        }
        private bool isMaterial = false;
        public bool IsMaterial
        {
            get { return isMaterial; }
            set
            {
                this.isMaterial = value;
                this.OnPropertyChanged("IsMaterial");
            }
        }
        private string setRun = "0";
        public string SetRun
        {
            get { return setRun; }
            set
            {
                if (value != null)
                {
                    this.setRun = value;
                    this.OnPropertyChanged("SetRun");
                }
            }
        }
        private string actualRun = "0";
        public string ActualRun
        {
            get { return actualRun; }
            set
            {
                if (value != null)
                {
                    this.actualRun = value;
                    this.OnPropertyChanged("ActualRun");
                }
            }
        }

        private string paint = "";
        public string Paint
        {
            get { return paint; }
            set
            {
                this.paint = value;
                this.OnPropertyChanged("Paint");
            }
        }

        private string nextpaint = "";
        public string NextPaint
        {
            get { return nextpaint; }
            set
            {
                this.nextpaint = value;
                this.OnPropertyChanged("NextPaint");
            }
        }

        private bool isTray = false;
        public bool IsTray
        {
            get { return isTray; }
            set
            {
                this.isTray = value;
                this.OnPropertyChanged("IsTray");
            }
        }

        private bool isDischarge = false;
        public bool IsDischarge
        {
            get { return isDischarge; }
            set
            {
                this.isDischarge = value;
                this.OnPropertyChanged("IsDischarge");
            }
        }

        private bool isQC_M = false;
        public bool IsQC_M
        {
            get { return isQC_M; }
            set
            {
                this.isQC_M = value;
                this.OnPropertyChanged("IsQC_M");
            }
        }

        private bool isUnderObservation = false;
        public bool IsUnderObservation
        {
            get { return isUnderObservation; }
            set
            {
                this.isUnderObservation = value;
                this.OnPropertyChanged("IsUnderObservation");
            }
        }

        private int observationPossition = 0;
        public int ObservationPossition
        {
            get { return observationPossition; }
            set
            {
                this.observationPossition = value;
                this.OnPropertyChanged("ObservationPossition");
            }
        }

        private string weightL = "0.0";
        public string WeightL
        {
            get { return weightL; }
            set
            {
                if (value != null)
                {
                    this.weightL = value;
                    this.OnPropertyChanged("WeightL");
                }
            }
        }
        private string weightR = "0.0";
        public string WeightR
        {
            get { return weightR; }
            set
            {
                if (value != null)
                {
                    this.weightR = value;
                    this.OnPropertyChanged("WeightR");
                }
            }
        }

        IRegionService iRS = ApplicationService.GetService<IRegionService>();
        #endregion

        #region - - - Commands - - -
        public ICommand ShowMRData { get; set; }
        void ShowMRDataCommandExecuted(object parameter)
        {
            ApplicationService.SetView("MessageBoxRegion", "MO_MRData", CurrentOrder.MR.Id);
        }

        public ICommand UpdateData { get; set; }
        void UpdateDataCommandExecuted(object parameter)
        {
            if (MessageBoxView.Show("@MessageBox.Text3", "@Buttons.Text8", MessageBoxButton.YesNo, MessageBoxResult.No, MessageBoxIcon.Question) == MessageBoxResult.Yes)
            {
                if (!IsTray)
                {
                    Task obTask = Task.Run(async () =>
                    {
                        IRecipeClass TBS = ApplicationService.GetService<IRecipeService>().GetRecipeClass("TrayBuffer");
                        var result1 = await TBS.SetDefaultValuesToBufferAsync();


                        foreach (VWVariable VWV in VWR.VWVariables)
                        {
                            VWV.Value = 0;
                        }

                        //VWR.VWVariables[VWR.VWVariables.IndexOf(VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Tablett.Belegt").ToArray()[0])].Value = Convert.ToInt32(IsTray);
                        //VWR.VWVariables[VWR.VWVariables.IndexOf(VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Tablett.Function.Discharge").ToArray()[0])].Value = IsMaterial ?  Convert.ToInt32(IsDischarge) : 0;
                        //VWR.VWVariables[VWR.VWVariables.IndexOf(VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Tablett.Function.Manuall QS").ToArray()[0])].Value = Convert.ToInt32(IsQC_M);
                        VWR.VWVariables[VWR.VWVariables.IndexOf(VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Tablett.Function.Observation").ToArray()[0])].Value = Convert.ToInt32(IsUnderObservation);
                        VWR.VWVariables[VWR.VWVariables.IndexOf(VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Tablett.Function.Observation position").ToArray()[0])].Value = ObservationPossition;

                        foreach (string v in TBS.GetVariableNames())
                        {
                            TBS.SetValue(v, GetValue(v));
                        }

                        var c = (new LocalDBAdapter("UPDATE Tray_Buffer " +
                                                   "SET Order_Id = -1, Status = '" + VWR.GetXML() + "' " +
                                                   "WHERE Coord = '" + GetShelveLeter(Convert.ToInt32(Shelve)) + Level + "';")).DB_Input();


                        await Dispatcher.InvokeAsync(delegate
                        {
                            MO_TrayBuffer MO_TB = (MO_TrayBuffer)iRS.GetView("MO_TrayBuffer");
                            MO_TB.GetTBTray(Shelve, Level).UpdateTrayStatus();
                        });

                        var result2 = await TBS.WriteBufferToProcessAsync();

                        ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.Data.Write", 1);

                    });
                    obTask.ContinueWith(async x =>
                    {
                        await Task.Delay(600);
                        ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.Data.Write", 0);
                        CloseCommandExecuted(null);
                    }, TaskContinuationOptions.OnlyOnRanToCompletion);
                }
                else
                {
                    Task obTask = Task.Run(async () =>
                    {
                        IRecipeClass TBS = ApplicationService.GetService<IRecipeService>().GetRecipeClass("TrayBuffer");
                        var result1 = await TBS.SetDefaultValuesToBufferAsync();

                        VWR.VWVariables[VWR.VWVariables.IndexOf(VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Tablett.Belegt").ToArray()[0])].Value = -1;
                        VWR.VWVariables[VWR.VWVariables.IndexOf(VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Tablett.Function.Discharge").ToArray()[0])].Value = IsMaterial ? Convert.ToInt32(IsDischarge) : 0;
                        VWR.VWVariables[VWR.VWVariables.IndexOf(VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Tablett.Function.Manuall QS").ToArray()[0])].Value = IsMaterial ? Convert.ToInt32(IsQC_M) : 0;
                        VWR.VWVariables[VWR.VWVariables.IndexOf(VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Tablett.Function.Observation").ToArray()[0])].Value = Convert.ToInt32(IsUnderObservation);
                        VWR.VWVariables[VWR.VWVariables.IndexOf(VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Tablett.Function.Observation position").ToArray()[0])].Value = ObservationPossition;

                        foreach (string v in TBS.GetVariableNames())
                        {
                            TBS.SetValue(v, GetValue(v));
                        }


                        var c = (new LocalDBAdapter("UPDATE Tray_Buffer " +
                                   "SET Order_Id = " + CurrentOrder.Id + ", Status = '" + VWR.GetXML() + "' " +
                                   "WHERE Coord = '" + GetShelveLeter(Convert.ToInt32(Shelve)) + Level + "';")).DB_Input();

                        await Dispatcher.InvokeAsync(delegate
                        {
                            MO_TrayBuffer MO_TB = (MO_TrayBuffer)iRS.GetView("MO_TrayBuffer");
                            MO_TB.GetTBTray(Shelve, Level).UpdateTrayStatus();
                        });

                        var result2 = await TBS.WriteBufferToProcessAsync();

                        ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.Data.Write", 1);


                    });
                    obTask.ContinueWith(async x =>
                    {
                        await Task.Delay(600);
                        ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.Data.Write", 0);

                        CloseCommandExecuted(null);
                    }, TaskContinuationOptions.OnlyOnRanToCompletion);

                }
            }

        }

        public ICommand DeleteData { get; set; }
        void DeleteDataCommandExecuted(object parameter)
        {
            if (MessageBoxView.Show("@MessageBox.Text3", "@Buttons.Text9", MessageBoxButton.YesNo, MessageBoxResult.No, MessageBoxIcon.Question) == MessageBoxResult.Yes)
            {
                Task obTask = Task.Run(async () =>
                {
                    IRecipeClass TBS = ApplicationService.GetService<IRecipeService>().GetRecipeClass("TrayBuffer");
                    var result1 = await TBS.SetDefaultValuesToBufferAsync();


                    foreach (VWVariable VWV in VWR.VWVariables)
                    {
                        VWV.Value = 0;
                    }

                    VWR.VWVariables[VWR.VWVariables.IndexOf(VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Tablett.Belegt").ToArray()[0])].Value = Convert.ToInt32(IsTray);
                    VWR.VWVariables[VWR.VWVariables.IndexOf(VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Tablett.Function.Observation").ToArray()[0])].Value = Convert.ToInt32(IsUnderObservation);
                    VWR.VWVariables[VWR.VWVariables.IndexOf(VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Tablett.Function.Observation position").ToArray()[0])].Value = ObservationPossition;

                    // VWR.VWVariables[VWR.VWVariables.IndexOf(VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Tablett.Function.Discharge").ToArray()[0])].Value = IsMaterial ? Convert.ToInt32(IsDischarge) : 0;
                    // VWR.VWVariables[VWR.VWVariables.IndexOf(VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Tablett.Function.Manuall QS").ToArray()[0])].Value = Convert.ToInt32(IsQC_M);

                    foreach (string v in TBS.GetVariableNames())
                    {
                        TBS.SetValue(v, GetValue(v));
                    }

                    var c = (new LocalDBAdapter("UPDATE Tray_Buffer " +
                                               "SET Order_Id = -1, Status = '" + VWR.GetXML() + "' " +
                                               "WHERE Coord = '" + GetShelveLeter(Convert.ToInt32(Shelve)) + Level + "';")).DB_Input();


                    await Dispatcher.InvokeAsync(delegate
                    {
                        MO_TrayBuffer MO_TB = (MO_TrayBuffer)iRS.GetView("MO_TrayBuffer");
                        MO_TB.GetTBTray(Shelve, Level).UpdateTrayStatus();
                    });

                    var result2 = await TBS.WriteBufferToProcessAsync();

                    ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.Data.Delete", 1);

                });
                obTask.ContinueWith(async x =>
                {
                    await Task.Delay(600);
                    ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.Data.Delete", 0);
                    CloseCommandExecuted(null);
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
            }


        }

        
        public ICommand Close { get; set; }
        void CloseCommandExecuted(object parameter)
        {
            Task obTask = Task.Run(async () =>
            {
                await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                {
                    ApplicationService.SetView("DialogRegion", "EmptyView");
                });
            });
        }

        #endregion

        #region - - - Methods - - -

        public void Update(string[] Parameters)
        {
            Shelve = Parameters[0];
            Level = Parameters[1];
            CurrentSL = GetShelveLeter(Convert.ToInt32(Shelve)).ToString() +" | " + (Convert.ToInt32(Level) + 1).ToString() ;
            
            Task obTask = Task.Run(() =>
            {
                DataTable DT = (new LocalDBAdapter("SELECT Order_Id, Status " +
                               "FROM Tray_Buffer " +
                               "WHERE Coord = '" + GetShelveLeter(Convert.ToInt32(Parameters[0])) + Parameters[1] + "'; ")).DB_Output();
                if (DT.Rows.Count > 0)
                {
                    VWR = new VWRecipe("TBStatus", DT.Rows[0]["Status"].ToString());
                  
                    long Order_Id = (long) DT.Rows[0]["Order_Id"];
                    //seting header
                    DT = (new LocalDBAdapter("SELECT Orders.Id, Orders.Data_1, Orders.Data_2, Orders.Data_3, Orders.MR_Id, Recipes_MR.Name " +
                                             "FROM Orders " +
                                             "INNER JOIN Recipes_MR ON Orders.MR_Id = Recipes_MR.Id " +
                                             "WHERE Orders.Id = " + Order_Id + "; ")).DB_Output();
                    if (DT.Rows.Count > 0)
                    {
                        foreach (DataRow r in DT.Rows)
                        {
                            CurrentOrder = new Order()
                            {
                                Id = Order_Id,
                                Data_1 = (string)r["Data_1"],
                                Data_2 = (string)r["Data_2"],
                                Data_3 = (string)r["Data_3"],
                                MR = new MachineRecipe() { Id = (long)r["MR_Id"], Name = (string)r["Name"] },
                            };
                        }
                    }
                    else { CurrentOrder = new Order(); }

                    SetStatus(VWR);
                }

            });
        }

        private string GetShelveLeter(int s)
        {
            switch (s)
            {
                case 0 : return "A";
                case 1: return "B";
                case 2: return "C";
                case 3: return "D";
                case 4: return "E";
                case 5: return "F";
                case 6: return "G";
                case 7: return "H";
                case 8: return "I";
                case 9: return "J";
                case 10: return "K";
                case 11: return "L";
                default: return "A";
            }
        }
        private void SetStatus(VWRecipe VWR) 
        {
            BoxNumber = VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Charge.Box.Nummer").ToArray()[0].Value.ToString();
            Run = VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Charge.Aktueller Durchlauf").ToArray()[0].Value.ToString();
            Charge = VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Charge.Chargen Nummer").ToArray()[0].Value.ToString();
            Weight = VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Charge.Chargen Gewicht").ToArray()[0].Value.ToString();
            IsMaterial = Convert.ToBoolean(Convert.ToInt32(VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Charge.Material vorhanden").ToArray()[0].Value));
            SetRun = VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Charge.Beschichtungen.Soll").ToArray()[0].Value.ToString();
            ActualRun = VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Charge.Beschichtungen.Ist").ToArray()[0].Value.ToString();
            Paint = "";
            NextPaint = "";
            if (Run != "0") 
            {
                DataTable temp = (new LocalDBAdapter("SELECT Paint_Id " +
                    "FROM Recipes_Coating " +
                    "WHERE Id = (SELECT C" + Run + "_Id FROM Recipes_MR WHERE Id = (SELECT MR_Id FROM Orders WHERE Id = " + CurrentOrder.Id + "));")).DB_Output();

                if (temp.Rows.Count > 0)
                {
                    long Charge_Paint_Id = (long)temp.Rows[0]["Paint_Id"];
                    Paint = ApplicationService.GetVariableValue("NL.PLC.Blocks.50 HMI.00 Allgemein.Lacktyp Namen.Lacktyp Name[" + Charge_Paint_Id + "]").ToString();
                }

                temp = (new LocalDBAdapter("SELECT Paint_Id " +
                    "FROM Recipes_Coating " +
                    "WHERE Id = (SELECT C" + (Convert.ToInt32(Run)+1).ToString() + "_Id FROM Recipes_MR WHERE Id = (SELECT MR_Id FROM Orders WHERE Id = " + CurrentOrder.Id + "));")).DB_Output();

                if (temp.Rows.Count > 0)
                {
                    long Charge_Paint_Id = (long)temp.Rows[0]["Paint_Id"];
                    NextPaint = ApplicationService.GetVariableValue("NL.PLC.Blocks.50 HMI.00 Allgemein.Lacktyp Namen.Lacktyp Name[" + Charge_Paint_Id + "]").ToString();
                }
            }

            IsTray = Convert.ToBoolean(Convert.ToInt32(VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Tablett.Belegt").ToArray()[0].Value));
            IsDischarge = Convert.ToBoolean(Convert.ToInt32(VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Tablett.Function.Discharge").ToArray()[0].Value));
            IsQC_M = Convert.ToBoolean(Convert.ToInt32(VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Tablett.Function.Manuall QS").ToArray()[0].Value));

            WeightL = VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Tablett.Gewicht links").ToArray()[0].Value.ToString();
            WeightR = VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Tablett.Gewicht rechts").ToArray()[0].Value.ToString();
        }

        private string GetValue(string v) 
        {
            string Item = v;
            if (v.Contains(".Status.Tray."))
            {
                Item = v.Replace("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Tray", "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Tablett");
            }
            if (v.Contains(".Status.Charge."))
            {
                Item = v.Replace("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Charge", "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Charge");
            }

            return VWR.VWVariables.Where(x => (string)x.Item == Item).ToArray()[0].Value.ToString();
        }
        #endregion
    }


}