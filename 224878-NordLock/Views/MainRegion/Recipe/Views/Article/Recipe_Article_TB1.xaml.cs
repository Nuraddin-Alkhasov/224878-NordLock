using HMI.Views.MainRegion.MachineOverview;
using System;
using System.Windows;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;
using VisiWin.Recipe;

namespace HMI.Views.MainRegion.Recipe
{
	/// <summary>
	/// Interaction logic for BSStepEdit.xaml
	/// </summary>
	[ExportView("Recipe_Article_TB1")]
	public partial class Recipe_Article_TB1 : VisiWin.Controls.View
	{

        public Recipe_Article_TB1()
		{
			this.InitializeComponent();
            IRecipeClass T = ApplicationService.GetService<IRecipeService>().GetRecipeClass("Article");
            T.SetValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Station.Trockner Band 1.Abdunstzeiten.Minute", 2);
        }

        private DoubleAnimation SetOpacity(Double _O, int _T)
        {
            return new DoubleAnimation
            {
                To = _O,
                Duration = TimeSpan.FromSeconds(_T),
            };
        }


        private void View_Loaded(object sender, RoutedEventArgs e)
        {
            A.BeginAnimation(UIElement.OpacityProperty, SetOpacity(1, 1));
        }
        private void A_Unloaded(object sender, RoutedEventArgs e)
        {
            A.BeginAnimation(UIElement.OpacityProperty, SetOpacity(0, 1));
        }
    }
}