using HMI.Module;
using HMI.Views.DialogRegion;
using HMI.Views.MessageBoxRegion;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using VisiWin.DataAccess;
using VisiWin.Helper;
using VisiWin.Language;
using VisiWin.Logging;
using VisiWin.Recipe;

namespace HMI.Views.MainRegion.MachineOverview
{

	[ExportView("MO_Status_Basket")]
	public partial class MO_Status_Basket : VisiWin.Controls.View
    {

        public MO_Status_Basket()
		{
			this.InitializeComponent();
          
        }



        private void View_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                StateCollection Temp_SC = new StateCollection();
                for (int i = 1; i <= 10; i++)
                {
                    string temp = ApplicationService.GetVariableValue("NL.PLC.Blocks.50 HMI.00 Allgemein.Lacktyp Namen.Lacktyp Name[" + i.ToString() + "]").ToString();
                    if (temp != "")
                    {
                        Temp_SC.Add(new State()
                        {
                            Text = temp,
                            Value = i.ToString()
                        });
                    }
                }
                PaintList.StateList = Temp_SC;
            }
        }

        private void Switch_ValueChanged(object sender, VisiWin.DataAccess.VariableEventArgs e)
        {
            if ((bool)e.Value)
            {
                BL.IsChecked = true;
                BR.IsChecked = true;
            }
            else
            {
                BL.IsChecked = false;
                BR.IsChecked = false;
            }
        }
    }
}