using System.Windows.Controls;
using System.Windows.Input;
using VisiWin.ApplicationFramework;


namespace HMI.Views.MainRegion.MachineOverview
{

    [ExportView("DPR_Selector")]
    public partial class DPR_Selector : VisiWin.Controls.View
    {

        public DPR_Selector()
        {
            this.InitializeComponent();
            IRegionService iRS = ApplicationService.GetService<IRegionService>();
            DataPickerAdapter temp = (DataPickerAdapter)((MO_DataPicker)iRS.GetView("MO_DataPicker")).DataContext;
            this.DataContext = temp;
        }

        private void DataGridRow_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            RSdgv_recipe.UnselectAllCells();
            ((DataGridRow)sender).IsSelected = true;
            //tbName.Value = ;
            //    tbDescription.Value = "";
        }

    }
}