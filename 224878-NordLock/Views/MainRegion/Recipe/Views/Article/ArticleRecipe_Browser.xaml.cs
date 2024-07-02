using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisiWin.ApplicationFramework;


namespace HMI.Views.MainRegion.Recipe
{

    [ExportView("ArticleRecipe_Browser")]
    public partial class ArticleRecipe_Browser
    {
        IRegionService iRS = ApplicationService.GetService<IRegionService>();
        public ArticleRecipe_Browser()
        {
            this.InitializeComponent();
         
            this.DataContext = (RecipeAdapter_Article)((Recipe_Article)iRS.GetView("Recipe_Article")).DataContext; 
        }

        #region - - - Event Handlers - - -
        private void _IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                RecipeAdapter_Article RA = (RecipeAdapter_Article)((Recipe_Article)iRS.GetView("Recipe_Article")).DataContext;
                RBdgv_recipe.SelectedIndex = -1;
                RA.SelectedArticleRecipe = RA.LastLoadedSavedArticleRecipe;
            }
        }

        private void RBdgv_recipe_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            RBdgv_recipe.UnselectAllCells();
            ((DataGridRow)sender).IsSelected = true;
        }
        #endregion

    }
}