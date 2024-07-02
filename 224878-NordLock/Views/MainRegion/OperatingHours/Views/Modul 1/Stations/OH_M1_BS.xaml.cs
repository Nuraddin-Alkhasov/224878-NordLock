using VisiWin.ApplicationFramework;

namespace HMI.Views.MainRegion.OperatingHours
{
	/// <summary>
	/// Interaction logic for BSStepEdit.xaml
	/// </summary>
	[ExportView("OH_M1_BS")]
	public partial class OH_M1_BS : VisiWin.Controls.View
	{
       
        public OH_M1_BS()
		{
			this.InitializeComponent();
        }

        private void Btn1_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            btn1.IsSelected = true;
        }

      
    }
}