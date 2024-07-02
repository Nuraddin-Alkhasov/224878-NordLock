using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisiWin.ApplicationFramework;


namespace HMI.Views.MainRegion.Recipe
{

    [ExportView("CoatingStepRecipe_Browser")]
    public partial class CoatingStepRecipe_Browser : VisiWin.Controls.View
    {
        IRegionService iRS = ApplicationService.GetService<IRegionService>();
        public CoatingStepRecipe_Browser()
        {
            this.InitializeComponent();
            this.DataContext = (RecipeAdapter_CS)((Recipe_Coating_Steps)iRS.GetView("Recipe_Coating_Steps")).DataContext;
        }

        private void _IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible) 
            {
                RecipeAdapter_CS RCS = (RecipeAdapter_CS)((Recipe_Coating_Steps)iRS.GetView("Recipe_Coating_Steps")).DataContext;
                RBdgv_recipe.SelectedIndex = -1;
                RCS.SelectedCoatingStepRecipe = RCS.LastLoadedSavedCoatingStepRecipe;
            }
        }
        private void RBdgv_recipe_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            RBdgv_recipe.UnselectAllCells();
            ((DataGridRow)sender).IsSelected = true;
        }
    }
}