using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VisiWin.ApplicationFramework;
using VisiWin.Recipe;

namespace HMI.Views.MainRegion.Recipe
{
	/// <summary>
	/// Interaction logic for View1.xaml
	/// </summary>
	[ExportView("Recipe_Article")]
	public partial class Recipe_Article : VisiWin.Controls.View
	{
        public bool SaveRecipe = true;

        public Recipe_Article()
		{
            this.InitializeComponent();
        }
    }
}