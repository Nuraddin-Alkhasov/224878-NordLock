using VisiWin.ApplicationFramework;

namespace HMI.Parameter
{
	/// <summary>
	/// Interaction logic for Parameter.xaml
	/// </summary>
	[ExportView("Parameter_Main")]
	public partial class Parameter_Main : VisiWin.Controls.View
	{
		public Parameter_Main()
		{
			this.InitializeComponent();
		}
	}
}