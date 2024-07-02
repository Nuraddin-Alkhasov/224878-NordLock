using HMI.Views.MainRegion.MachineOverview;
using System;
using System.Windows;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;

namespace HMI.Views.MainRegion.Recipe
{
	/// <summary>
	/// Interaction logic for BSStepEdit.xaml
	/// </summary>
	[ExportView("Recipe_Article_LD")]
	public partial class Recipe_Article_LD : VisiWin.Controls.View
	{

        public Recipe_Article_LD()
		{
			this.InitializeComponent();
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