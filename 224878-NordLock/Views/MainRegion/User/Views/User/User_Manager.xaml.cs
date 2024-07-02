using HMI.Views.DialogRegion;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisiWin.ApplicationFramework;

namespace HMI.User
{
	/// <summary>
	/// Interaction logic for UserManager.xaml
	/// </summary>
	[ExportView("User_Manager")]
	public partial class User_Manager : VisiWin.Controls.View
	{
		public User_Manager()
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

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            DialogView.Show("ServiceChecker", "Jumo Checker", DialogButton.Close, DialogResult.OK);
        }


        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            DialogView.Show("Backup", "Backup", DialogButton.Close, DialogResult.OK);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogView.Show("Barcode", "Barcode", DialogButton.Close, DialogResult.OK);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogView.Show("EKS", "EKS System", DialogButton.Close, DialogResult.OK);
        }
        int oldIndex = 0;
        private void DataGridRow_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            dgv_users.UnselectAllCells();
            ((DataGridRow)sender).IsSelected = true;
            oldIndex = dgv_users.SelectedIndex;
        }

        private void LayoutRoot_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                if(dgv_users.Items.Count >=1)
                    dgv_users.SelectedIndex = oldIndex;
                else
                    dgv_users.SelectedIndex = -1;
            }
        }
    }
}