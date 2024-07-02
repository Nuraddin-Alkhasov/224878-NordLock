
using System.Windows.Controls;
using System.Windows.Input;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Dashboard
{
    /// <summary>
    /// Auswahldialog für vorhandene Widgets im Projekt
    /// </summary>
    [ExportView("DB_WidgetSelector")]
    public partial class DB_WidgetSelector : View
    {
        public DB_WidgetSelector()
        {
            this.InitializeComponent();
        }
        private void gi_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            list.UnselectAll();
            ((System.Windows.Controls.ListBoxItem)sender).IsSelected = true;
        }
    }
}