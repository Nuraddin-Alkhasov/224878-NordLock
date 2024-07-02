using HMI.Views.MainRegion.Recipe.Custom_Objects;
using System.Windows.Controls;
using System.Windows.Input;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;


namespace HMI.Views.MainRegion.Recipe
{

	[ExportView("Recipe_Binding")]
	public partial class Recipe_Binding : VisiWin.Controls.View
	{
        public Recipe_Binding()
		{
			this.InitializeComponent();
         
        }

        private void TextVarOut_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ApplicationService.SetView("MessageBoxRegion", "Recipe_Selector");
        }

        private void mr_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            ApplicationService.SetView("MessageBoxRegion", "Recipe_Selector");
        }

        private void DataGridRow_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            dgv_bctor.UnselectAllCells();
            ((DataGridRow)sender).IsSelected = true;
        }

        private void Grid_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            ((RecipeBindingAdapter)this.DataContext).UpdateBarcodeList();
        }
    }
}