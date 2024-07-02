using System.ComponentModel.Composition;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;


namespace HMI.Dashboard
{
    /// <summary>
    /// Interaction logic for DashboardWidgetBar.xaml
    /// </summary>
    [ExportDashboardWidget("DB_Widget_KZT", "Dashboard.Text25", "@Dashboard.Text11", 1, 1)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class DB_Widget_KZT : View
    {
        

        public DB_Widget_KZT()
        {
            InitializeComponent();

            UC.DoWork();
        }

    


    }
}
