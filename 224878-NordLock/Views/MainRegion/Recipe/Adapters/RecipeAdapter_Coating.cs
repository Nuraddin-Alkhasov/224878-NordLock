using HMI.Module;
using HMI.UserControls;
using HMI.Views.MessageBoxRegion;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisiWin.ApplicationFramework;
using VisiWin.Commands;

namespace HMI.Views.MainRegion.Recipe
{

    [ExportAdapter("RecipeAdapter_Coating")]
    public class RecipeAdapter_Coating : AdapterBase, INotifyPropertyChanged
    {
        readonly IRegionService iRS = ApplicationService.GetService<IRegionService>();
        public RecipeAdapter_Coating()
        {
            Task obTask = Task.Run(async () =>
            {
                AddRecipe("@RecipeSystem.Text7", new RecipeTemplateData() { Id = 0, Recipe = new CoatingStepRecipe() });
                await Task.Delay(300);
                AddAddBtn();
            });

            this.LoadFileToBuffer = new ActionCommand(this.LoadRecipeToBufferCommandExecuted);
            this.SaveFileCommand = new ActionCommand(this.SaveRecipeCommandExecuted);
            this.DeleteFileCommand = new ActionCommand(this.OnDeleteFileCommandExecuted);
            this.CloseCommand = new ActionCommand(this.OnCloseCommandExecuted);

            this.NewCommand = new ActionCommand(NewCommandExecuted);
            this.DeleteCommand = new ActionCommand(DeleteCommandExecuted);

            this.DipUp = new ActionCommand(DipUpCommandExecuted);
            this.DipDown = new ActionCommand(DipDownCommandExecuted);
            this.SpinUp = new ActionCommand(SpinUpCommandExecuted);
            this.SpinDown = new ActionCommand(SpinDownCommandExecuted);
            this.TiltUp = new ActionCommand(TiltUpCommandExecuted);
            this.TiltDown = new ActionCommand(TiltDownCommandExecuted);

            IsDipChecked = true;
        }

        #region - - - Properties - - -

        #region - - - Recipe Browser - - -

        private string nameBuffer = "";
        public string NameBuffer
        {
            get { return this.nameBuffer; }
            set
            {
                this.nameBuffer = value;
                this.OnPropertyChanged("NameBuffer");
            }
        }

        private string descriptionBuffer = "";
        public string DescriptionBuffer
        {
            get { return this.descriptionBuffer; }
            set
            {
                this.descriptionBuffer = value;
                this.OnPropertyChanged("DescriptionBuffer");
            }
        }

        ObservableCollection<CoatingRecipe> coatingRecipes = new ObservableCollection<CoatingRecipe>();
        public ObservableCollection<CoatingRecipe> CoatingRecipes
        {
            get { return this.coatingRecipes; }
            set
            {
                this.coatingRecipes = value;
                UpdateCoatingRecipeLists();
                this.OnPropertyChanged("CoatingRecipes");
            }
        }

        public CoatingRecipe LastLoadedSavedCoatingRecipe = new CoatingRecipe();
        private CoatingRecipe selectedCoatingRecipe = new CoatingRecipe();
        public CoatingRecipe SelectedCoatingRecipe
        {
            get { return this.selectedCoatingRecipe; }
            set
            {
                this.selectedCoatingRecipe = value;
                if (value != null)
                {
                    this.NameBuffer = value.Name;
                    this.DescriptionBuffer = value.Description;
                    this.OnPropertyChanged("SelectedCoatingRecipe");
                }
            }
        }

        #endregion


        #region - - - Source - - -

        ObservableCollection<CoatingStepRecipe> coatingStepRecipes = new ObservableCollection<CoatingStepRecipe>();
        public ObservableCollection<CoatingStepRecipe> CoatingStepRecipes
        {
            get { return this.coatingStepRecipes; }
            set
            {
                if (value != null)
                {
                    this.coatingStepRecipes = value;
                    DipRecipes = new ObservableCollection<CoatingStepRecipe>(CoatingStepRecipes.Where(x => x.Type_Id == 1));
                    SpinRecipes = new ObservableCollection<CoatingStepRecipe>(CoatingStepRecipes.Where(x => x.Type_Id == 2));
                    TiltRecipes = new ObservableCollection<CoatingStepRecipe>(CoatingStepRecipes.Where(x => x.Type_Id == 3));

                    UpdateCoatingRecipeList(false);
                }
            }
        }

        ObservableCollection<CoatingStepRecipe> dipRecipes = new ObservableCollection<CoatingStepRecipe>();
        public ObservableCollection<CoatingStepRecipe> DipRecipes
        {
            get { return this.dipRecipes; }
            set
            {
                if (value != null)
                {
                    this.dipRecipes = value;
                    if (value.Count >= 1) DipRecipeSelectedIndex = 0;
                    this.OnPropertyChanged("DipRecipes");
                }
            }
        }

        bool isDipChecked = false;
        public bool IsDipChecked
        {
            get { return this.isDipChecked; }
            set
            {
                if (value)
                {
                    DipRecipeSelectedIndex = DipRecipes.Count > 1 ? 0 : -1;
                }
                else
                {
                    DipRecipeSelectedIndex = -1;
                }
                this.isDipChecked = value;
                this.OnPropertyChanged("IsDipChecked");
            }
        }

        int dipRecipeSelectedIndex;
        public int DipRecipeSelectedIndex
        {
            get { return this.dipRecipeSelectedIndex; }
            set
            {
                dipRecipeSelectedIndex = value;
                this.OnPropertyChanged("DipRecipeSelectedIndex");
            }
        }

        ObservableCollection<CoatingStepRecipe> spinRecipes = new ObservableCollection<CoatingStepRecipe>();
        public ObservableCollection<CoatingStepRecipe> SpinRecipes
        {
            get { return this.spinRecipes; }
            set
            {
                if (value != null)
                {
                    this.spinRecipes = value;
                    this.OnPropertyChanged("SpinRecipes");
                }
            }
        }

        bool isSpinChecked = false;
        public bool IsSpinChecked
        {
            get { return this.isSpinChecked; }
            set
            {
                if (value)
                {
                    SpinRecipeSelectedIndex = SpinRecipes.Count > 1 ? 0 : -1;
                }
                else
                {
                    SpinRecipeSelectedIndex = -1;
                }
                this.isSpinChecked = value;
                this.OnPropertyChanged("IsSpinChecked");
            }
        }

        int spinRecipeSelectedIndex = -1;
        public int SpinRecipeSelectedIndex
        {
            get { return this.spinRecipeSelectedIndex; }
            set
            {
                spinRecipeSelectedIndex = value;
                this.OnPropertyChanged("SpinRecipeSelectedIndex");
            }
        }

        ObservableCollection<CoatingStepRecipe> tiltRecipes = new ObservableCollection<CoatingStepRecipe>();
        public ObservableCollection<CoatingStepRecipe> TiltRecipes
        {
            get { return this.tiltRecipes; }
            set
            {
                if (value != null)
                {
                    this.tiltRecipes = value;
                    this.OnPropertyChanged("TiltRecipes");
                }
            }
        }

        bool isTiltChecked = false;
        public bool IsTiltChecked
        {
            get { return this.isTiltChecked; }
            set
            {
                if (value)
                {
                    TiltRecipeSelectedIndex = TiltRecipes.Count > 1 ? 0 : -1;
                }
                else
                {
                    TiltRecipeSelectedIndex = -1;
                }
                this.isTiltChecked = value;
                this.OnPropertyChanged("IsTiltChecked");
            }
        }

        int tiltRecipeSelectedIndex = -1;
        public int TiltRecipeSelectedIndex
        {
            get { return this.tiltRecipeSelectedIndex; }
            set
            {
                tiltRecipeSelectedIndex = value;
                this.OnPropertyChanged("TiltRecipeSelectedIndex");
            }
        }

        #endregion

        #region - - - Destination - - -

        ObservableCollection<string> paintTypes = new ObservableCollection<string>();
        public ObservableCollection<string> PaintTypes
        {
            get { return this.paintTypes; }
            set
            {
                if (value != null)
                {
                    this.paintTypes = value;
                    this.OnPropertyChanged("PaintTypes");
                }
            }
        }

        long paintTypeSelectedIndex = -1;
        public long PaintTypeSelectedIndex
        {
            get { return this.paintTypeSelectedIndex; }
            set
            {
                paintTypeSelectedIndex = value;
                this.OnPropertyChanged("PaintTypeSelectedIndex");
            }
        }

        ObservableCollection<object> coatingSteps = new ObservableCollection<object>();
        public ObservableCollection<object> CoatingSteps
        {
            get { return this.coatingSteps; }
            set
            {
                this.coatingSteps = value;

                this.OnPropertyChanged("CoatingSteps");
            }
        }

        private object selectedCoatingStep;
        public object SelectedCoatingStep
        {
            get { return this.selectedCoatingStep; }
            set
            {
                if (value != null)
                {
                    if (value.ToString() == "Recipe_Add")
                    {
                        selectedCoatingStep = ((Recipe_Template)CoatingSteps[CoatingSteps.Count - 2]);//.A.IsChecked = true;
                    }
                    else 
                    {
                       selectedCoatingStep = value;
                    }
                }
                else 
                {
                    selectedCoatingStep = value;
                }
                this.OnPropertyChanged("SelectedCoatingStep");
            }
        }

        #endregion


        #endregion

        #region - - - Commands - - -
        public void LoadRecipeToBuffer(CoatingRecipe CR)
        {
            if (CR.Id != -1)
            {
                SelectedCoatingRecipe = CR;
                LoadRecipeToBufferCommandExecuted(null);
            }
        }

        public ICommand LoadFileToBuffer { get; set; }
        private void LoadRecipeToBufferCommandExecuted(object parameter)
        {
            if (this.SelectedCoatingRecipe.Id != -1)
            {
                Task obTask = Task.Run(async () => {
                    int i = 0;
                    CoatingSteps = new ObservableCollection<object>();
                    foreach (CoatingStepRecipe CSR in SelectedCoatingRecipe.CoatingSteps) 
                    {
                        PaintTypeSelectedIndex = SelectedCoatingRecipe.Paint_Id;
                        if (CSR.Id != -1) 
                        {
                            AddRecipe("@RecipeSystem.Text7", new RecipeTemplateData(i,CSR.Name, CSR.VWR.Description, CSR, CSR.Symbol));
                            await Task.Delay(300);
                            i++;
                        }  
                    }
                    
                    if(SelectedCoatingRecipe.CoatingSteps[7].Id == -1)
                        AddAddBtn();
                });
                UpdateName();
                LastLoadedSavedCoatingRecipe = SelectedCoatingRecipe;
                OnCloseCommandExecuted("False");
            }
        }
        public ICommand SaveFileCommand { get; set; }
        private void SaveRecipeCommandExecuted(object parameter)
        {
            
            if (PaintTypeSelectedIndex == -1)
            {
                new MessageBoxTask("@RecipeSystem.Results.Text3", "@RecipeSystem.Results.SaveFile", MessageBoxIcon.Exclamation);
                OnCloseCommandExecuted(null);
                return;
            }
            foreach (object CS in CoatingSteps)
                switch (CS.ToString())
                {
                    case "Recipe_Template":
                        if (((CoatingStepRecipe)((Recipe_Template)CS).RTD.Recipe).Id == -1)
                        {
                            new MessageBoxTask("@RecipeSystem.Results.Text3", "@RecipeSystem.Results.SaveFile", MessageBoxIcon.Exclamation);
                            OnCloseCommandExecuted(null);
                            return;
                        }
                        break;
                    default: break;
                }


            long RecipeId = ((new RecipeManager(this.NameBuffer)).IsCoatingRecipeExisting());
            if (RecipeId != -1)
            {
                if (MessageBoxView.Show("@RecipeSystem.Results.OverwriteFileQuery", "@RecipeSystem.Results.SaveFile", MessageBoxButton.YesNo, icon: MessageBoxIcon.Question) == MessageBoxResult.No)
                {
                    ApplicationService.SetView("DialogRegion", "EmptyView");
                    return;
                }
            }
            if (NameBuffer.Length >= 2 && DescriptionBuffer.Length >= 2)
            {
                Task obTask = Task.Run(() =>
                {
                    if (RecipeId == -1)
                    {
                        string ColumnNames = "";
                        string Values = "";
                        foreach (object cs in CoatingSteps)
                        {
                            if (cs.ToString() == "Recipe_Template") 
                            {
                                ColumnNames += ", S" + (CoatingSteps.IndexOf(cs)+1).ToString() + "_Id";
                                Values += ", " + ((CoatingStepRecipe)((Recipe_Template)cs).RTD.Recipe).Id.ToString();
                            }
                      
                        }

                        (new LocalDBAdapter("INSERT " +
                                            "INTO Recipes_Coating (Name, Description, Paint_Id"+ ColumnNames + ",LastChanged, User) " +
                                            "VALUES ('" + NameBuffer + "','" + DescriptionBuffer + "'," + PaintTypeSelectedIndex + Values + ",'"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +"','"+ ApplicationService.GetVariableValue("__CURRENT_USER.FULLNAME").ToString() + "');")
                            ).DB_Input();

                    }
                    else
                    {
                        string UpdateData = "";
                        
                        for (int i = 1; i <= 8; i++)
                        {
                            if (i <= CoatingSteps.Count)
                            {
                                if (CoatingSteps[i-1].ToString() == "Recipe_Template")
                                {
                                    UpdateData += ", S" + i.ToString() + "_Id = " + ((CoatingStepRecipe)((Recipe_Template)CoatingSteps[i - 1]).RTD.Recipe).Id.ToString();
                                }
                                else
                                {
                                    UpdateData += ", S" + i.ToString() + "_Id = -1"; ;
                                }
                            }
                            else 
                            {
                                UpdateData += ", S" + i.ToString() + "_Id = -1";
                            }
                                
                        }

                        (new LocalDBAdapter("UPDATE Recipes_Coating " +
                                            "SET Description = '" + DescriptionBuffer + "', Paint_Id = "+ PaintTypeSelectedIndex + UpdateData  + ", LastChanged = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', User = '"+ ApplicationService.GetVariableValue("__CURRENT_USER.FULLNAME").ToString() + "' " +
                                            "WHERE Id = " + SelectedCoatingRecipe.Id + ";")
                        ).DB_Input();
                    }

                    UpdateCoatingRecipeList(true);
                  
                });
                UpdateName();
                OnCloseCommandExecuted("False");
            }
        }
        public ICommand DeleteFileCommand { get; set; }
        private void OnDeleteFileCommandExecuted(object parameter)
        {
            if (this.SelectedCoatingRecipe.Id != -1)
            {
                //DataTable DT = (new LocalDBAdapter("SELECT * " +
                //                                     "FROM Recipes_MR " +
                //                                     "WHERE C1_Id = " + SelectedCoatingRecipe.Id +
                //                                     " OR C2_Id = " + SelectedCoatingRecipe.Id +
                //                                     " OR C3_Id = " + SelectedCoatingRecipe.Id +
                //                                     " OR C4_Id = " + SelectedCoatingRecipe.Id + "; ")).DB_Output();

                //if (DT.Rows.Count != 0)
                //{
                //    new MessageBoxTask("@RecipeSystem.Results.Text5", "@RecipeSystem.Results.DeleteFile", MessageBoxIcon.Exclamation);
                //}
                //else
                //{
                    if (MessageBoxView.Show("@RecipeSystem.Results.DeleteFileQuery", "@RecipeSystem.Results.DeleteFile", MessageBoxButton.YesNo, icon: MessageBoxIcon.Question) == MessageBoxResult.Yes)
                    {
                        CoatingRecipe RecipeToDelete = this.SelectedCoatingRecipe;
                        Task obTask = Task.Run(() =>
                        {

                            (new LocalDBAdapter("DELETE " +
                                                "FROM Recipes_Coating " +
                                                "WHERE ID = " + RecipeToDelete.Id + "; ")).DB_Output();

                            UpdateCoatingRecipeList(false);
                        });
                        NameBuffer = "";
                        DescriptionBuffer = "";
                        LastLoadedSavedCoatingRecipe = SelectedCoatingRecipe = new CoatingRecipe();
                        UpdateName();
                    }
               // }
            }
        }

        public ICommand CloseCommand { get; set; }
        private void OnCloseCommandExecuted(object parameter)
        {
            ApplicationService.SetView("DialogRegion", "EmptyView");
        }

        public ICommand NewCommand { get; set; }
        private void NewCommandExecuted(object parameter)
        {
            if (CoatingSteps.Count == 8)
            {
                CoatingSteps.RemoveAt(7);
                AddRecipe("@RecipeSystem.Text7", new RecipeTemplateData() { Id=7});
            }
            else
            {
                AddRecipe("@RecipeSystem.Text7", new RecipeTemplateData() { Id = CoatingSteps.Count - 1 });
            }           
        }

        public ICommand DeleteCommand { get; set; }
        private void DeleteCommandExecuted(object parameter)
        {
            int Id = ((Recipe_Template)parameter).RTD.Id;
            if (CoatingSteps.Count == 2)
            {
                ((Recipe_Template)CoatingSteps[0]).RTD = new RecipeTemplateData() { Recipe = new CoatingStepRecipe()};
            }
            else 
            {
                CoatingSteps.Remove(parameter);
                if (CoatingSteps[CoatingSteps.Count - 1].ToString() != "Recipe_Add")
                {
                    AddAddBtn();
                }

                for (int i = 0; i < CoatingSteps.Count; i++) 
                {
                    if (CoatingSteps[i].ToString() != "Recipe_Add")
                        ((Recipe_Template)CoatingSteps[i]).RTD.Id = i;
                }
            }

            if(Id>=1)
                ((Recipe_Template)CoatingSteps[Id - 1]).A.IsChecked = true;
            else
                ((Recipe_Template)CoatingSteps[0]).A.IsChecked = true;
        }

        public ICommand DipUp { get; set; }
        private void DipUpCommandExecuted(object parameter)
        {
            if(!IsDipChecked) IsDipChecked = true;
            if (DipRecipeSelectedIndex < DipRecipes.Count - 1 )
                DipRecipeSelectedIndex = DipRecipeSelectedIndex + 1;
        }

        public ICommand DipDown { get; set; }
        private void DipDownCommandExecuted(object parameter)
        {
            if (!IsDipChecked) IsDipChecked = true;
            if (DipRecipeSelectedIndex > 0 )
                DipRecipeSelectedIndex = DipRecipeSelectedIndex - 1;
        }

        public ICommand SpinUp { get; set; }
        private void SpinUpCommandExecuted(object parameter)
        {
            if (!IsSpinChecked) IsSpinChecked = true;
            if (SpinRecipeSelectedIndex < SpinRecipes.Count - 1 )
                SpinRecipeSelectedIndex = SpinRecipeSelectedIndex + 1;
        }
        public ICommand SpinDown { get; set; }
        private void SpinDownCommandExecuted(object parameter)
        {
            if (!IsSpinChecked) IsSpinChecked = true;
            if (SpinRecipeSelectedIndex > 0)
                SpinRecipeSelectedIndex = SpinRecipeSelectedIndex - 1;
        }
        public ICommand TiltUp { get; set; }
        private void TiltUpCommandExecuted(object parameter)
        {
            if (!IsTiltChecked) IsTiltChecked = true;
            if (TiltRecipeSelectedIndex < TiltRecipes.Count - 1)
                TiltRecipeSelectedIndex = TiltRecipeSelectedIndex + 1;
        }
        public ICommand TiltDown { get; set; }
        private void TiltDownCommandExecuted(object parameter)
        {
            if (!IsTiltChecked) IsTiltChecked = true;
            if (TiltRecipeSelectedIndex > 0)
                TiltRecipeSelectedIndex = TiltRecipeSelectedIndex - 1;
        }

        #endregion

        #region - - - Event Handlers - - -

        private void A_Checked(object sender, RoutedEventArgs e)
        {
            SelectedCoatingStep = CoatingSteps[(((Recipe_Template)((RadioButton)sender).Parent).RTD.Id)];
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region - - - Methods - - -
        public void UpdateCoatingRecipeList(bool setLastLoadedSavedCoatingRecipe)
        {
            Task obTask = Task.Run(() => {
                DataTable DT = (new LocalDBAdapter("SELECT * " +
                                                     "FROM Recipes_Coating; ")).DB_Output();

                if (DT.Rows.Count > 0)
                {
                    var CR_Temp = new ObservableCollection<CoatingRecipe>();
                    foreach (DataRow r in DT.Rows)
                    {
                        ObservableCollection<CoatingStepRecipe> temp = new ObservableCollection<CoatingStepRecipe>();
                        for (int i = 1; i <= 8; i++) 
                        {
                            temp.Add(((long)r["S" + i.ToString()+"_Id"]) != -1 ? CoatingStepRecipes.Where(x => x.Id == (long)r["S" + i.ToString() + "_Id"]).First() : new CoatingStepRecipe());
                        }

                        CR_Temp.Add(new CoatingRecipe()
                        {
                            Id = (long)r["Id"],
                            Name = (string)r["Name"],
                            Description = (string)r["Description"],
                            Paint_Id = (long)r["Paint_Id"],
                            CoatingSteps = temp,
                            Version= (string)r["Version"],
                            LastChanged = (DateTime)r["LastChanged"],
                            User = (string)r["User"]
                        });
                    }

                    CoatingRecipes = CR_Temp;

                    if (setLastLoadedSavedCoatingRecipe)
                        LastLoadedSavedCoatingRecipe = CoatingRecipes.Where(x => x.Name == NameBuffer).First();
                   
                }
            });
        }

        private void UpdateName()
        {
            ((Recipe_PN)iRS.GetView("Recipe_PN")).Rname.Value = NameBuffer;
            ((Recipe_PN)iRS.GetView("Recipe_PN")).Descr.Value = DescriptionBuffer;
        }
        private void UpdateCoatingRecipeLists()
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                RecipeAdapter_MR adapter = (RecipeAdapter_MR)((Recipe_Machine)iRS.GetView("Recipe_Machine")).DataContext;
                adapter.CoatingRecipes = CoatingRecipes;
            }));
        }
        public void UpdatePaintTypes() 
        {
            PaintTypes.Clear();
            PaintTypes.Add("  ");
            for (int i = 1; i <= 10; i++) 
            {
                PaintTypes.Add(ApplicationService.GetVariableValue("NL.PLC.Blocks.50 HMI.00 Allgemein.Lacktyp Namen.Lacktyp Name[" + i + "]").ToString());
            }
            PaintTypeSelectedIndex = SelectedCoatingRecipe.Paint_Id;
        }

        private void AddRecipe(string _header, RecipeTemplateData _rtd)
        {
            Task T = Task.Run(() =>
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                CoatingSteps.Insert(_rtd.Id, new Recipe_Template()
                {
                    RTD = _rtd,
                    LocalizableLabelText = _header,
                });

                ((Recipe_Template)CoatingSteps[_rtd.Id])._del.Command = DeleteCommand;
                ((Recipe_Template)CoatingSteps[_rtd.Id])._del.CommandParameter = CoatingSteps[_rtd.Id];
                ((Recipe_Template)CoatingSteps[_rtd.Id]).A.GroupName = "DG_CoatingSteps";
                ((Recipe_Template)CoatingSteps[_rtd.Id]).A.Checked += A_Checked;

                Recipe_Coating_PR temp = (Recipe_Coating_PR)iRS.GetView("Recipe_Coating_PR");
                temp.SV.ScrollIntoView(CoatingSteps[CoatingSteps.Count - 1]);

                foreach (object rt in CoatingSteps)
                {
                    if (rt.ToString() != "Recipe_Add")
                    {
                        if (((Recipe_Template)rt).A.IsChecked == true)
                            ((Recipe_Template)rt).A.IsChecked = false;
                        
                        }
                    }

                    ((Recipe_Template)CoatingSteps[_rtd.Id]).A.IsChecked = true;
                }));
            });
        }
        private void AddAddBtn()
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                CoatingSteps.Add(new Recipe_Add());
                ((Recipe_Add)CoatingSteps[CoatingSteps.Count - 1]).Addbtn.Command = NewCommand;
            }));
        }

        #endregion
    }


}