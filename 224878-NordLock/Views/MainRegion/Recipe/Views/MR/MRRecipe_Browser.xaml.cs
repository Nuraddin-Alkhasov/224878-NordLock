using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisiWin.ApplicationFramework;


namespace HMI.Views.MainRegion.Recipe
{

    [ExportView("MRRecipe_Browser")]
    public partial class MRRecipe_Browser : VisiWin.Controls.View
    {
        IRegionService iRS = ApplicationService.GetService<IRegionService>();
        public MRRecipe_Browser()
        {
            this.InitializeComponent();

            RecipeAdapter_MR temp = (RecipeAdapter_MR)((Recipe_Machine)iRS.GetView("Recipe_Machine")).DataContext;
            this.DataContext = temp;
            temp.UpdateMachineRecipeList(false);
        }

        private void _IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                RecipeAdapter_MR RMR = (RecipeAdapter_MR)((Recipe_Machine)iRS.GetView("Recipe_Machine")).DataContext;
                RBdgv_recipe.SelectedIndex = -1;
                RMR.SelectedMachineRecipe = RMR.LastLoadedSavedMachineRecipe;
            }
        }
        private void RBdgv_recipe_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            RBdgv_recipe.UnselectAllCells();
            ((DataGridRow)sender).IsSelected = true;
        }

    }
}