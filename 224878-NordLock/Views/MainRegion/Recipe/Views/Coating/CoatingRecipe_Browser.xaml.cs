using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisiWin.ApplicationFramework;


namespace HMI.Views.MainRegion.Recipe
{

    [ExportView("CoatingRecipe_Browser")]
    public partial class CoatingRecipe_Browser
    {
        IRegionService iRS = ApplicationService.GetService<IRegionService>();
        public CoatingRecipe_Browser()
        {
            this.InitializeComponent();
            
            this.DataContext = (RecipeAdapter_Coating)((Recipe_Coating_PR)iRS.GetView("Recipe_Coating_PR")).DataContext; 
        }

        #region - - - Event Handlers - - -

        private void _IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                RecipeAdapter_Coating RC = (RecipeAdapter_Coating)((Recipe_Coating_PR)iRS.GetView("Recipe_Coating_PR")).DataContext;
                RBdgv_recipe.SelectedIndex = -1;
                RC.SelectedCoatingRecipe = RC.LastLoadedSavedCoatingRecipe;
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