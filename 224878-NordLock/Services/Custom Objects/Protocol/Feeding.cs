using HMI.Module;
using System;
using System.Data;
using System.Threading.Tasks;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;

namespace HMI.Services.Custom_Objects
{
    class Feeding
    {
        public Feeding()
        {
            VS = ApplicationService.GetService<IVariableService>();
        }

        IVariableService VS;
        public string StationName { get; set; }

        #region - - - Order - - -  

        IVariable VWV_Order_Id;
        public string VN_Order_Id { set { VWV_Order_Id = VS.GetVariable(value); } }

        #endregion

        #region - - - Charge - - -  

        IVariable VWV_Charge;
        public string VN_Charge { set { VWV_Charge = VS.GetVariable(value); } }

        IVariable VWV_Box_Id;
        public string VN_Box_Id { set { VWV_Box_Id = VS.GetVariable(value); } }

        IVariable VWV_Weight;
        public string VN_Weight { set { VWV_Weight = VS.GetVariable(value); } }

        IVariable VWV_Optimized;
        public string VN_Optimized { set { VWV_Optimized = VS.GetVariable(value); } }

        IVariable VWV_NewCharge;
        public string VN_NewCharge
        {
            set
            {
                VWV_NewCharge = VS.GetVariable(value);
                VWV_NewCharge.Change += Event_NewCharge;
            }
        }
        private void Event_NewCharge(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue && (bool)e.Value)
            {
                VWV_NewCharge.Value = false;
                Task.Run(() => 
                { 
                    WriteNewCharge();
                    WriteNewRun();
                });
            }
        }
        private void WriteNewCharge()
        {
            string Charge;
            DataTable temp = (new LocalDBAdapter("SELECT MAX(Charge) as Charge " +
                                                 "FROM Charges " +
                                                 "WHERE Order_Id = " + VWV_Order_Id.Value + ";")).DB_Output();

            if (temp.Rows[0]["Charge"] != System.DBNull.Value)
            {
                Charge = ((long)temp.Rows[0]["Charge"] + 1).ToString();
            }
            else { Charge = "1"; }

            VWV_Charge.Value = Charge;

            var a = (new LocalDBAdapter("INSERT " +
                                        "INTO Charges (Start, Order_Id, Box_Id, Charge, Weight, Optimized) " +
                                        "VALUES ('" + GetDataTimeNowToFormat() + "'," + VWV_Order_Id.Value + "," + VWV_Box_Id.Value + "," + Charge +",'"+ Math.Round((float)VWV_Weight.Value, 1).ToString().Replace(",",".") + "',"+ VWV_Optimized.Value +");")).DB_Input();

            var c = (new LocalDBAdapter("UPDATE Orders " +
                                        "SET Charges = " + Charge + " " +
                                        "WHERE Id = " + VWV_Order_Id.Value + ";")).DB_Input();
        }
        
        #endregion

        #region - - - Run - - - 
        
        IVariable VWV_Run;
        public string VN_Run { set { VWV_Run = VS.GetVariable(value); } }
       
        private void WriteNewRun()
        {
            DataTable temp = (new LocalDBAdapter("SELECT Id " +
                                                 "FROM Charges " +
                                                 "WHERE Order_Id = " + VWV_Order_Id.Value + " AND Charge = " + VWV_Charge.Value + ";")).DB_Output();


            if (temp.Rows.Count > 0)
            {
                long Charge_Id = (long)temp.Rows[0]["Id"];

                VWV_Run.Value = 1;
                var a = (new LocalDBAdapter("INSERT " +
                                            "INTO Runs (Start, Charge_Id, Run) " +
                                            "VALUES ('" + GetDataTimeNowToFormat() + "'," + Charge_Id + "," + VWV_Run.Value + ");")).DB_Input();
                
                var c = (new LocalDBAdapter("UPDATE Charges " +
                                            "SET Runs = " + VWV_Run.Value + " " +
                                            "WHERE Id = " + Charge_Id + ";")).DB_Input();
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
