using VisiWin.ApplicationFramework;

namespace HMI.Views.MainRegion.OperatingHours
{
	/// <summary>
	/// Interaction logic for BSStepEdit.xaml
	/// </summary>
	[ExportView("OH_M4_Mani")]
	public partial class OH_M4_Mani : VisiWin.Controls.View
	{
       
        public OH_M4_Mani()
		{
			this.InitializeComponent();
        }

        private void Btn1_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            btn1.IsSelected = true;
        }

        private void Btn2_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            btn2.IsSelected = true;
        }

        private void Btn3_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            btn3.IsSelected = true;
        }
        private void Btn4_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            btn4.IsSelected = true;
        }
        private void Btn5_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            btn5.IsSelected = true;
        }
    }
}