using VisiWin.ApplicationFramework;

namespace HMI.Views.MainRegion.OperatingHours
{
	/// <summary>
	/// Interaction logic for BSStepEdit.xaml
	/// </summary>
	[ExportView("OH_M4_HNT")]
	public partial class OH_M4_HNT : VisiWin.Controls.View
	{
       
        public OH_M4_HNT()
		{
			this.InitializeComponent();
        }

        private void Btn1_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            btn1.IsSelected = true;
        }
    }
}