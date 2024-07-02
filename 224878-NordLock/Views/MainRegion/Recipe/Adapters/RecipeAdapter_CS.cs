using HMI.Module;
using HMI.Views.MessageBoxRegion;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VisiWin.ApplicationFramework;
using VisiWin.Commands;
using VisiWin.Recipe;

namespace HMI.Views.MainRegion.Recipe
{

    [ExportAdapter("RecipeAdapter_CS")]
    public class RecipeAdapter_CS : AdapterBase, INotifyPropertyChanged
    {
        readonly int Class_Id = 0;
        long SelectedRecipeType = 1;
        readonly IRegionService iRS = ApplicationService.GetService<IRegionService>();

        public RecipeAdapter_CS() 
        {
            IsDipChecked = true;
            UpdateCoatingStepRecipeList(false);
            
            this.LoadFileToBuffer = new ActionCommand(this.LoadRecipeToBufferCommandExecuted);
            this.SaveFileCommand = new ActionCommand(this.SaveRecipeCommandExecuted);
            this.DeleteFileCommand = new ActionCommand(this.OnDeleteFileCommandExecuted);
            this.CloseCommand = new ActionCommand(this.OnCloseCommandExecuted);
        }

        #region - - - Properties - - -

        ObservableCollection<CoatingStepRecipe> coatingStepRecipes = new ObservableCollection<CoatingStepRecipe>();
        public ObservableCollection<CoatingStepRecipe> CoatingStepRecipes
        {
            get { return this.coatingStepRecipes; }
            set
            {
                if (value != null)
                {
                    this.coatingStepRecipes = value;
                    UpdateCoatingStepRecipeLists();
                    this.OnPropertyChanged("CoatingStepRecipes");
                }
            }
        }

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

        public CoatingStepRecipe LastLoadedSavedCoatingStepRecipe = new CoatingStepRecipe();
        private CoatingStepRecipe selectedCoatingStepRecipe = new CoatingStepRecipe();
        public CoatingStepRecipe SelectedCoatingStepRecipe
        {
            get { return this.selectedCoatingStepRecipe; }
            set
            {
                if (value != null)
                {
                    this.selectedCoatingStepRecipe = value;
                    this.NameBuffer = value.Name;
                    this.DescriptionBuffer = value.VWR.Description;
                    this.OnPropertyChanged("SelectedCoatingStepRecipe");
                }   
            }        
        }
        object content = new Recipe_Coating_D();
        public object Content
        {
            get { return content; }
            set
            {
                this.content = value;
                this.OnPropertyChanged("Content");
            }
        }

        bool isDipChecked = true;
        public bool IsDipChecked 
        {
            get { return this.isDipChecked; }
            set
            {
                if (value) 
                {
                    SelectedRecipeType = 1;
                    IsSpinChecked = false;
                    IsTiltChecked = false;
                    Symbol = "R_Dip";
                    SelectedCoatingStepRecipe.Class.SetValue("Recipe.CoatingStep.Tauchen / Schleudern / Wälzen", SelectedRecipeType);
                    Content = new Recipe_Coating_D();
                }
                this.isDipChecked = value;
                this.OnPropertyChanged("IsDipChecked");
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
                    SelectedRecipeType = 2;
                    IsDipChecked = false;
                    IsTiltChecked = false;
                    Symbol = "R_Spin";
                    SelectedCoatingStepRecipe.Class.SetValue("Recipe.CoatingStep.Tauchen / Schleudern / Wälzen", SelectedRecipeType);
                    Content = new Recipe_Coating_S();
                }
                this.isSpinChecked = value;
                this.OnPropertyChanged("IsSpinChecked");
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
                    SelectedRecipeType = 3;
                    IsDipChecked = false;
                    IsSpinChecked = false;
                    Symbol = "R_Tilt";
                    SelectedCoatingStepRecipe.Class.SetValue("Recipe.CoatingStep.Tauchen / Schleudern / Wälzen", SelectedRecipeType);
                    Content = new Recipe_Coating_T();
                }
                this.isTiltChecked = value;
                this.OnPropertyChanged("IsTiltChecked");
            }
        }

        string symbol="R_Dip";
        public string Symbol
        {
            get { return this.symbol; }
            set
            {
                this.symbol = value;
                this.OnPropertyChanged("Symbol");
            }
        }

        #endregion

        #region - - - Commands - - -
        public void LoadRecipeToBuffer(CoatingStepRecipe CSR)
        {
            if (CSR != null)
                if (CSR.Id != -1)
                {
                    SelectedCoatingStepRecipe = CSR;
                    LoadRecipeToBufferCommandExecuted(null);
                }
        }
        public ICommand LoadFileToBuffer { get; set; }
        private void LoadRecipeToBufferCommandExecuted(object parameter)
        {
            if (this.SelectedCoatingStepRecipe.Id != -1) 
            {
                Task obTask = Task.Run(() => {
                    DataTable DT = (new LocalDBAdapter("SELECT Data FROM Recipes_CoatingStep_VW WHERE ID = " + SelectedCoatingStepRecipe.Id + "; ")).DB_Output();
                    if (DT.Rows.Count > 0)
                    {
                        System.IO.File.WriteAllText(SelectedCoatingStepRecipe.Class.CurrentPath + @"\\" + SelectedCoatingStepRecipe.Name + ".R", DT.Rows[0]["Data"].ToString());
                    }
                });

                obTask.ContinueWith((xs) =>
                {
                    Dispatcher.BeginInvoke((Action)(async () =>
                    {
                        LoadFromFileToBufferResult result = await SelectedCoatingStepRecipe.Class.LoadFromFileToBufferAsync(this.SelectedCoatingStepRecipe.Name);
                        if (result.Result != LoadRecipeResult.Succeeded)
                        {
                            new MessageBoxTask("@RecipeSystem.Results.LoadError", "@MessageBox.Text1", MessageBoxIcon.Error);
                        }
                    }));
                   
                },
                TaskContinuationOptions.OnlyOnRanToCompletion
                );
                UpdateName();
                LastLoadedSavedCoatingStepRecipe = SelectedCoatingStepRecipe;
                OnCloseCommandExecuted("False");
            } 
        }
        public ICommand SaveFileCommand { get; set; }
        private void SaveRecipeCommandExecuted(object parameter)
        {
            long RecipeId = ((new RecipeManager(this.NameBuffer, SelectedCoatingStepRecipe.Class_Id)).IsCoatingStepRecipeExisting());
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
                DeleteOldStepParameter();

                Task obTask = Task.Run(async () => {

                    SaveToFileFromBufferResult e = (await SelectedCoatingStepRecipe.Class.SaveToFileFromBufferAsync(this.NameBuffer, this.DescriptionBuffer, true));
                    if (e.Result != SaveRecipeResult.Succeeded)
                    {
                        new MessageBoxTask("@RecipeSystem.Results.LoadError", "@MessageBox.Text1", MessageBoxIcon.Error);
                    }
                    else
                    {
                        if (RecipeId == -1)
                        {

                            (new LocalDBAdapter("INSERT " +
                                                "INTO Recipes_CoatingStep_VW (Name, Class_Id, Type_Id, Data) " +
                                                "VALUES ('" + this.NameBuffer + "'," + Class_Id + "," + SelectedRecipeType + ",'" + File.ReadAllText(SelectedCoatingStepRecipe.Class.CurrentPath + @"\\" + this.NameBuffer + ".R") + "')")
                             ).DB_Input();

                        }
                        else
                        {
                            (new LocalDBAdapter("UPDATE Recipes_CoatingStep_VW " +
                                                "SET Data = '" + File.ReadAllText(SelectedCoatingStepRecipe.Class.CurrentPath + @"\\" + this.NameBuffer + ".R") + "', Type_Id = " + SelectedRecipeType + " " +
                                                "WHERE Id = " + RecipeId + "")
                            ).DB_Input();
                        }
                    }

                    UpdateCoatingStepRecipeList(true);
                   
                });
                UpdateName();
                OnCloseCommandExecuted("False");
            }
        }
        public ICommand DeleteFileCommand { get; set; }
        private void OnDeleteFileCommandExecuted(object parameter)
        {
            if (this.SelectedCoatingStepRecipe.Id != -1)
            {
                //DataTable DT = (new LocalDBAdapter("SELECT * " +
                //                                     "FROM Recipes_Coating " +
                //                                     "WHERE S1_Id = " + SelectedCoatingStepRecipe.Id +
                //                                     " OR S2_Id = " + SelectedCoatingStepRecipe.Id +
                //                                     " OR S3_Id = " + SelectedCoatingStepRecipe.Id +
                //                                     " OR S4_Id = " + SelectedCoatingStepRecipe.Id +
                //                                     " OR S5_Id = " + SelectedCoatingStepRecipe.Id +
                //                                     " OR S6_Id= " + SelectedCoatingStepRecipe.Id +
                //                                     " OR S7_Id = " + SelectedCoatingStepRecipe.Id +
                //                                     " OR S8_Id = " + SelectedCoatingStepRecipe.Id + "; ")).DB_Output();

                //if (DT.Rows.Count != 0)
                //{
                //    new MessageBoxTask("@RecipeSystem.Results.Text6", "@RecipeSystem.Results.DeleteFile", MessageBoxIcon.Exclamation);
                //}
                //else
                //{
                    if (MessageBoxView.Show("@RecipeSystem.Results.DeleteFileQuery", "@RecipeSystem.Results.DeleteFile", MessageBoxButton.YesNo, icon: MessageBoxIcon.Question) == MessageBoxResult.Yes)
                    {
                        CoatingStepRecipe RecipeToDelete = this.SelectedCoatingStepRecipe;
                        Task obTask = Task.Run(() =>
                        {
                            RecipeToDelete.Class.DeleteFile(RecipeToDelete.Name);

                            (new LocalDBAdapter("DELETE " +
                                                "FROM Recipes_CoatingStep_VW " +
                                                "WHERE ID = " + RecipeToDelete.Id + "; ")).DB_Output();
                            
                            UpdateCoatingStepRecipeList(false);
                        });
                        NameBuffer = "";
                        DescriptionBuffer = "";
                        LastLoadedSavedCoatingStepRecipe = SelectedCoatingStepRecipe = new CoatingStepRecipe();
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

        #endregion

        #region - - - Event Handlers - - -

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

        private void UpdateCoatingStepRecipeList(bool setLastLoadedSavedCoatingStepRecipe)
        {
            Task obTask = Task.Run(() => {
                DataTable DT = (new LocalDBAdapter("SELECT Id, Name, Class_Id, Type_Id, Data " +
                                                    "FROM Recipes_CoatingStep_VW " +
                                                    "WHERE Class_Id = " + Class_Id + "; ")).DB_Output();
               
                var temp = new ObservableCollection<CoatingStepRecipe>();

                if (DT.Rows.Count > 0)
                {
                       
                    foreach (DataRow r in DT.Rows)
                    {
                        temp.Add(new CoatingStepRecipe()
                        {
                            Id = (long)r["Id"],
                            Name = (string)r["Name"],
                            Type_Id = (long)r["Type_Id"],
                            Data = (string)r["Data"]
                        });
                    }
                      
                }

                CoatingStepRecipes = temp;

                if(setLastLoadedSavedCoatingStepRecipe)
                    LastLoadedSavedCoatingStepRecipe = CoatingStepRecipes.Where(x => x.Name == NameBuffer).First();
            });
        }
        private void DipRecipeReset() 
        {
            SelectedCoatingStepRecipe.Class.SetValue("Recipe.CoatingStep.Tauchen.Reversierzeit", 0);
            SelectedCoatingStepRecipe.Class.SetValue("Recipe.CoatingStep.Tauchen.Rpm Körbe tauchen", 0);
            SelectedCoatingStepRecipe.Class.SetValue("Recipe.CoatingStep.Tauchen.Tauchzeit", 0);
        }
        private void SpinRecipeReset()
        {
            SelectedCoatingStepRecipe.Class.SetValue("Recipe.CoatingStep.Schleudern.Planet Drehzahl 2", 0);
            SelectedCoatingStepRecipe.Class.SetValue("Recipe.CoatingStep.Schleudern.Planet Drehzeit 2", 0);
            SelectedCoatingStepRecipe.Class.SetValue("Recipe.CoatingStep.Schleudern.Rotor Drehzahl 1", 0);
            SelectedCoatingStepRecipe.Class.SetValue("Recipe.CoatingStep.Schleudern.Rotor Drehzahl 2", 0);
            SelectedCoatingStepRecipe.Class.SetValue("Recipe.CoatingStep.Schleudern.Rotor Drehzahl 3", 0);
            SelectedCoatingStepRecipe.Class.SetValue("Recipe.CoatingStep.Schleudern.Rotor Drehzeit 1", 0);
            SelectedCoatingStepRecipe.Class.SetValue("Recipe.CoatingStep.Schleudern.Rotor Drehzeit 3", 0);
        }
        private void TiltRecipeReset()
        {
            SelectedCoatingStepRecipe.Class.SetValue("Recipe.CoatingStep.Wälzen.Kippwinkel", 0);
            SelectedCoatingStepRecipe.Class.SetValue("Recipe.CoatingStep.Wälzen.Korbdrehzahl", 0);
            SelectedCoatingStepRecipe.Class.SetValue("Recipe.CoatingStep.Wälzen.Reversierzeit", 0);
            SelectedCoatingStepRecipe.Class.SetValue("Recipe.CoatingStep.Wälzen.Wälzzeit", 0);
        }
        private void DeleteOldStepParameter() 
        {
            switch (SelectedRecipeType) 
            {
                case 1:
                    SpinRecipeReset();
                    TiltRecipeReset();
                    break;
                case 2:
                    DipRecipeReset();
                    TiltRecipeReset();
                    break;
                case 3:
                    DipRecipeReset();
                    SpinRecipeReset();
                    break;
            }
        
        }

        private void UpdateName()
        {
            ((Recipe_PN)iRS.GetView("Recipe_PN")).Rname.Value = NameBuffer;
            ((Recipe_PN)iRS.GetView("Recipe_PN")).Descr.Value = DescriptionBuffer;
        }
        private void UpdateCoatingStepRecipeLists()
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {   
                var adapter = (RecipeAdapter_Coating)((Recipe_Coating_PR)iRS.GetView("Recipe_Coating_PR")).DataContext;
                adapter.CoatingStepRecipes = CoatingStepRecipes;
            }));
        }

        #endregion

    }

   
}