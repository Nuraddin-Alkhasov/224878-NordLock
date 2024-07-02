using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisiWin.ApplicationFramework;

namespace HMI
{
    /// <summary>
    /// Interaction logic for AlarmHistoryView.xaml
    /// </summary>
    [ExportView("LoggingView")]
    public partial class LoggingView : VisiWin.Controls.View
    {
        public LoggingView()
        {
            this.InitializeComponent();
        }

        int oldIndex = 0;
        private void DataGridRow_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            dgv_logging.UnselectAllCells();
            ((DataGridRow)sender).IsSelected = true;
            oldIndex = dgv_logging.SelectedIndex;
        }

        private void LayoutRoot_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                if (dgv_logging.Items.Count >= 1)
                    dgv_logging.SelectedIndex = oldIndex;
                else
                    dgv_logging.SelectedIndex = -1;
            }
        }
    }
}