using VisiWin.ApplicationFramework;

namespace HMI.User
{
	/// <summary>
	/// Interaction logic for UserAdd.xaml
	/// </summary>
    [ExportView("User_ChangePassword")]
	public partial class User_ChangePassword : VisiWin.Controls.View, IView
	{
		public User_ChangePassword()
		{
			this.InitializeComponent();
		}
	}
}