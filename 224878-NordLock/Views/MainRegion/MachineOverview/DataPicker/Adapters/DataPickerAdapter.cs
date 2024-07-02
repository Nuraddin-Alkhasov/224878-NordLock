using HMI.Module;
using HMI.Views.MainRegion.Recipe;
using HMI.Views.MessageBoxRegion;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using VisiWin.ApplicationFramework;
using VisiWin.Commands;
using VisiWin.DataAccess;

namespace HMI.Views.MainRegion.MachineOverview
{
   
    [ExportAdapter("DataPickerAdapter")]
    public class DataPickerAdapter : AdapterBase
    {
        readonly IRegionService iRS = ApplicationService.GetService<IRegionService>();
        readonly IVariableService VS = ApplicationService.GetService<IVariableService>();
        IVariable LE;
        readonly string isEditable = "NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Handshake.to PC.isEditable";
        public DataPickerAdapter()
        {
            this.SelectMachineRecipe = new ActionCommand(this.SelectMachineRecipeCommandExecuted);
            this.UpdateData = new ActionCommand(this.UpdateDataCommandExecuted);
            this.CloseRecipeSelectorCommand = new ActionCommand(this.OnCloseRecipeSelectorCommandExecuted);
            this.CloseDataPickerCommand = new ActionCommand(this.OnCloseDataPickerCommandExecuted);
            
             
            LE = VS.GetVariable(isEditable);
            LE.Change += LE_Change;
        }

        private void LE_Change(object sender, VariableEventArgs e)
        {
            if ((bool)e.Value) { LoadEnabled = true; }
            else { LoadEnabled = false; }
        }

        #region - - - Properties - - -

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

        private ObservableCollection<MachineRecipe> machineRecipes = new ObservableCollection<MachineRecipe>();
        public ObservableCollection<MachineRecipe> MachineRecipes
        {
            get { return this.machineRecipes; }
            set
            {
                this.machineRecipes = value;
                this.OnPropertyChanged("MachineRecipes");
            }
        }

        private MachineRecipe selectedMachineRecipe = new MachineRecipe();
        public MachineRecipe SelectedMachineRecipe
        {
            get { return this.selectedMachineRecipe; }
            set
            {
                
                if (value != null)
                {
                    this.selectedMachineRecipe = value;
                    this.OnPropertyChanged("SelectedMachineRecipe");
                }
            }
        }

        bool loadEnabled;
        public bool LoadEnabled
        {
            get
            {
                return loadEnabled;
            }
            set
            {
                this.loadEnabled = value;
                this.OnPropertyChanged("LoadEnabled");
            }
        }

        bool editEnabled;
        public bool EditEnabled
        {
            get
            {
                return editEnabled;
            }
            set
            {
                this.editEnabled = value;
                this.OnPropertyChanged("EditEnabled");
            }
        }


        #endregion

        #region - - - Commands - - -

        public ICommand SelectMachineRecipe { get; set; }
        private void SelectMachineRecipeCommandExecuted(object parameter)
        {
            CurrentOrder.MR = SelectedMachineRecipe;
            CurrentOrder.User = ApplicationService.GetVariableValue("__CURRENT_USER.FULLNAME").ToString();
            this.OnPropertyChanged("CurrentOrder");
            OnCloseRecipeSelectorCommandExecuted(null);
        }

        public ICommand UpdateData { get; set; }
        private void UpdateDataCommandExecuted(object parameter)
        {
            if (CurrentOrder.Data_1.ToString().Length < 0 || +
                CurrentOrder.Data_2.ToString().Length < 0 || +
                CurrentOrder.Data_3.ToString().Length < 0 || +
                CurrentOrder.MR.Id == -1
                )
            {
                new MessageBoxTask("@Datapicker.Text7", "@Datapicker.Text9", MessageBoxIcon.Exclamation);
                return;
            }

            ;

            if (SetOrderId() == 0)
            {

                SetHeader();
            }
            else
            {
                ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Handshake.from PC.WrongSpinSpeed", true);
            }
                

           
            OnCloseDataPickerCommandExecuted(null);
        }

        public ICommand CloseRecipeSelectorCommand { get; set; }
        private void OnCloseRecipeSelectorCommandExecuted(object parameter)
        {
            ApplicationService.SetView("MessageBoxRegion", "EmptyView");
        }

        public ICommand CloseDataPickerCommand { get; set; }
        private void OnCloseDataPickerCommandExecuted(object parameter)
        {
            ApplicationService.SetView("DialogRegion", "EmptyView");
        }

        #endregion
      
 
        #region - - - Methods - - -
        public void UpdateMachineRecipeList(long _id)
        {
            Task obTask = Task.Run(() => {

                DataTable DT = (new LocalDBAdapter("SELECT * " +
                                                   "FROM Recipes_MR; ")).DB_Output();
                MachineRecipes = new ObservableCollection<MachineRecipe>();
                if (DT.Rows.Count > 0)
                {
                    foreach (DataRow r in DT.Rows)
                    {
                        MachineRecipes.Add(new MachineRecipe()
                        {
                            Id = (long)DT.Rows[0]["Id"],
                            Name = (string)DT.Rows[0]["Name"],
                            Description = (string)DT.Rows[0]["Description"],
                            Article = GetArticleData((long)DT.Rows[0]["Article_Id"]),
                            CoatingLayers = GetCoatingLayers(DT.Rows[0]),
                            Version = (string)DT.Rows[0]["Version"],
                            LastChanged = (DateTime)DT.Rows[0]["LastChanged"],
                            User = (string)DT.Rows[0]["User"]
                        });
                    }
                }
                UpdateSelectedOrder(_id);
            });
        }

        public void UpdateSelectedOrder(long _id)
        {
            Task obTask = Task.Run(() => {
                long O_Id = _id;
                if (_id == -1) 
                {
                    O_Id = (uint)ApplicationService.GetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Header.Order Id");
                    //O_Id = (long)Convert.ToUInt64(temp);
                }
               
                DataTable DT = (new LocalDBAdapter("SELECT Orders.Id, Orders.Data_1, Orders.Data_2, Orders.Data_3, Orders.MR_Id, Recipes_MR.Name, Recipes_MR.Description, Orders.User " +
                                                   "FROM Orders " +
                                                   "INNER JOIN Recipes_MR ON Orders.MR_Id = Recipes_MR.Id " +
                                                   "WHERE Orders.Id = " + O_Id + "; ")).DB_Output();

                if (DT.Rows.Count > 0)
                {
                    foreach (DataRow r in DT.Rows)
                    {
                        CurrentOrder = new Order()
                        {
                            Id = O_Id,
                            Data_1 = (string)r["Data_1"],
                            Data_2 = (string)r["Data_2"],
                            Data_3 = (string)r["Data_3"],
                            MR = MachineRecipes.Where(x => x.Id == (long)r["MR_Id"]).First(),
                            User = (string)r["User"]
                        };
                    }
                    SelectedMachineRecipe = CurrentOrder.MR;
                }
                else { CurrentOrder = new Order(); }
            });
        }

        public void CheckIfOrderAlreadyExist() 
        {
            DataTable DT = (new LocalDBAdapter("SELECT Id " +
                                                  "FROM Orders " +
                                                  "WHERE Data_2 = '" + CurrentOrder.Data_2 + "'; ")).DB_Output();
            if (DT.Rows.Count > 0)
            {
                EditEnabled = false;
                UpdateMachineRecipeList((long) DT.Rows[0]["Id"]);
            }
            else 
            {
                EditEnabled = true;
            }

        }

        int SetOrderId()
        {
            long weight = Convert.ToInt64(CurrentOrder.MR.Article.VWR.VWVariables.Where(x => (string)x.Item == "NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Station.Feeding.BS.Weight per basket").ToArray()[0].Value);
            double RPM = Math.Floor(0.05 * weight * weight - 6 * weight + 365);
            foreach (CoatingRecipe CR in CurrentOrder.MR.CoatingLayers) 
            {
                if (CR.Id != -1)
                {
                    foreach (CoatingStepRecipe CSR in CR.CoatingSteps)
                    {
                        if (CSR.Id != -1 && CSR.Type_Id == 2)
                        {
                            double RPM_2 = Convert.ToInt64(CSR.VWR.VWVariables.Where(x => (string)x.Item == "Recipe.CoatingStep.Schleudern.Rotor Drehzahl 2").ToArray()[0].Value);

                            if (RPM_2 > RPM)
                            {
                                return -1;
                            }
                        }
                    }
                }
            }


            DataTable DT = (new LocalDBAdapter("SELECT Id " +
                                               "FROM Orders " +
                                               "WHERE Data_2 ='" + CurrentOrder.Data_2 + "'; ")).DB_Output();

            if (DT.Rows.Count == 0)
            {
                bool result = (new LocalDBAdapter("INSERT INTO Orders " +
                                                  "(TimeStamp, Data_1, Data_2, Data_3, MR_Id, User) " +
                                                  "VALUES ('" + GetDataTimeNowToFormat() + "','" + CurrentOrder.Data_1 + "','" + CurrentOrder.Data_2 + "','" + CurrentOrder.Data_3 + "'," + CurrentOrder.MR.Id + ",'" + ApplicationService.GetVariableValue("__CURRENT_USER.FULLNAME").ToString() + "')")).DB_Input();
                DT = (new LocalDBAdapter("SELECT * " +
                                         "FROM Orders " +
                                         "WHERE Data_2 ='" + CurrentOrder.Data_2 + "'; ")).DB_Output();

            }

           
            uint Old_Order_Id = (uint)ApplicationService.GetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Header.Order Id");
            CurrentOrder.Id = (long)(DT.Rows[0]["Id"]);
            ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Header.Order Id", CurrentOrder.Id);

            short Box_Id = 1;
            if (Old_Order_Id == CurrentOrder.Id)
            {
                Box_Id = (short)(1 + (short)ApplicationService.GetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Handshake.from PC.BoxId"));
            }
            ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Handshake.to PC.NL Data.Order#STRING12", CurrentOrder.Data_1).ToString();
            ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Handshake.to PC.NL Data.Batch#STRING12", CurrentOrder.Data_2).ToString();
            ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Handshake.to PC.NL Data.Item#STRING12", CurrentOrder.Data_3).ToString();
            //ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Handshake.to PC.NL Data.Recipe number").ToString();
            ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Header.Order Id", CurrentOrder.Id);
            ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Handshake.from PC.BoxNumber", Box_Id);

            return 0;
        }

       

        void SetHeader()
        {
            ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Header.MR.Article Id", CurrentOrder.MR.Article.Id);
            for (int i = 1; i <= 4; i++)
            {
                ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Header.MR.CoatingLayer " + i + ".Paint Id", CurrentOrder.MR.CoatingLayers[i - 1].Paint_Id);
                ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Header.MR.CoatingLayer " + i + ".Recipe Id", CurrentOrder.MR.CoatingLayers[i - 1].Id);
            }
        }

        private string GetDataTimeNowToFormat()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        MachineRecipe GetMachineRecipe(long _id)
        {
            DataTable DT = (new LocalDBAdapter("SELECT MR_Id " +
                                                "FROM Orders " +
                                                "WHERE Id = " + _id + "; ")).DB_Output();

            if (DT.Rows.Count > 0)
            {
                long MR_Id = (long)DT.Rows[0]["MR_Id"];
                DT = (new LocalDBAdapter("SELECT * " +
                                         "FROM Recipes_MR " +
                                         "WHERE Id = " + MR_Id + "; ")).DB_Output();

                if (DT.Rows.Count > 0)
                {
                    return new MachineRecipe()
                    {
                        Id = (long)DT.Rows[0]["Id"],
                        Name = (string)DT.Rows[0]["Name"],
                        Description = (string)DT.Rows[0]["Description"],
                        Article = GetArticleData((long)DT.Rows[0]["Article_Id"]),
                        CoatingLayers = GetCoatingLayers(DT.Rows[0]),
                        Version = (string)DT.Rows[0]["Version"],
                        LastChanged = (DateTime)DT.Rows[0]["LastChanged"],
                        User = (string)DT.Rows[0]["User"]
                    };
                }
            }

            return new MachineRecipe();
        }
        ArticleRecipe GetArticleData(long A_Id)
        {
            if (A_Id != -1)
            {
                DataTable DT = (new LocalDBAdapter("SELECT Id, Name, Data " +
                                                   "FROM Recipes_Article_VW " +
                                                   "WHERE Id = " + A_Id + "; ")).DB_Output();
                if (DT.Rows.Count > 0)
                {
                    return new ArticleRecipe()
                    {
                        Id = (long)DT.Rows[0]["Id"],
                        Name = (string)DT.Rows[0]["Name"],
                        Data = (string)DT.Rows[0]["Data"]
                    };
                }
            }
            return new ArticleRecipe();
        }
        ObservableCollection<CoatingRecipe> GetCoatingLayers(DataRow _dr)
        {
            ObservableCollection<CoatingRecipe> CR = new ObservableCollection<CoatingRecipe>();
            for (int i = 1; i <= 10; i++)
            {
                if ((long)_dr["C" + i + "_Id"] != -1)
                {
                    CR.Add(GetCoatingData((long)_dr["C" + i + "_Id"]));
                }
                else { CR.Add(new CoatingRecipe() { Id = 0, Paint_Id = 0 }); }
            }
            return CR;
        }
        CoatingRecipe GetCoatingData(long _id)
        {
            if (_id != -1)
            {
                DataTable DT = (new LocalDBAdapter("SELECT Id, Name, Paint_Id " +
                                                  "FROM Recipes_Coating " +
                                                  "WHERE Id = " + _id + "; ")).DB_Output();

                if (DT.Rows.Count > 0)
                {
                    return new CoatingRecipe()
                    {
                        Id = (long)DT.Rows[0]["Id"],
                        Name = (string)DT.Rows[0]["Name"],
                        Paint_Id = (long)DT.Rows[0]["Paint_Id"]
                    };
                }
            }
            return new CoatingRecipe() { Id = 0, Paint_Id = 0 };
        }

        #endregion
    }


}