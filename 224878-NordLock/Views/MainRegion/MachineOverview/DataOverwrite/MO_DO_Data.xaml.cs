
using System;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Recipe;

namespace HMI.Views.MainRegion.MachineOverview
{

	[ExportView("MO_DO_Data")]
	public partial class MO_DO_Data : VisiWin.Controls.View
	{

        public MO_DO_Data()
		{
			this.InitializeComponent();
          
        }

        private void View_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
               // mr.Value = ApplicationService.GetVariableValue("").ToString();
               // user.Value = ApplicationService.GetVariableValue("").ToString();
            }
        }

        private void Mr_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            ApplicationService.SetView("MessageBoxRegion", "Recipe_Selector");
        }

        private void Mr_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ApplicationService.SetView("MessageBoxRegion", "Recipe_Selector");
        }

        private void Mr_ValueChanged(object sender, VisiWin.DataAccess.VariableEventArgs e)
        {
            if (mr.Value.ToString() != "")
            {
                Task obTask = Task.Run(() =>
                {
                    Application.Current.Dispatcher.InvokeAsync((Action)delegate
                    {
                        IRecipeClass T = ApplicationService.GetService<IRecipeService>().GetRecipeClass("MR");
                        if (T.IsExistingRecipeFile(mr.Value))
                        {
                            rd.Value = T.GetRecipeFile(mr.Value).Description;
                        }
                    });
                });
            }
            else
            {
                rd.Value = "";
            }
        }
    }
}