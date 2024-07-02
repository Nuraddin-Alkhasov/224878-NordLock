using System.Windows;
using System.Windows.Controls;
using VisiWin.ApplicationFramework;


namespace HMI.Views.MainRegion.Recipe
{

    [ExportView("Recipe_Selector")]
    public partial class Recipe_Selector : VisiWin.Controls.View
    {

        public Recipe_Selector()
        {
            this.InitializeComponent();
            IRegionService iRS = ApplicationService.GetService<IRegionService>();
            this.DataContext = (RecipeBindingAdapter)((Recipe_Binding)iRS.GetView("Recipe_Binding")).DataContext;
        }

        private void DataGridRow_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            RSdgv_recipe.UnselectAllCells();
            ((DataGridRow)sender).IsSelected = true;
        }

    }
}