using System;
using System.Windows;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;

namespace HMI.Views.MainRegion.Recipe
{
	[ExportView("Recipe_Coating_T")]
	public partial class Recipe_Coating_T : VisiWin.Controls.View
	{

        public Recipe_Coating_T()
		{
			this.InitializeComponent();
        }

        private void A_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (A.IsVisible)
            {
                A.BeginAnimation(UIElement.OpacityProperty, SetOpacity(1, 1));
            }
            else
            {
                A.BeginAnimation(UIElement.OpacityProperty, SetOpacity(0, 1));    
            }
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