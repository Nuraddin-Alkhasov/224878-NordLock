using HMI.Module;
using HMI.UserControls;
using HMI.Views.MainRegion.Protocol;
using HMI.Views.MainRegion.Recipe;
using HMI.Views.MessageBoxRegion;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VisiWin.ApplicationFramework;
using VisiWin.Commands;

namespace HMI.Views.MainRegion
{
   
    [ExportAdapter("Adapter_SetOrderPriority")]
    public class Adapter_SetOrderPriority : AdapterBase
    {

        public Adapter_SetOrderPriority()
        {
            this.UpCommand = new ActionCommand(this.UpCommandExecuted);
            this.DownCommand = new ActionCommand(this.DownCommandExecuted);
            this.CloseCommand = new ActionCommand(this.CloseCommandExecuted);
            this.SetPriorityCommand = new ActionCommand(this.SetPriorityCommandExecuted);
            TabletsCount = 0;
        }

        #region - - - Properties - - -
        ObservableCollection<Order_Template> orders = new ObservableCollection<Order_Template>();
        public ObservableCollection<Order_Template> Orders
        {
            get { return this.orders; }
            set
            {
                this.orders = value;
                this.OnPropertyChanged("Orders");
            }
        }

        Order_Template selectedOrder = null;
        public Order_Template SelectedOrder
        {
            get { return this.selectedOrder; }
            set
            {
                this.selectedOrder = value;
                this.OnPropertyChanged("SelectedOrder");
            }
        }
        private int _TabletsCount { get; set; }
        public int TabletsCount {
            get { return this._TabletsCount; }
            set
            {
                this._TabletsCount = value;
                this.OnPropertyChanged("TabletsCount");
            }
        }
        #endregion

        #region - - - Commands - - -
        IRegionService iRS = ApplicationService.GetService<IRegionService>();
        public ICommand UpCommand { get; set; }
        void UpCommandExecuted(object parameter)
        {

            SelectedOrder = GetSelectedOrder(); 
            if (SelectedOrder != null) 
            {
                int oldIndex = Orders.IndexOf(SelectedOrder);
                if (oldIndex > 0) 
                {
                    int newIndex = oldIndex - 1;
                    Orders.Move(oldIndex, newIndex);
                    
                    
                    MO_SetOrderPriority MO_SOP = (MO_SetOrderPriority)iRS.GetView("MO_SetOrderPriority");
                    MO_SOP.SV.ScrollIntoView(MO_SOP.SV.SelectedItem);
                }
             
            }
            else { SelectedOrder = GetSelectedOrder(); }
        }
        public ICommand DownCommand { get; set; }
        private void DownCommandExecuted(object parameter)
        {
            SelectedOrder = GetSelectedOrder();
            if (SelectedOrder != null)
            {
                int oldIndex = Orders.IndexOf(SelectedOrder);
                if (oldIndex < Orders.Count - 1)
                {
                    int newIndex = oldIndex + 1;
                    Orders.Move(oldIndex, newIndex);

                    MO_SetOrderPriority MO_SOP = (MO_SetOrderPriority)iRS.GetView("MO_SetOrderPriority");
                    MO_SOP.SV.ScrollIntoView(MO_SOP.SV.SelectedItem);
                }
            }
            else { SelectedOrder = GetSelectedOrder(); }
        }
        public ICommand CloseCommand { get; set; }
        private void CloseCommandExecuted(object parameter)
        {
            ApplicationService.SetView("DialogRegion", "EmptyView");
        }
        public ICommand SetPriorityCommand { get; set; }
        private void SetPriorityCommandExecuted(object parameter)
        {
          
            if (MessageBoxView.Show("@MessageBox.Text3", "@MainView.Text71", MessageBoxButton.YesNo, MessageBoxResult.No, MessageBoxIcon.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    if (Orders.Count > 0) 
                    {
                        SetTrayBufferData();

                        for (int i = 0; i <= Orders.Count - 1; i++)
                        {
                            ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Handshake.from PC.Priority[" + i + "]", Orders[i].O.Id);
                        }
                        for (int i = Orders.Count; i <= 69; i++)
                        {
                            ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Handshake.from PC.Priority[" + i + "]", 0);
                        }

                        ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Handshake.from PC.Prority Tabletts Count", _TabletsCount);

                        
                        Task taskA = Task.Run(() =>
                        {
                            ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Produktionsmodus.Lager Rückführen.Start", true);
                        });
                        taskA.ContinueWith(async x =>
                        {
                            await Task.Delay(800);
                            ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Produktionsmodus.Lager Rückführen.Start", false);
                        }, TaskContinuationOptions.OnlyOnRanToCompletion);
                         
                    }
                    ApplicationService.SetView("DialogRegion", "EmptyView");
                }
                catch (Exception ex)
                {
                    new MessageBoxTask(ex, "* * * *", MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region - - - Methods - - -

        Order_Template GetSelectedOrder() 
        {
            foreach (Order_Template temp in Orders) 
            {
                if (temp.A.IsChecked == true)
                {
                    return temp;
                }
            }
            return null;
        }
        private ObservableCollection<long> OldOrders = new ObservableCollection<long>();
        public void GetOrders()
        {
            Orders.Clear();
            TabletsCount = 0;
            OldOrders = new ObservableCollection<long>();
            Task obTask = Task.Run(() => {

                short Paint_Id = (short) ApplicationService.GetVariableValue("NL.PLC.Blocks.2 Modul 2.05 Tauchbecken.DB LTB HMI.Actual value.Dipping Vat.Lacktyp");
                uint ActualOrder = (uint)ApplicationService.GetVariableValue("NLM4.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Produktionsmodus.Lager Rückführen.Aktuell bearbeitete Order ID");
                DataTable DT = (new LocalDBAdapter("SELECT Order_Id, Status " + 
                                                    "FROM Tray_Buffer " +
                                                    "WHERE Order_Id <> -1")).DB_Output();

                short DCisOn = (short)ApplicationService.GetVariableValue("NLM4.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Produktionsmodus.Doppeltakt.Status");

                int materials = 0;
                foreach (DataRow r in DT.Rows)
                {
                    VWRecipe VWR = new VWRecipe("TBStatus", r["Status"].ToString());
                    long run = Convert.ToInt64(VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Charge.Aktueller Durchlauf").ToArray()[0].Value);

                    DataTable temp = (new LocalDBAdapter("SELECT Paint_Id " +
                                                              "FROM Recipes_Coating " +
                                                              "WHERE Id = (SELECT C" + (run + 1).ToString() + "_Id FROM Recipes_MR WHERE Id = (SELECT MR_Id FROM Orders WHERE Id = " + r["Order_Id"] + "));")).DB_Output();
                    if (temp.Rows.Count > 0)
                    {
                        long Charge_Paint_Id = (long)temp.Rows[0]["Paint_Id"];
                        if (Charge_Paint_Id == Paint_Id)
                        {
                            materials++;
                        }
                    }
                    

                }
           
                if (DT.Rows.Count > 0)
                {
                    foreach (DataRow r in DT.Rows)
                    {
                        VWRecipe VWR = new VWRecipe("TBStatus", r["Status"].ToString());
                        long run = Convert.ToInt64(VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Charge.Aktueller Durchlauf").ToArray()[0].Value);
                        long setC = Convert.ToInt64(VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Charge.Beschichtungen.Soll").ToArray()[0].Value);
                        long actualC = Convert.ToInt64(VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Charge.Beschichtungen.Ist").ToArray()[0].Value);
                        if (run != 4)
                        {
                            DataTable temp = (new LocalDBAdapter("SELECT Paint_Id " +
                                                                 "FROM Recipes_Coating " +
                                                                 "WHERE Id = (SELECT C" + (run + 1).ToString() + "_Id FROM Recipes_MR WHERE Id = (SELECT MR_Id FROM Orders WHERE Id = " + r["Order_Id"] + "));")).DB_Output();

                            if (temp.Rows.Count > 0)
                            {
                                long Charge_Paint_Id = (long)temp.Rows[0]["Paint_Id"];
                                
                                if (DCisOn == 1)
                                {
                                    if (materials <= 34)
                                    {
                                        if (Charge_Paint_Id == Paint_Id)
                                        {
                                            if (OldOrders.Where(x => x == (long)r["Order_Id"]).ToArray().Length == 0)
                                            {
                                                temp = (new LocalDBAdapter("Select Orders.Id, Orders.Data_1, Orders.Data_2, Orders.Data_3, Orders.MR_Id, Recipes_MR.Name " +
                                                                            "From Orders " +
                                                                            "INNER JOIN Recipes_MR ON Orders.MR_Id = Recipes_MR.Id " +
                                                                            "WHERE Orders.Id =" + r["Order_Id"] + "; ")).DB_Output();
                                                if (temp.Rows.Count > 0)
                                                {
                                                    Dispatcher.BeginInvoke((Action)(() =>
                                                    {
                                                        Order_Template OT = new Order_Template()
                                                        {
                                                            O = new Order
                                                            {
                                                                Id = (long)temp.Rows[0]["Id"],
                                                                Data_1 = (string)temp.Rows[0]["Data_1"],
                                                                Data_2 = (string)temp.Rows[0]["Data_2"],
                                                                Data_3 = (string)temp.Rows[0]["Data_3"],
                                                                MR_Id = (long)temp.Rows[0]["MR_Id"],
                                                                MR = (string)temp.Rows[0]["Name"]
                                                            }
                                                        };

                                                        if (ActualOrder == OT.O.Id)
                                                        {
                                                            OT.A.IsChecked = true;
                                                        }
                                                        Orders.Add(OT);
                                                    }));
                                                }
                                                OldOrders.Add((long)r["Order_Id"]);
                                            }



                                            TabletsCount++;
                                        }
                                    }
                                    else 
                                    {
                                        if (Charge_Paint_Id == Paint_Id && (setC - actualC) == 1)
                                        {

                                            if (OldOrders.Where(x => x == (long)r["Order_Id"]).ToArray().Length == 0)
                                            {
                                                temp = (new LocalDBAdapter("Select Orders.Id, Orders.Data_1, Orders.Data_2, Orders.Data_3, Orders.MR_Id, Recipes_MR.Name " +
                                                                        "From Orders " +
                                                                        "INNER JOIN Recipes_MR ON Orders.MR_Id = Recipes_MR.Id " +
                                                                        "WHERE Orders.Id =" + r["Order_Id"] + "; ")).DB_Output();
                                                if (temp.Rows.Count > 0)
                                                {
                                                    Dispatcher.BeginInvoke((Action)(() =>
                                                    {
                                                        Order_Template OT = new Order_Template()
                                                        {
                                                            O = new Order
                                                            {
                                                                Id = (long)temp.Rows[0]["Id"],
                                                                Data_1 = (string)temp.Rows[0]["Data_1"],
                                                                Data_2 = (string)temp.Rows[0]["Data_2"],
                                                                Data_3 = (string)temp.Rows[0]["Data_3"],
                                                                MR_Id = (long)temp.Rows[0]["MR_Id"],
                                                                MR = (string)temp.Rows[0]["Name"]
                                                            }
                                                        };

                                                        if (ActualOrder == OT.O.Id)
                                                        {
                                                            OT.A.IsChecked = true;
                                                        }
                                                        Orders.Add(OT);
                                                    }));
                                                }
                                                OldOrders.Add((long)r["Order_Id"]);
                                            }

                                            TabletsCount++;
                                        }
                                    }
                                   
                                }
                                else 
                                {
                                    if (Charge_Paint_Id == Paint_Id)
                                    {
                                        if (OldOrders.Where(x => x == (long)r["Order_Id"]).ToArray().Length == 0)
                                        {
                                            temp = (new LocalDBAdapter("Select Orders.Id, Orders.Data_1, Orders.Data_2, Orders.Data_3, Orders.MR_Id, Recipes_MR.Name " +
                                                                        "From Orders " +
                                                                        "INNER JOIN Recipes_MR ON Orders.MR_Id = Recipes_MR.Id " +
                                                                        "WHERE Orders.Id =" + r["Order_Id"] + "; ")).DB_Output();
                                            if (temp.Rows.Count > 0)
                                            {
                                                Dispatcher.BeginInvoke((Action)(() =>
                                                {
                                                    Order_Template OT = new Order_Template()
                                                    {
                                                        O = new Order
                                                        {
                                                            Id = (long)temp.Rows[0]["Id"],
                                                            Data_1 = (string)temp.Rows[0]["Data_1"],
                                                            Data_2 = (string)temp.Rows[0]["Data_2"],
                                                            Data_3 = (string)temp.Rows[0]["Data_3"],
                                                            MR_Id = (long)temp.Rows[0]["MR_Id"],
                                                            MR = (string)temp.Rows[0]["Name"]
                                                        }
                                                    };

                                                    if (ActualOrder == OT.O.Id)
                                                    {
                                                        OT.A.IsChecked = true;
                                                    }
                                                    Orders.Add(OT);
                                                }));
                                            }
                                            OldOrders.Add((long)r["Order_Id"]);
                                        }
                                        
                                       

                                        TabletsCount++;
                                    }
                                }
                                
                            }
                        }

                    }
                }
            });
        }

       private void SetTrayBufferData()
        {
            IRegionService iRS = ApplicationService.GetService<IRegionService>();
            Task obTask = Task.Run(async () =>
            {
                DataTable DT = (new LocalDBAdapter("SELECT * " +
                                                 "FROM Tray_Buffer; ")).DB_Output();

                if (DT.Rows.Count > 0)
                {
                    foreach (DataRow dr in DT.Rows)
                    {
                        VWRecipe VWR = new VWRecipe("TBStatus", dr["Status"].ToString());
                        string d = VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Tablett.Function.Manuall QS").ToArray()[0].Value.ToString();
                        if (d != "0")
                        {
                            VWR.VWVariables[VWR.VWVariables.IndexOf(VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Tablett.Function.Manuall QS").ToArray()[0])].Value = 0;

                            var c = (new LocalDBAdapter("UPDATE Tray_Buffer " +
                                                       "SET Status = '" + VWR.GetXML() + "' " +
                                                       "WHERE Id = " + dr["Id"].ToString() + ";")).DB_Input();
                            await Dispatcher.InvokeAsync(delegate
                            {
                                MachineOverview.MO_TrayBuffer MO_TB = (MachineOverview.MO_TrayBuffer)iRS.GetView("MO_TrayBuffer");
                                MO_TB.GetTBTray(dr["Coord"].ToString()).UpdateTrayStatus();
                            });
                        }
                    }



                }

            });
                
        }

        #endregion
    }


}