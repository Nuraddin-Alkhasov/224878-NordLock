using System;
using VisiWin.ApplicationFramework;

namespace HMI.Views.MainRegion.Recipe
{
	/// <summary>
	/// Interaction logic for BSStepEdit.xaml
	/// </summary>
	[ExportView("Recipe_Coating_Steps")]
	public partial class Recipe_Coating_Steps
	{
        public Recipe_Coating_Steps()
        {
            this.InitializeComponent();
        }

        private void V1_ValueChanged(object sender, VisiWin.DataAccess.VariableEventArgs e)
        {
            switch ((short)e.Value)
            {
                case 1: btndiping.IsChecked = true; break;
                case 2: btnspining.IsChecked = true; break;
                case 3: btntilting.IsChecked = true; break;
            }
        }
        private void NumericVarIn_ValueChanged(object sender, VisiWin.DataAccess.VariableEventArgs e)
        {
            if (weight.Value >= 30)
            {
                mDreh.Value = Math.Floor(0.05 * weight.Value * weight.Value - 6 * weight.Value + 365);
            }
            else 
            {
                mDreh.Value = 230;
            }
        }
    }
}