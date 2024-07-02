using HMI.UserControls;
using VisiWin.ApplicationFramework;

namespace HMI.Views.MainRegion.Recipe
{
	/// <summary>
	/// Interaction logic for View1.xaml
	/// </summary>
	[ExportView("Recipe_Coating_PN")]
	public partial class Recipe_Coating_PN
	{
        public Recipe_Coating_PN()
		{
			this.InitializeComponent();
        }

        private void pn_C_SelectedPanoramaRegionChanged(object sender, VisiWin.Controls.SelectedPanoramaRegionChangedEventArgs e)
        {
            IRegionService iRS = ApplicationService.GetService<IRegionService>();
            Recipe_PN R_PN = (Recipe_PN)iRS.GetView("Recipe_PN");
            RecipeAdapter_Coating RA_C = (RecipeAdapter_Coating)((Recipe_Coating_PR)iRS.GetView("Recipe_Coating_PR")).DataContext;
            if (R_PN.pn_recipe.SelectedPanoramaRegionIndex == 2) 
            {
                switch (pn_coating.SelectedPanoramaRegionIndex)
                {
                    case 0:
                        R_PN.HeaderTxt.LocalizableText = "@RecipeSystem.Text77";
                        R_PN.Rname.Value = RA_C.SelectedCoatingRecipe != null ? RA_C.SelectedCoatingRecipe.Name : "";
                        R_PN.Descr.Value = RA_C.SelectedCoatingRecipe != null ? RA_C.SelectedCoatingRecipe.Description : "";
                        
                        //((RecipeAdapter_Coating)((Recipe_Coating_PR)iRS.GetView("Recipe_Coating_PR")).DataContext).UpdatePaintTypes();
                        break;
                    case 1:
                        R_PN.HeaderTxt.LocalizableText = "@RecipeSystem.Text7";
                        RecipeAdapter_CS RA_CS = (RecipeAdapter_CS)((Recipe_Coating_Steps)iRS.GetView("Recipe_Coating_Steps")).DataContext;
                        R_PN.Rname.Value = RA_CS.LastLoadedSavedCoatingStepRecipe.Name;
                        R_PN.Descr.Value = RA_CS.LastLoadedSavedCoatingStepRecipe.VWR.Description;
                        RA_CS.LoadRecipeToBuffer((CoatingStepRecipe)((Recipe_Template)RA_C.SelectedCoatingStep).RTD.Recipe);
                        break;
                }
            }
        
        }
 
    }
}