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
	[ExportView("Recipe_Article_TAA")]
	public partial class Recipe_Article_TAA : VisiWin.Controls.View
	{

        public Recipe_Article_TAA()
		{
			this.InitializeComponent();
			IRecipeClass T = ApplicationService.GetService<IRecipeService>().GetRecipeClass("Article");
			T.SetValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Station.Tablett auskippen Auslauf.Kippinterval[1].Angle", 180);
		}
		private void Switch_ValueChanged(object sender, VisiWin.DataAccess.VariableEventArgs e)
		{
			if ((bool)e.Value)
			{
				hits.RawLimitMin = 1;
			}
			else
			{
				hits.RawLimitMin = 0;
				hits.Value = 0;
			}
		}

		private void View_Loaded(object sender, RoutedEventArgs e)
		{
			A.BeginAnimation(UIElement.OpacityProperty, SetOpacity(1, 1));
		}
		private void A_Unloaded(object sender, RoutedEventArgs e)
		{
			A.BeginAnimation(UIElement.OpacityProperty, SetOpacity(0, 1));
		}

		private DoubleAnimation SetOpacity(Double _O, int _T)
        {
            return new DoubleAnimation
            {
                To = _O,
                Duration = TimeSpan.FromSeconds(_T),
            };
        }
    }
}