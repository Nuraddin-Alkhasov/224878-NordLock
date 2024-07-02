using HMI.Module;
using HMI.Views.MessageBoxRegion;
using System;
using System.Collections.ObjectModel;
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
   
    [ExportAdapter("RecipeAdapter_Article")]
    public class RecipeAdapter_Article : AdapterBase
    {
        readonly IRegionService iRS = ApplicationService.GetService<IRegionService>();
        readonly int Class_Id = 1;
        public RecipeAdapter_Article()
        {
            this.LoadFileToBuffer = new ActionCommand(this.LoadRecipeToBufferCommandExecuted);
            this.SaveFileCommand = new ActionCommand(this.SaveRecipeCommandExecuted);
            this.DeleteFileCommand = new ActionCommand(this.OnDeleteFileCommandExecuted);
            this.CloseCommand = new ActionCommand(this.OnCloseCommandExecuted);

            UpdateArticleRecipeList(false);
            IsLDChecked = true;
        }

        #region - - - Properties - - -
        ObservableCollection<ArticleRecipe> articleRecipes = new ObservableCollection<ArticleRecipe>();
        public ObservableCollection<ArticleRecipe> ArticleRecipes
        {
            get { return this.articleRecipes; }
            set
            {
                if (value != null)
                {
                    this.articleRecipes = value;
                    UpdateArticleRecipeLists();
                    this.OnPropertyChanged("ArticleRecipes");
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
        public ArticleRecipe LastLoadedSavedArticleRecipe = new ArticleRecipe();
        private ArticleRecipe selectedArticleRecipe = new ArticleRecipe();
        public ArticleRecipe SelectedArticleRecipe
        {
            get { return selectedArticleRecipe; }
            set
            {
                if (value != null)
                {
                    this.selectedArticleRecipe = value;
                    this.NameBuffer = value.Name;
                    this.DescriptionBuffer = value.VWR.Description;
                    this.OnPropertyChanged("SelectedArticleRecipe");
                }
            }
        }

        object content = new Recipe_Article_LD();
        public object Content
        {
            get { return content; }
            set
            {
                this.content = value;
                this.OnPropertyChanged("Content");
            }
        }

        bool isLDChecked = false;
        public bool IsLDChecked
        {
            get { return this.isLDChecked; }
            set
            {
                if (value)
                {
                    Content = new Recipe_Article_LD();
                }
                this.isLDChecked = value;
                this.OnPropertyChanged("IsLDChecked");
            }
        }

        bool isHCChecked = false;
        public bool IsHCChecked
        {
            get { return this.isHCChecked; }
            set
            {
                if (value)
                {
                    Content = new Recipe_Article_HC();
                }
                this.isHCChecked = value;
                this.OnPropertyChanged("IsHCChecked");
            }
        }
        bool isDCChecked = false;
        public bool IsDCChecked
        {
            get { return this.isDCChecked; }
            set
            {
                if (value)
                {
                    Content = new Recipe_Article_DC();
                }
                this.isDCChecked = value;
                this.OnPropertyChanged("IsDCChecked");
            }
        }

        bool isBSChecked = false;
        public bool IsBSChecked
        {
            get { return this.isBSChecked; }
            set
            {
                if (value)
                {
                    Content = new Recipe_Article_BS();
                }
                this.isBSChecked = value;
                this.OnPropertyChanged("IsBSChecked");
            }
        }

        bool isKAChecked = false;
        public bool IsKAChecked
        {
            get { return this.isKAChecked; }
            set
            {
                if (value)
                {
                    Content = new Recipe_Article_KA();
                }
                this.isKAChecked = value;
                this.OnPropertyChanged("IsKAChecked");
            }
        }

        bool isTB1Checked = false;
        public bool IsTB1Checked
        {
            get { return this.isTB1Checked; }
            set
            {
                if (value)
                {
                    Content = new Recipe_Article_TB1();
                }
                this.isTB1Checked = value;
                this.OnPropertyChanged("IsTB1Checked");
            }
        }

        bool isTB2Checked = false;
        public bool IsTB2Checked
        {
            get { return this.isTB2Checked; }
            set
            {
                if (value)
                {
                    Content = new Recipe_Article_TB2();
                }
                this.isTB2Checked = value;
                this.OnPropertyChanged("IsTB2Checked");
            }
        }

        bool isTARChecked = false;
        public bool IsTARChecked
        {
            get { return this.isTARChecked; }
            set
            {
                if (value)
                {
                    Content = new Recipe_Article_TAR();
                }
                this.isTARChecked = value;
                this.OnPropertyChanged("IsTARChecked");
            }
        }
        bool isTAAChecked = false;
        public bool IsTAAChecked
        {
            get { return this.isTAAChecked; }
            set
            {
                if (value)
                {
                    Content = new Recipe_Article_TAA();
                }
                this.isTAAChecked = value;
                this.OnPropertyChanged("IsTAAChecked");
            }
        }


        #endregion

        #region - - - Commands - - -
        public void LoadRecipeToBuffer(ArticleRecipe AR) 
        {
            if (AR.Id != -1) 
            {
                SelectedArticleRecipe = AR;
                LoadRecipeToBufferCommandExecuted(null);
            }
        }
        public ICommand LoadFileToBuffer { get; set; }
        void LoadRecipeToBufferCommandExecuted(object parameter)
        {
            if (this.SelectedArticleRecipe.Id != -1)
            {
                Task obTask = Task.Run(() => {
                    DataTable DT = (new LocalDBAdapter("SELECT Data FROM Recipes_Article_VW WHERE ID = " + SelectedArticleRecipe.Id + "; ")).DB_Output();
                    if (DT.Rows.Count > 0)
                    {
                        System.IO.File.WriteAllText(SelectedArticleRecipe.Class.CurrentPath + @"\\" + SelectedArticleRecipe.Name + ".R", DT.Rows[0]["Data"].ToString());
                    }
                });

                obTask.ContinueWith((xs) =>
                {
                    Dispatcher.BeginInvoke((Action)(async () =>
                    {
                        LoadFromFileToBufferResult result = await SelectedArticleRecipe.Class.LoadFromFileToBufferAsync(this.SelectedArticleRecipe.Name);
                        if (result.Result != LoadRecipeResult.Succeeded)
                        {
                            new MessageBoxTask("@RecipeSystem.Results.LoadError", "@MessageBox.Text1", MessageBoxIcon.Error);
                        }
                    }));            
                },
                TaskContinuationOptions.OnlyOnRanToCompletion
                );
                UpdateName();
                LastLoadedSavedArticleRecipe = SelectedArticleRecipe;
                OnCloseCommandExecuted(null);
            }
        }
        public ICommand SaveFileCommand { get; set; }
        private void SaveRecipeCommandExecuted(object parameter)
        {
            long RecipeId = ((new RecipeManager(this.NameBuffer, SelectedArticleRecipe.Class_Id)).IsArticleRecipeExisting());
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
                Task obTask = Task.Run(async () =>
                {
                    SaveToFileFromBufferResult e = (await SelectedArticleRecipe.Class.SaveToFileFromBufferAsync(this.NameBuffer, this.DescriptionBuffer, true));
                    if (e.Result != SaveRecipeResult.Succeeded)
                    {
                        new MessageBoxTask("@RecipeSystem.Results.LoadError", "@MessageBox.Text1", MessageBoxIcon.Error);
                    }
                    else
                    {
                        if (RecipeId == -1)
                        {

                            (new LocalDBAdapter("INSERT " +
                                                "INTO Recipes_Article_VW (Name, Data) " +
                                                "VALUES ('" + this.NameBuffer + "', '" + File.ReadAllText(SelectedArticleRecipe.Class.CurrentPath + @"\\" + this.NameBuffer + ".R") + "')")
                             ).DB_Input();

                        }
                        else
                        {
                            (new LocalDBAdapter("UPDATE Recipes_Article_VW " +
                                                "SET Data = '" + File.ReadAllText(SelectedArticleRecipe.Class.CurrentPath + @"\\" + this.NameBuffer + ".R") + "' " +
                                                "WHERE Id = " + RecipeId + "")
                            ).DB_Input();
                        }
                    }

                    UpdateArticleRecipeList(true);
                });
                UpdateName();
                OnCloseCommandExecuted("False");
            }
        }
        public ICommand DeleteFileCommand { get; set; }
        private void OnDeleteFileCommandExecuted(object parameter)
        {
            if (this.SelectedArticleRecipe.Id != -1)
            {
                //DataTable DT = (new LocalDBAdapter("SELECT * " +
                //                                  "FROM Recipes_MR " +
                //                                  "WHERE Article_Id = " + SelectedArticleRecipe.Id + "; ")).DB_Output();

                //if (DT.Rows.Count != 0)
                //{
                //    new MessageBoxTask("@RecipeSystem.Results.Text5", "@RecipeSystem.Results.DeleteFile", MessageBoxIcon.Exclamation);
                //}
                //else
                //{

                    if (MessageBoxView.Show("@RecipeSystem.Results.DeleteFileQuery", "@RecipeSystem.Results.DeleteFile", MessageBoxButton.YesNo, icon: MessageBoxIcon.Question) == MessageBoxResult.Yes)
                    {
                        ArticleRecipe RecipeToDelete = this.SelectedArticleRecipe;
                        Task obTask = Task.Run(() =>
                        {
                            SelectedArticleRecipe.Class.DeleteFile(RecipeToDelete.Name);

                            (new LocalDBAdapter("DELETE " +
                                                "FROM Recipes_Article_VW " +
                                                "WHERE ID = " + RecipeToDelete.Id + "; ")).DB_Output();

                            UpdateArticleRecipeList(false);
                        });
                        NameBuffer = "";
                        DescriptionBuffer = "";
                        LastLoadedSavedArticleRecipe = SelectedArticleRecipe = new ArticleRecipe(); 
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

        #region - - - Methods - - -
        public void UpdateArticleRecipeList(bool setLastLoadedSavedArticleRecipe)
        {
            Task obTask = Task.Run(() => {
                DataTable DT = (new LocalDBAdapter("SELECT Id, Name, Data " +
                                                    "FROM Recipes_Article_VW " +
                                                    "WHERE Class_Id = " + Class_Id + "; ")).DB_Output();

                var temp = new ObservableCollection<ArticleRecipe>();

                if (DT.Rows.Count > 0)
                {
                    foreach (DataRow r in DT.Rows)
                    {
                        temp.Add(new ArticleRecipe()
                        {
                            Id = (long)r["Id"],
                            Name = (string)r["Name"],
                            Type_Id = 0,
                            Data = (string)r["Data"]
                        });
                    }

                }
                ArticleRecipes = temp;

                if (setLastLoadedSavedArticleRecipe)
                    LastLoadedSavedArticleRecipe = ArticleRecipes.Where(x => x.Name == NameBuffer).First();
            });
        }

        private void UpdateName()
        {
            ((Recipe_PN)iRS.GetView("Recipe_PN")).Rname.Value = NameBuffer;
            ((Recipe_PN)iRS.GetView("Recipe_PN")).Descr.Value = DescriptionBuffer;
        }

        private void UpdateArticleRecipeLists()
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                RecipeAdapter_MR adapter = (RecipeAdapter_MR)((Recipe_Machine)iRS.GetView("Recipe_Machine")).DataContext;
                adapter.ArticleRecipes = ArticleRecipes;
            }));
        }
        #endregion
    }


}