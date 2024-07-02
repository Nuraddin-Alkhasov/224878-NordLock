using HMI.Module;
using HMI.Views.MessageBoxRegion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using VisiWin.Alarm;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;

namespace HMI.Services.Custom_Objects
{
    class Return
    {
        public Return()
        {
            VS = ApplicationService.GetService<IVariableService>();
        }

        readonly IVariableService VS;
        public string StationName { get; set; }

        #region - - - Order - - -  

        IVariable VWV_Order_Id;
        public string VN_Order_Id { set { VWV_Order_Id = VS.GetVariable(value); } }

        #endregion

        #region - - - Charge - - -  

        IVariable VWV_Charge;
        public string VN_Charge { set { VWV_Charge = VS.GetVariable(value); } }

        #endregion

        #region - - - Run - - - 
        
        IVariable VWV_Run;
        public string VN_Run { set { VWV_Run = VS.GetVariable(value); } }
        IVariable VWV_NewRun;
        public string VN_NewRun
        {
            set
            {
                VWV_NewRun = VS.GetVariable(value);
                VWV_NewRun.Change += Event_NewRun;
            }
        }
        private void Event_NewRun(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue && (bool)e.Value)
            {
                VWV_NewRun.Value = false;
                Task.Run(() => {
                    try
                    {
                        WriteEndToRun();
                        WriteNewRun();
                    }
                    catch (Exception ex) 
                    {
                        new MessageBoxTask(ex.ToString(), "@DB.Text1", MessageBoxIcon.Error);
                    }
                 
                
                });
            }
        }
        private void WriteNewRun()
        {
            string Run;
            DataTable temp = (new LocalDBAdapter("SELECT Id " +
                                                 "FROM Charges " +
                                                 "WHERE Order_Id = " + VWV_Order_Id.Value + " AND Charge = " + VWV_Charge.Value + ";")).DB_Output();


            if (temp.Rows.Count > 0)
            {
                string Charge_Id = temp.Rows[0]["Id"].ToString();
                Run = ((short)VWV_Run.Value + 1).ToString();
                VWV_Run.Value = Run;
                var a = (new LocalDBAdapter("INSERT " +
                                            "INTO Runs (Start, Charge_Id, Run) " +
                                            "VALUES ('" + GetDataTimeNowToFormat() + "',"+ Charge_Id + "," + Run + ");")).DB_Input();
                
                var c = (new LocalDBAdapter("UPDATE Charges " +
                                            "SET Runs = " + Run + " " +
                                            "WHERE Id = " + Charge_Id + ";")).DB_Input();
            }
        }

        private void WriteEndToRun()
        {
            DataTable temp = (new LocalDBAdapter("SELECT Id " +
                                                 "FROM Charges " +
                                                 "WHERE Order_Id = " + VWV_Order_Id.Value + " AND Charge = " + VWV_Charge.Value + ";")).DB_Output();


            if (temp.Rows.Count > 0)
            {
                string Charge_Id = temp.Rows[0]["Id"].ToString();
                var b = (new LocalDBAdapter("UPDATE Runs " +
                                            "SET End = '" + GetDataTimeNowToFormat() + "' " +
                                            "WHERE Charge_Id = " + Charge_Id + " AND Run = " + VWV_Run.Value + ";")).DB_Input();
            }
            
        }
      
        #endregion

        #region - - - Methods - - - 

        private string GetDataTimeNowToFormat()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        #endregion



    }
}
