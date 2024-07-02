using HMI.Module;
using HMI.Views.MainRegion.Protocol.Custom_Objects;
using HMI.Views.MessageBoxRegion;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;

namespace HMI.Views.MainRegion.MachineOverview
{

	[ExportView("MO_DataPicker")]
	public partial class MO_DataPicker : VisiWin.Controls.View
	{
        public MO_DataPicker()
		{
			this.InitializeComponent();
        }

        private void _IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                ((DataPickerAdapter)this.DataContext).UpdateMachineRecipeList(-1);
            }
            //new XMLSerializer(685);
            //IVariableService VS = ApplicationService.GetService<IVariableService>();
            //IVariable VWV_Weight = VS.GetVariable("NL.PLC.Blocks.1 Modul 1.04 Basket filling station.DB KBD PD.Status.Charge.Chargen Gewicht");
            //var x = Math.Round((float)VWV_Weight.Value, 1).ToString();
            //string VN_PaintTemp = "NL.PLC.Blocks.2 Modul 2.05 Tauchbecken.DB LTB HMI.Actual value.Dipping Vat.Lack Temperatur";
            //var x = (double)Math.Round((float)VS.GetValue(VN_PaintTemp, true), 1);

        }

        private void data3_ValueChanged(object sender, VariableEventArgs e)
        {
            //Task obTask = Task.Run(() =>
            //{
            //    Application.Current.Dispatcher.InvokeAsync((Action)delegate
            //    {
            //        DataTable DT = (new localDBAdapter("Select * " +
            //                                             "FROM Barcodes " +
            //                                             "WHERE Barcode = '" + data3.Value + "';")).DB_Output();
            //        if (DT.Rows.Count == 0)
            //        {
            //            new MessageBoxTask("@DataPicker.Text11", "@Datenauswahl.Text9", MessageBoxIcon.Warning);
            //        }
            //        else
            //        {
            //            if (DT.Rows.Count != 0)
            //            {
            //                DataPickerAdapter DPA = ((DataPickerAdapter)this.DataContext);
            //                DPA.CurrentOrder.Data_3 = data3.Value;
            //                DPA.CurrentOrder.MR = DPA.MachineRecipes.Where(x => x.Id == (long)DT.Rows[0]["MR_Id"]).First();
            //                DPA.CurrentOrder = new Order(DPA.CurrentOrder); 
            //                user.Value = ApplicationService.GetVariableValue("__CURRENT_USER.FULLNAME").ToString();
            //            }
            //        }
            //    });
            //});
        }

        private void data1_ValueChanged(object sender, VariableEventArgs e)
        {
            ((DataPickerAdapter)this.DataContext).CheckIfOrderAlreadyExist();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DataPickerAdapter DPA = ((DataPickerAdapter)this.DataContext);
            if (DPA.EditEnabled)
                ApplicationService.SetView("MessageBoxRegion", "DPR_Selector");
        }
    }
}