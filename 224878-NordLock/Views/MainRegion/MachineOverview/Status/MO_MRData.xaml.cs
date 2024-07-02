using HMI.Module;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using VisiWin.Recipe;

namespace HMI.Views.MainRegion
{

	[ExportView("MO_MRData")]
	public partial class MO_MRData
    {
        public MO_MRData()
		{
			this.InitializeComponent();
          
        }

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			ApplicationService.SetView("MessageBoxRegion", "EmptyView");
		}

		private void View_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
            if (this.IsVisible) 
            {
                Task obTask = Task.Run(() => {
                    long MR_Id = (long)ApplicationService.ObjectStore.GetValue("MO_MRData_KEY");
                    ApplicationService.ObjectStore.Remove("MO_MRData_KEY");
                    DataTable DT = (new LocalDBAdapter("SELECT * " +
                                                        "FROM Recipes_MR " +
                                                        "WHERE Id = " + MR_Id + "; ")).DB_Output();
                    if (DT.Rows.Count > 0)
                    {
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            MR.Value = (string)DT.Rows[0]["Name"];
                            Descr.Value = (string)DT.Rows[0]["Description"];

                            LC.Value = ((DateTime)DT.Rows[0]["LastChanged"]).ToString("dd.MM.yyyy HH:mm:ss");
                            User.Value = (string)DT.Rows[0]["User"];

                            SetCoatingLayerNames(C1, (long)DT.Rows[0]["C1_Id"]);
                            SetCoatingLayerNames(C2, (long)DT.Rows[0]["C2_Id"]);
                            SetCoatingLayerNames(C3, (long)DT.Rows[0]["C3_Id"]);
                            SetCoatingLayerNames(C4, (long)DT.Rows[0]["C4_Id"]);

                        }));
                      
                    }
                });
            }
        }

        void SetCoatingLayerNames(TextVarOut C, long _id)
        {
            if (_id != -1)
            {
                DataTable DT = (new LocalDBAdapter("SELECT * " +
                                                  "FROM Recipes_Coating " +
                                                  "WHERE Id = " + _id + "; ")).DB_Output();

                if (DT.Rows.Count > 0)
                {
                    C.Visibility = Visibility.Visible;
                    C.Value = (string)DT.Rows[0]["Name"];
                }
            }
            else 
            {
                C.Value = "";
                C.Visibility = Visibility.Collapsed;
            }
        }
    }
}