using VisiWin.ApplicationFramework;

namespace HMI.User
{
	/// <summary>
	/// Interaction logic for UserAdd.xaml
	/// </summary>
    [ExportView("User_AddChangeGroup")]
	public partial class User_AddChangeGroup : VisiWin.Controls.View, IView
	{
		public User_AddChangeGroup()
		{
			this.InitializeComponent();
		}
	}
}