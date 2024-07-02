using VisiWin.ApplicationFramework;

namespace HMI.User
{
	/// <summary>
	/// Interaction logic for UserAdd.xaml
	/// </summary>
    [ExportView("User_AddChangeUser")]
	public partial class User_AddChangeUser : VisiWin.Controls.View, IView
	{
		public User_AddChangeUser()
		{
			this.InitializeComponent();
		}
	}
}