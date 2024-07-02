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
   
    [ExportAdapter("RecipeAdapter_MR")]
    public class RecipeAdapter_MR : AdapterBase, INotifyPropertyChanged
    {
        readonly IRegionService iRS = ApplicationService.GetService<IRegionService>();
        public RecipeAdapter_MR()
        {
            Task obTask = Task.Run(async () =>
            {
                AddRecipe("@Lists.CoatingLayers.Text0", new RecipeTemplateData() { Id = 0, Recipe = new CoatingRecipe() });
                await Task.Delay(300);
                AddAddBtn();
            });

            this.LoadFileToBuffer = new ActionCommand(this.LoadRecipeToBufferCommandExecuted);
            this.SaveFileCommand = new ActionCommand(this.SaveRecipeCommandExecuted);
            this.DeleteFileCommand = new ActionCommand(this.OnDeleteFileCommandExecuted);
            this.CloseCommand = new ActionCommand(this.OnCloseCommandExecuted);

            this.NewCommand = new ActionCommand(NewCommandExecuted);
            this.DeleteArticleCommand = new ActionCommand(DeleteArticleCommandExecuted);
            this.DeleteCoatingCommand = new ActionCommand(DeleteCoatingCommandExecuted);
           
            this.ArticleUp = new ActionCommand(ArticleUpCommandExecuted);
            this.ArticleDown = new ActionCommand(ArticleDownCommandExecuted);
            this.CoatingUp = new ActionCommand(CoatingUpCommandExecuted);
            this.CoatingDown = new ActionCommand(CoatingDownCommandExecuted);

            IsArticleChecked = true;
            ArticleBufferIsChecked = true;
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
        public MachineRecipe LastLoadedSavedMachineRecipe = new MachineRecipe();
        private MachineRecipe selectedMachineRecipe = new MachineRecipe();
        public MachineRecipe SelectedMachineRecipe
        {
            get { return this.selectedMachineRecipe; }
            set
            {
                this.selectedMachineRecipe = value;
                if (value != null)
                {
                    this.NameBuffer = value.Name;
                    this.DescriptionBuffer = value.Description;
                    this.OnPropertyChanged("SelectedMachineRecipe");
                }
            }
        }

        #endregion

        #region - - - Source - - -

        ObservableCollection<ArticleRecipe> articleRecipes = new ObservableCollection<ArticleRecipe>();
        public ObservableCollection<ArticleRecipe> ArticleRecipes
        {
            get { return this.articleRecipes; }
            set
            { 
                this.articleRecipes = value;
                if (value.Count >= 1) ArticleRecipeSelectedIndex = 0;
                this.OnPropertyChanged("ArticleRecipes");
            }
        }

        bool isArticleChecked = false;
        public bool IsArticleChecked
        {
            get { return this.isArticleChecked; }
            set
            {
                if (value)
                {
                    ArticleRecipeSelectedIndex = ArticleRecipes.Count > 1 ? 0 : -1;
                }
                else
                {
                    ArticleRecipeSelectedIndex = -1;
                }
                this.isArticleChecked = value;
                this.OnPropertyChanged("IsArticleChecked");
            }
        }

        int articleRecipeSelectedIndex;
        public int ArticleRecipeSelectedIndex
        {
            get { return this.articleRecipeSelectedIndex; }
            set
            {
                articleRecipeSelectedIndex = value;
                this.OnPropertyChanged("ArticleRecipeSelectedIndex");
            }
        }

        ObservableCollection<CoatingRecipe> coatingRecipes = new ObservableCollection<CoatingRecipe>();
        public ObservableCollection<CoatingRecipe> CoatingRecipes
        {
            get { return this.coatingRecipes; }
            set
            {
                this.coatingRecipes = value;
                this.OnPropertyChanged("CoatingRecipes");
            }
        }

        bool isCoatingChecked = false;
        public bool IsCoatingChecked
        {
            get { return this.isCoatingChecked; }
            set
            {
                if (value)
                {
                    CoatingRecipeSelectedIndex = CoatingRecipes.Count > 1 ? 0 : -1;
                }
                else
                {
                    CoatingRecipeSelectedIndex = -1;
                }
                this.isCoatingChecked = value;
                this.OnPropertyChanged("IsCoatingChecked");
            }
        }

        int coatingRecipeSelectedIndex = -1;
        public int CoatingRecipeSelectedIndex
        {
            get { return this.coatingRecipeSelectedIndex; }
            set
            {
                coatingRecipeSelectedIndex = value;
                this.OnPropertyChanged("CoatingRecipeSelectedIndex");
            }
        }

        #endregion

        #region - - - Destination - - -

        ArticleRecipe articleBuffer = new ArticleRecipe();
        public ArticleRecipe ArticleBuffer
        {
            get { return this.articleBuffer; }
            set
            {
                this.articleBuffer = value;
                this.OnPropertyChanged("ArticleBuffer");
            }
        }
        
        bool articleBufferIsChecked = false;
        public bool ArticleBufferIsChecked
        {
            get { return this.articleBufferIsChecked; }
            set
            {
                this.articleBufferIsChecked = value;
                this.OnPropertyChanged("ArticleBufferIsChecked");
            }
        }

        ObservableCollection<object> coatingLayers = new ObservableCollection<object>();
        public ObservableCollection<object> CoatingLayers
        {
            get { return this.coatingLayers; }
            set
            {
                this.coatingLayers = value;
                this.OnPropertyChanged("CoatingLayers");
            }
        }

        private object selectedCoatingLayer;
        public object SelectedCoatingLayer
        {
            get { return this.selectedCoatingLayer; }
            set
            {
                if (value != null)
                {
                    if (value.ToString() == "Recipe_Add")
                    {
                        selectedCoatingLayer= ((Recipe_Template)CoatingLayers[CoatingLayers.Count-2]);
                    }
                    this.selectedCoatingLayer = value;
                }
                else
                {
                    this.selectedCoatingLayer = value;
                }

                    this.OnPropertyChanged("SelectedCoatingLayer");
            }
        }

        #endregion

        #endregion

        #region - - - Commands - - -
        public ICommand LoadFileToBuffer { get; set; }
        private void LoadRecipeToBufferCommandExecuted(object parameter)
        {
            if (this.SelectedMachineRecipe.Id != -1)
            {
                Task obTask = Task.Run(async () =>
                {
                    int i = 0;
                    ArticleBuffer = SelectedMachineRecipe.Article;
                    CoatingLayers = new ObservableCollection<object>();
                    foreach (CoatingRecipe C in SelectedMachineRecipe.CoatingLayers)
                    {
                        if (C.Id != -1)
                        {
                            AddRecipe("@Lists.CoatingLayers.Text"+i, new RecipeTemplateData(i, C.Name, C.Description, C, C.Symbol));
                            await Task.Delay(300);
                            i++;
                        }
                    }

                    if (SelectedMachineRecipe.CoatingLayers[9].Id == -1)
                        AddAddBtn();
                   
                });
                UpdateName();
                LastLoadedSavedMachineRecipe = SelectedMachineRecipe;
                OnCloseCommandExecuted("False");
            }
        }
        
        public ICommand SaveFileCommand { get; set; }
        private void SaveRecipeCommandExecuted(object parameter)
        {
            if (ArticleBuffer.Id == -1) 
            {
                new MessageBoxTask("@RecipeSystem.Results.Text3", "@RecipeSystem.Results.SaveFile", MessageBoxIcon.Exclamation);
                OnCloseCommandExecuted(null);
                return;
            }
            if (NameBuffer.Length >= 2 && DescriptionBuffer.Length >= 2)
            {
                foreach (object RT in CoatingLayers)
                    switch (RT.ToString())
                    {
                        case "Recipe_Template":
                            if (((CoatingRecipe)((Recipe_Template)RT).RTD.Recipe).Id == -1)
                            {
                                new MessageBoxTask("@RecipeSystem.Results.Text3", "@RecipeSystem.Results.SaveFile", MessageBoxIcon.Exclamation);
                                OnCloseCommandExecuted(null);
                                return;
                            }
                            break;
                        default: break;
                    }

                long RecipeId = ((new RecipeManager(this.NameBuffer)).IsMRRecipeExisting());
                if (RecipeId != -1)
                {
                    if (MessageBoxView.Show("@RecipeSystem.Results.OverwriteFileQuery", "@RecipeSystem.Results.SaveFile", MessageBoxButton.YesNo, icon: MessageBoxIcon.Question) == MessageBoxResult.No)
                    {
                        ApplicationService.SetView("DialogRegion", "EmptyView");

                        return;
                    }
                }

                Task obTask = Task.Run(() =>
                {
                    if (RecipeId == -1)
                    {
                        string ColumnNames = ",Article_Id";
                        string Values = "," + ArticleBuffer.Id.ToString();
                        foreach (object cs in CoatingLayers)
                        {
                            if (cs.ToString() == "Recipe_Template")
                            {
                                ColumnNames += ", C" + (CoatingLayers.IndexOf(cs) + 1).ToString() + "_Id";
                                Values += ", " + ((CoatingRecipe)((Recipe_Template)cs).RTD.Recipe).Id.ToString();
                            }

                        }

                        (new LocalDBAdapter("INSERT " +
                                            "INTO Recipes_MR (Name, Description" + ColumnNames + ",LastChanged, User) " +
                                            "VALUES ('" + NameBuffer + "','" + DescriptionBuffer + "'" + Values + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + ApplicationService.GetVariableValue("__CURRENT_USER.FULLNAME").ToString() + "');")
                            ).DB_Input();

                    }
                    else
                    {
                        string UpdateData = ",Article_Id = " + ArticleBuffer.Id.ToString();

                        for (int i = 1; i <= 10; i++)
                        {
                            if (i <= CoatingLayers.Count)
                            {
                                if (CoatingLayers[i - 1].ToString() == "Recipe_Template")
                                {
                                    UpdateData += ", C" + i.ToString() + "_Id = " + ((CoatingRecipe)((Recipe_Template)CoatingLayers[i - 1]).RTD.Recipe).Id.ToString();
                                }
                                else
                                {
                                    UpdateData += ", C" + i.ToString() + "_Id = -1"; ;
                                }
                            }
                            else
                            {
                                UpdateData += ", C" + i.ToString() + "_Id = -1";
                            }

                        }

                        (new LocalDBAdapter("UPDATE Recipes_MR " +
                                            "SET Description = '" + DescriptionBuffer + "'" + UpdateData + ", LastChanged = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', User = '" + ApplicationService.GetVariableValue("__CURRENT_USER.FULLNAME").ToString() + "' " +
                                            "WHERE Id = " + SelectedMachineRecipe.Id + ";")
                        ).DB_Input();
                    }

                    UpdateMachineRecipeList(true);
                });
                UpdateName();
                OnCloseCommandExecuted("False");
            }
        }
        public ICommand DeleteFileCommand { get; set; }
        private void OnDeleteFileCommandExecuted(object parameter)
        {
            if (this.SelectedMachineRecipe.Id != -1)
            {
                //DataTable DT = (new LocalDBAdapter("SELECT * " +
                //                                   "FROM Orders " +
                //                                   "WHERE MR_Id = " + SelectedMachineRecipe.Id + "; ")).DB_Output();

                //if (DT.Rows.Count != 0)
                //{
                //    new MessageBoxTask("@RecipeSystem.Results.Text4", "@RecipeSystem.Results.DeleteFile", MessageBoxIcon.Exclamation);
                //}
                //else 
                //{
                    if (MessageBoxView.Show("@RecipeSystem.Results.DeleteFileQuery", "@RecipeSystem.Results.DeleteFile", MessageBoxButton.YesNo, icon: MessageBoxIcon.Question) == MessageBoxResult.Yes)
                    {
                        MachineRecipe RecipeToDelete = this.SelectedMachineRecipe;
                        Task obTask = Task.Run(() =>
                        {

                            (new LocalDBAdapter("DELETE " +
                                                "FROM Recipes_MR " +
                                                "WHERE Id = " + RecipeToDelete.Id + "; ")).DB_Output();

                            UpdateMachineRecipeList(false);                        
                        });
                        NameBuffer = "";
                        DescriptionBuffer = "";
                        LastLoadedSavedMachineRecipe = SelectedMachineRecipe = new MachineRecipe();
                        UpdateName();
                    }
             //   }
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
            if (CoatingLayers.Count == 4)
            {
                CoatingLayers.RemoveAt(3);
                AddRecipe("@Lists.CoatingLayers.Text" + CoatingLayers.Count, new RecipeTemplateData() { Id = 3, Recipe= new CoatingRecipe() });
            }
            else
            {
                AddRecipe("@Lists.CoatingLayers.Text" + (CoatingLayers.Count-1), new RecipeTemplateData() { Id = CoatingLayers.Count - 1, Recipe = new CoatingRecipe() });
            }

        }
        public ICommand DeleteArticleCommand { get; set; }
        private void DeleteArticleCommandExecuted(object parameter)
        {
            ArticleBuffer = new ArticleRecipe();
            ArticleBufferIsChecked = true;
            foreach (object rt in CoatingLayers)
            {
                if (rt.ToString() != "Recipe_Add")
                {
                    if (((Recipe_Template)rt).A.IsChecked == true)
                        ((Recipe_Template)rt).A.IsChecked = false;

                }
            }
        }
        public ICommand DeleteCoatingCommand { get; set; }
        private void DeleteCoatingCommandExecuted(object parameter)
        {
            int Id = ((Recipe_Template)parameter).RTD.Id;
            if (CoatingLayers.Count == 2 )
            {
                ((Recipe_Template)CoatingLayers[0]).RTD = new RecipeTemplateData(Id) { Recipe = new CoatingRecipe()};
            }
            else
            {
                CoatingLayers.Remove(parameter);
                if (CoatingLayers[CoatingLayers.Count - 1].ToString() != "Recipe_Add")
                {
                    AddAddBtn();
                }

                for (int i = 0; i < CoatingLayers.Count; i++)
                {
                    if (CoatingLayers[i].ToString() != "Recipe_Add") 
                    {
                        ((Recipe_Template)CoatingLayers[i]).RTD.Id = i;
                        ((Recipe_Template)CoatingLayers[i]).LocalizableLabelText = "@Lists.CoatingLayers.Text"+ i;
                       
                    }
                      
                }

                if (Id >= 1)
                    ((Recipe_Template)CoatingLayers[Id - 1]).A.IsChecked = true;
                else
                    ((Recipe_Template)CoatingLayers[0]).A.IsChecked = true;
            }
        }

        public ICommand ArticleUp { get; set; }
        private void ArticleUpCommandExecuted(object parameter)
        {
            if (!IsArticleChecked) IsArticleChecked = true;
            if (ArticleRecipeSelectedIndex < ArticleRecipes.Count - 1)
                ArticleRecipeSelectedIndex = ArticleRecipeSelectedIndex + 1;
        }
        public ICommand ArticleDown { get; set; }
        private void ArticleDownCommandExecuted(object parameter)
        {
            if (!IsArticleChecked) IsArticleChecked = true;
            if (ArticleRecipeSelectedIndex > 0)
                ArticleRecipeSelectedIndex = ArticleRecipeSelectedIndex - 1;
        }

        public ICommand CoatingUp { get; set; }
        private void CoatingUpCommandExecuted(object parameter)
        {
            if (!IsCoatingChecked) IsCoatingChecked = true;
            if (CoatingRecipeSelectedIndex < CoatingRecipes.Count - 1)
                CoatingRecipeSelectedIndex = CoatingRecipeSelectedIndex + 1;
        }
        public ICommand CoatingDown { get; set; }
        private void CoatingDownCommandExecuted(object parameter)
        {
            if (!IsCoatingChecked) IsCoatingChecked = true;
            if (CoatingRecipeSelectedIndex > 0)
                CoatingRecipeSelectedIndex = CoatingRecipeSelectedIndex - 1;
        }

        #endregion

        #region - - - Event Handlers - - -

        private void C_Checked(object sender, RoutedEventArgs e)
        {
             SelectedCoatingLayer = CoatingLayers[(((Recipe_Template)((RadioButton)sender).Parent).RTD.Id)];
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
        public void UpdateMachineRecipeList(bool setLastLoadedSavedMachineRecipe)
        {
            Task obTask = Task.Run(() => {
                DataTable DT = (new LocalDBAdapter("SELECT * " +
                                                     "FROM Recipes_MR; ")).DB_Output();
                var MR_Temp = new ObservableCollection<MachineRecipe>();
                if (DT.Rows.Count > 0)
                {
                    foreach (DataRow r in DT.Rows)
                    {
                        ObservableCollection<CoatingRecipe> temp = new ObservableCollection<CoatingRecipe>();
                        for (int i = 1; i <= 10; i++)
                        {
                            temp.Add(((long)r["C" + i.ToString() + "_Id"]) != -1 ? CoatingRecipes.Where(x => x.Id == (long)r["C" + i.ToString() + "_Id"]).First() : new CoatingRecipe());
                        }

                        MR_Temp.Add(new MachineRecipe()
                        {
                            Id = (long)r["Id"],
                            Name = (string)r["Name"],
                            Description = (string)r["Description"],
                            Article = (((long)r["Article_Id"]) != -1 ? ArticleRecipes.Where(x => x.Id == (long)r["Article_Id"]).First() : new ArticleRecipe()),
                            CoatingLayers = temp,
                            Version = (string)r["Version"],
                            LastChanged = (DateTime)r["LastChanged"],
                            User = (string)r["User"]
                        });
                    }
                }

                MachineRecipes = MR_Temp;

                if (setLastLoadedSavedMachineRecipe)
                    LastLoadedSavedMachineRecipe = MachineRecipes.Where(x => x.Name == NameBuffer).First();
            });
        }
        private void UpdateName()
        {
            ((Recipe_PN)iRS.GetView("Recipe_PN")).Rname.Value = NameBuffer;
            ((Recipe_PN)iRS.GetView("Recipe_PN")).Descr.Value = DescriptionBuffer;
        }
        private void AddRecipe(string _header, RecipeTemplateData _rtd) 
        {
            Task T = Task.Run(() =>
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    CoatingLayers.Insert(_rtd.Id, new Recipe_Template()
                    {
                        RTD = _rtd,
                        LocalizableLabelText = _header,
                    });
                    ((Recipe_Template)CoatingLayers[_rtd.Id])._del.Command = DeleteCoatingCommand;
                    ((Recipe_Template)CoatingLayers[_rtd.Id])._del.CommandParameter = CoatingLayers[_rtd.Id];
                    ((Recipe_Template)CoatingLayers[_rtd.Id]).A.GroupName = "DG_Rec";
                    ((Recipe_Template)CoatingLayers[_rtd.Id]).A.Checked += C_Checked;

                    Recipe_Machine temp = (Recipe_Machine)iRS.GetView("Recipe_Machine");
                    temp.SV.ScrollIntoView(CoatingLayers[CoatingLayers.Count - 1]);

                    ArticleBufferIsChecked = false;
                    foreach (object rt in CoatingLayers)
                    {
                        if (rt.ToString() != "Recipe_Add")
                        {
                            if (((Recipe_Template)rt).A.IsChecked == true)
                                ((Recipe_Template)rt).A.IsChecked = false;

                        }
                    }

                    ((Recipe_Template)CoatingLayers[_rtd.Id]).A.IsChecked = true;

                }));
            });
        }

        private void AddAddBtn()
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                CoatingLayers.Add(new Recipe_Add());
                ((Recipe_Add)CoatingLayers[CoatingLayers.Count - 1]).Addbtn.Command = NewCommand;
            }));
        }

        #endregion
       
    }


}