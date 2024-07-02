using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisiWin.ApplicationFramework;

namespace HMI.User
{
	/// <summary>
	/// Interaction logic for GroupManager.xaml
	/// </summary>
	[ExportView("User_GroupManager")]
	public partial class User_GroupManager : VisiWin.Controls.View
	{
		public User_GroupManager()
		{
			this.InitializeComponent();
		}
        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Visible;
            ButtonOpenMenu.Visibility = Visibility.Collapsed;

        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            ButtonOpenMenu.Visibility = Visibility.Visible;

        }
        int oldIndex = 0;
        private void DataGridRow_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            dgv_usergroups.UnselectAllCells();
            ((DataGridRow)sender).IsSelected = true;
            oldIndex = dgv_usergroups.SelectedIndex;
        }

        private void LayoutRoot_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                if (dgv_usergroups.Items.Count >= 1)
                    dgv_usergroups.SelectedIndex = oldIndex;
                else
                    dgv_usergroups.SelectedIndex = -1;
            }
        }
    }
}