using VisiWin.ApplicationFramework;

namespace HMI.User
{
	/// <summary>
	/// Interaction logic for View2.xaml
	/// </summary>
	[ExportView("User_PN")]
	public partial class User_PN : VisiWin.Controls.View
	{

		
		public User_PN()
		{
			this.InitializeComponent();
		
		}

        private void Pn_benutzerverwaltung_SelectedPanoramaRegionChanged(object sender, VisiWin.Controls.SelectedPanoramaRegionChangedEventArgs e)
        {
            if (pn_benutzerverwaltung.SelectedPanoramaRegionIndex == 0)
            {
                header.LocalizableText = "@UserManagement.Text4";
            }
            else
            {
                header.LocalizableText = "@UserManagement.Text5";
            }
        }
    }
}