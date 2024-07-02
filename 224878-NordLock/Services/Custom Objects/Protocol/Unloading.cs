using HMI.Module;
using HMI.Views.MainRegion.Protocol.Custom_Objects;
using System;
using System.Data;
using System.Threading.Tasks;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;

namespace HMI.Services.Custom_Objects
{
    class Unloading
    {
        public Unloading()
        {
            VS = ApplicationService.GetService<IVariableService>();
        }

        IVariableService VS;
        public string StationName { get; set; }
        public bool ProtocolSetValuesActive { get; set; }

        #region - - - Order - - -  

        IVariable VWV_Order_Id;
        public string VN_Order_Id { set { VWV_Order_Id = VS.GetVariable(value); } }

        #endregion

        #region - - - Charge - - -  

        IVariable VWV_Charge;
        public string VN_Charge { set { VWV_Charge = VS.GetVariable(value); } }
       
        IVariable VWV_EndCharge;
        public string VN_EndCharge
        {
            set
            {
                VWV_EndCharge = VS.GetVariable(value);
                VWV_EndCharge.Change += Event_EndCharge;
            }
        }
        private void Event_EndCharge(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue && (bool)e.Value)
            {
                VWV_EndCharge.Value = false;
                Task.Run(() => {
                    WriteEndToRun();
                    WriteEndToCharge();
                    if ((bool)VWV_Discharged.Value) { WriteMessageToCharge(); }
                    
                }).ContinueWith(x =>
                {
                    GenerateQualityDataFile();
                }, TaskContinuationOptions.OnlyOnRanToCompletion); 
            }
        }

        private void WriteEndToCharge()
        {
            DataTable temp = (new LocalDBAdapter("SELECT Id " +
                                                 "FROM Charges " +
                                                 "WHERE Order_Id = " + VWV_Order_Id.Value + " AND Charge = " + VWV_Charge.Value + ";")).DB_Output();


            if (temp.Rows.Count > 0)
            {
                long Charge_Id = (long)temp.Rows[0]["Id"];
                var b = (new LocalDBAdapter("UPDATE Charges " +
                                            "SET End = '" + GetDataTimeNowToFormat() + "' " +
                                            "WHERE Id = " + Charge_Id + ";")).DB_Input();
            }

        }

        #endregion

        #region - - - Run - - - 
        
        IVariable VWV_Run;
        public string VN_Run { set { VWV_Run = VS.GetVariable(value); } }
      

      
        private void WriteEndToRun()
        {
            DataTable temp = (new LocalDBAdapter("SELECT Id " +
                                                 "FROM Charges " +
                                                 "WHERE Order_Id = " + VWV_Order_Id.Value + " AND Charge = " + VWV_Charge.Value + ";")).DB_Output();


            if (temp.Rows.Count > 0)
            {
                long Charge_Id = (long)temp.Rows[0]["Id"];
                var b = (new LocalDBAdapter("UPDATE Runs " +
                                            "SET End = '" + GetDataTimeNowToFormat() + "' " +
                                            "WHERE Charge_Id = " + Charge_Id + " AND Run = " + VWV_Run.Value + ";")).DB_Input();
            }
            
        }


        #endregion

        #region - - - Station Error - - - 
        public string VN_Discharged { set { VWV_Discharged = VS.GetVariable(value); } }
        IVariable VWV_Discharged;
        private void WriteMessageToCharge()
        {
            DataTable temp = (new LocalDBAdapter("SELECT Id " +
                                                "FROM Charges " +
                                                "WHERE Order_Id = " + VWV_Order_Id.Value + " AND Charge = " + VWV_Charge.Value + ";")).DB_Output();
            if (temp.Rows.Count > 0)
            {
                string Charge_Id = temp.Rows[0]["Id"].ToString();

                var a = (new LocalDBAdapter("UPDATE Charges " +
                                            "SET Error = 2 " +
                                            "WHERE Id = " + Charge_Id + ";").DB_Input());

                string text = ApplicationService.GetText("@Protocol.Text51");
                bool result = (new LocalDBAdapter("INSERT " +
                                                  "INTO Errors (TimeStamp, Charge_Id, Text, User) " +
                                                  "VALUES ('" + GetDataTimeNowToFormat() + "'," + Charge_Id + ",'" + text + "','" + ApplicationService.GetVariableValue("__CURRENT_USER.FULLNAME").ToString() + "')")).DB_Input();
                
            }
        }

        #endregion

        #region - - - Station - - - 

        private string GetDataTimeNowToFormat()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        #endregion

        #region - - - Quality Data - - -
        private void GenerateQualityDataFile () 
        {
            DataTable DT = (new LocalDBAdapter("SELECT * " +
                                                             "FROM Charges " +
                                                             "WHERE Order_Id = " + VWV_Order_Id.Value + " AND End Is NULL;")).DB_Output();
            if (DT.Rows.Count == 0)
            {
                new XMLSerializer((uint)VWV_Order_Id.Value);
            }
        }

        #endregion

    }
}
