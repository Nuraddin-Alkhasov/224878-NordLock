using VisiWin.ApplicationFramework;

namespace HMI.Views.MainRegion.OperatingHours
{
	/// <summary>
	/// Interaction logic for BSStepEdit.xaml
	/// </summary>
	[ExportView("OH_M2_LTB")]
	public partial class OH_M2_LTB : VisiWin.Controls.View
	{
       
        public OH_M2_LTB()
		{
			this.InitializeComponent();
        }

        private void Btn1_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            btn1.IsSelected = true;
        }

    }
}