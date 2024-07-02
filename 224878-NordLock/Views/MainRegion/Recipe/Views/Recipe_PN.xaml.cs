using HMI.UserControls;
using System.Linq;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Views.MainRegion.Recipe
{

    [ExportView("Recipe_PN")]
    public partial class Recipe_PN 
    {
        IRegionService iRS = ApplicationService.GetService<IRegionService>();

        public Recipe_PN()
        {
            this.InitializeComponent();
        }

        private void Pn_recipe_SelectedPanoramaRegionChanged(object sender, SelectedPanoramaRegionChangedEventArgs e)
        {
            IRegionService iRS = ApplicationService.GetService<IRegionService>();
            RecipeAdapter_MR RA_MR = (RecipeAdapter_MR)((Recipe_Machine)iRS.GetView("Recipe_Machine")).DataContext;
            switch (pn_recipe.SelectedPanoramaRegionIndex)
            {
                case 0:
                    HeaderTxt.LocalizableText = "@RecipeSystem.Text2";
                    RecipeAdapter_Article RA_A = (RecipeAdapter_Article)((Recipe_Article)iRS.GetView("Recipe_Article")).DataContext;
                    if (RA_MR.LastLoadedSavedMachineRecipe != null)
                    {
                        Rname.Value = RA_MR.LastLoadedSavedMachineRecipe.Article.Name;
                        Descr.Value = RA_MR.LastLoadedSavedMachineRecipe.Article.VWR.Description;
                        RA_A.LoadRecipeToBuffer(RA_MR.ArticleBuffer);
                    }
                    break;
                case 1:
                    HeaderTxt.LocalizableText = "@RecipeSystem.Text1";
                    if (RA_MR.LastLoadedSavedMachineRecipe != null) 
                    {
                        Rname.Value = RA_MR.LastLoadedSavedMachineRecipe.Name;
                        Descr.Value = RA_MR.LastLoadedSavedMachineRecipe.Description;
                    }
                    break;
                case 2:
                    Recipe_Coating_PN rcpn = (Recipe_Coating_PN)pn_recipe.PanoramaRegions[2].Content;
                    switch (rcpn.pn_coating.SelectedPanoramaRegionIndex)
                    {
                        case 0: 
                            HeaderTxt.LocalizableText = "@RecipeSystem.Text77";
                            RecipeAdapter_Coating RA_C = (RecipeAdapter_Coating)((Recipe_Coating_PR)iRS.GetView("Recipe_Coating_PR")).DataContext;
                            if (RA_MR.LastLoadedSavedMachineRecipe != null)
                            {
                                if (RA_MR.SelectedCoatingLayer != null && RA_MR.LastLoadedSavedMachineRecipe.CoatingLayers[0].Id !=-1)
                                {
                                    CoatingRecipe CR = RA_MR.LastLoadedSavedMachineRecipe.CoatingLayers.Where(x => x.Name == ((CoatingRecipe)((Recipe_Template)RA_MR.SelectedCoatingLayer).RTD.Recipe).Name).First();

                                    if (CR != null)
                                    {
                                        Rname.Value = CR.Name;
                                        Descr.Value = CR.Description;
                                        RA_C.LoadRecipeToBuffer((CoatingRecipe)((Recipe_Template)RA_MR.SelectedCoatingLayer).RTD.Recipe);
                                    }
                                }    
                            }
                            break;
                        case 1: HeaderTxt.LocalizableText = "@RecipeSystem.Text7";
                            RecipeAdapter_CS RA_CS = (RecipeAdapter_CS)((Recipe_Coating_Steps)iRS.GetView("Recipe_Coating_Steps")).DataContext;
                            if (RA_CS.LastLoadedSavedCoatingStepRecipe != null)
                            {
                                Rname.Value = RA_CS.LastLoadedSavedCoatingStepRecipe.Name;
                                Descr.Value = RA_CS.LastLoadedSavedCoatingStepRecipe.VWR.Description;
                                rcpn.pn_coating.ScrollPrevious();
                            }
                            break;
                    }
                    break;

            }
        }
     
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (pn_recipe.SelectedPanoramaRegionIndex)
            {
                case 0:
                    ApplicationService.SetView("DialogRegion", "ArticleRecipe_Browser");
                    break;
                case 1:
                    ApplicationService.SetView("DialogRegion", "MRRecipe_Browser");
                    break;
                case 2:
                    Recipe_Coating_PN rcpn = (Recipe_Coating_PN)pn_recipe.PanoramaRegions[2].Content;
                    switch (rcpn.pn_coating.SelectedPanoramaRegionIndex)
                    {
                        case 0: ApplicationService.SetView("DialogRegion", "CoatingRecipe_Browser"); break;

                        case 1: ApplicationService.SetView("DialogRegion", "CoatingStepRecipe_Browser"); break;
                    }   
                    break;
            }
        }

    }
}