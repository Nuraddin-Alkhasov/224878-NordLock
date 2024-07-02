using System.ComponentModel.Composition;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using System;
using System.Data;
using HMI.Module;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;

namespace HMI.Dashboard
{
    /// <summary>
    /// Interaction logic for DashboardWidgetBar.xaml
    /// </summary>
    [ExportDashboardWidget("DB_Widget_Prod4", "Dashboard.Text14", "@Dashboard.Text13", 1, 1)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class DB_Widget_Prod4 : View
    {

        public DB_Widget_Prod4()
        {
            InitializeComponent();
            BackgroundWorker BGW = new BackgroundWorker();
            BGW.DoWork += BGW_DoWork;
            BGW.RunWorkerCompleted += BGW_RunWorkerCompleted;
            BGW.RunWorkerAsync();
           
            
        }

        private void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            gauge.Value = Weight;
        }
        double Weight = 0;
        private void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            DataTable temp = (new LocalDBAdapter("SELECT SUM(Weight) as Weight " +
                                               "FROM Charges " +
                                               "WHERE Start >= '" + DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd HH:mm:ss") + "' AND Start<='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "';")).DB_Output();
            if (temp.Rows.Count == 0)
            { Weight = 0; }
            else 
            {
                if (temp.Rows[0]["Weight"] != System.DBNull.Value)
                {
                    Weight = Convert.ToDouble(temp.Rows[0]["Weight"]);
                }
                else
                {
                    Weight = 0;
                }
            }
            
        }
    }
}
