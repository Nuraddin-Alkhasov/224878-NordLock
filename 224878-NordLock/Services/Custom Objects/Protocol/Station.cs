using HMI.Module;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using VisiWin.Alarm;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;

namespace HMI.Services.Custom_Objects
{
    class Station
    {
        public Station()
        {
            VS = ApplicationService.GetService<IVariableService>();
            OLD_Error = new IAlarmItem[0];
            CurrentAlarmList = ApplicationService.GetService<IAlarmService>().GetCurrentAlarms2();
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
        public string VN_FirstCharge { set; get; }
        public string VN_LastCharge { set; get; }
        public string VN_LastStationOrderId { set; get; }
        public string VN_LastStationBoxNumber { set; get; }

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

            if (Charge == "1") 
            {
                ApplicationService.SetVariableValue(VN_FirstCharge, true);
            }
            VWV_Charge.Value = Charge;
            VWV_Run.Value = 1;
            long LS_Order_Id = (uint)ApplicationService.GetVariableValue(VN_LastStationOrderId);
            long LS_BoxNumber = (short)ApplicationService.GetVariableValue(VN_LastStationBoxNumber);
            if (LS_Order_Id != (uint)VWV_Order_Id.Value && LS_BoxNumber != (short)VWV_Box_Id.Value)
            {
                ApplicationService.SetVariableValue(VN_LastCharge, true);
            }

            var a = (new LocalDBAdapter("INSERT " +
                                        "INTO Charges (Start, Order_Id, Box_Id, Charge, Weight, Optimized) " +
                                        "VALUES ('" + GetDataTimeNowToFormat() + "'," + VWV_Order_Id.Value + "," + VWV_Box_Id.Value + "," + Charge +","+ VWV_Weight.Value.ToString() + ","+ VWV_Optimized.Value +");")).DB_Input();

            var c = (new LocalDBAdapter("UPDATE Orders " +
                                        "SET Charges = " + Charge + " " +
                                        "WHERE Id = " + VWV_Order_Id.Value + ";")).DB_Input();
        }
        
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
                VWV_NewRun.Value = false;
                Task.Run(() => { WriteEndToCharge(); });
            }
        }

        private void WriteEndToCharge()
        {
            DataTable temp = (new LocalDBAdapter("SELECT Id " +
                                                 "FROM Charges " +
                                                 "WHERE Order_Id = " + VWV_Order_Id.Value + " AND Charge = " + VWV_Charge.Value + ";")).DB_Output();


            if (temp.Rows.Count > 0)
            {
                string Charge_Id = (string)temp.Rows[0]["Id"];
                var b = (new LocalDBAdapter("UPDATE Charges " +
                                            "SET End = '" + GetDataTimeNowToFormat() + "' " +
                                            "WHERE Id = " + Charge_Id + ";")).DB_Input();
            }

        }

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
                Task.Run(() => { WriteNewRun(); });
            }
        }
        private void WriteNewRun()
        {
            DataTable temp = (new LocalDBAdapter("SELECT Id " +
                                                 "FROM Charges " +
                                                 "WHERE Order_Id = " + VWV_Order_Id.Value + " AND Charge = " + VWV_Charge.Value + ";")).DB_Output();


            if (temp.Rows.Count > 0)
            {
                long Charge_Id = (long)temp.Rows[0]["Id"];

                var a = (new LocalDBAdapter("INSERT " +
                                            "INTO Runs (Start, Charge_Id, Run) " +
                                            "VALUES ('" + GetDataTimeNowToFormat() + "'," + Charge_Id + "," + VWV_Run.Value + ");")).DB_Input();

                var c = (new LocalDBAdapter("UPDATE Charges " +
                                            "SET Runs = " + VWV_Run.Value + " " +
                                            "WHERE Id = " + Charge_Id + ";")).DB_Input();
            }
        }

        IVariable VWV_EndRun;
        public string VN_EndRun
        {
            set
            {
                VWV_EndRun = VS.GetVariable(value);
                VWV_EndRun.Change += Event_EndRun;
            }
        }
        private void Event_EndRun(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue && (bool)e.Value)
            {
                VWV_EndRun.Value = false;
                Task.Run(() => { WriteEndToRun(); });
            }
        }
        private void WriteEndToRun()
        {
            DataTable temp = (new LocalDBAdapter("SELECT Id " +
                                                 "FROM Charges " +
                                                 "WHERE Order_Id = " + VWV_Order_Id.Value + " AND Charge = " + VWV_Charge.Value + ";")).DB_Output();


            if (temp.Rows.Count > 0)
            {
                string Charge_Id = (string)temp.Rows[0]["Id"];
                var b = (new LocalDBAdapter("UPDATE Runs " +
                                            "SET End = '" + GetDataTimeNowToFormat() + "' " +
                                            "WHERE Charge_Id = " + Charge_Id + " AND Run = " + VWV_Run.Value + ";")).DB_Input();
            }
            
        }
        private void WriteDateTimeTORun(string clmn)
        {
            DataTable temp = (new LocalDBAdapter("SELECT Id " +
                                                 "FROM Charges " +
                                                 "WHERE Order_Id = " + VWV_Order_Id.Value + " AND Charge = " + VWV_Charge.Value + ";")).DB_Output();

            if (temp.Rows.Count > 0)
            {
                long Charge_Id = (long)temp.Rows[0]["Id"];

                var b = (new LocalDBAdapter("UPDATE Runs " +
                                            "SET " + clmn + " = '" + GetDataTimeNowToFormat() + "' " +
                                            "WHERE Charge_Id = " + Charge_Id + " AND Run = " + VWV_Run.Value + ";")).DB_Input();
            }
        }
        private void WriteSetValuesRun()
        {
            DataTable temp = (new LocalDBAdapter("SELECT Id " +
                                                "FROM Charges " +
                                                "WHERE Order_Id = " + VWV_Order_Id.Value + " AND Charge = " + VWV_Charge.Value + ";")).DB_Output();

            if (temp.Rows.Count > 0)
            {
                long Charge_Id = (long)temp.Rows[0]["Id"];
                bool result = (new LocalDBAdapter("INSERT " +
                                                  "INTO SetValues (Data) " +
                                                  "VALUES ('" + GetSetValues() + "');")).DB_Input();
               
                temp = (new LocalDBAdapter("SELECT MAX(Id) as Id " +
                                           "FROM SetValues;")).DB_Output();

                if (temp.Rows.Count > 0)
                {
                    long SetValue_Id = (long)temp.Rows[0]["Id"];

                    var b = (new LocalDBAdapter("UPDATE Runs " +
                            "SET SetValues_Id = '" + SetValue_Id + "' " +
                            "WHERE Charge_Id = " + Charge_Id + " AND Run = " + VWV_Run.Value + ";")).DB_Input();

                }



            }
            
        }
        public string GetSetValues() 
        {
            List<object> values= new List<object>();
           
            for (int i = 0; i <= 7; i++)
            {
                values.Add(new XElement("V", GetValueData("NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung PD Buffer.CoatingStep[" + i + "].Tauchen / Schleudern / Wälzen")));
                switch (((XElement)values[values.Count - 1]).Attribute("Value").Value)
                {
                    case "1":
                        values.Add(new XElement("V", GetValueData("NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung PD Buffer.CoatingStep[" + i + "].Tauchen.Reversierzeit")));
                        values.Add(new XElement("V", GetValueData("NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung PD Buffer.CoatingStep[" + i + "].Tauchen.Rpm Körbe tauchen")));
                        values.Add(new XElement("V", GetValueData("NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung PD Buffer.CoatingStep[" + i + "].Tauchen.Tauchzeit")));
                        break;
                    case "2":
                        values.Add(new XElement("V", GetValueData("NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung PD Buffer.CoatingStep[" + i + "].Schleudern.Planet Drehzahl 2")));
                        values.Add(new XElement("V", GetValueData("NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung PD Buffer.CoatingStep[" + i + "].Schleudern.Planet Drehzeit 2")));
                        values.Add(new XElement("V", GetValueData("NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung PD Buffer.CoatingStep[" + i + "].Schleudern.Rotor Drehzahl 1")));
                        values.Add(new XElement("V", GetValueData("NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung PD Buffer.CoatingStep[" + i + "].Schleudern.Rotor Drehzahl 2")));
                        values.Add(new XElement("V", GetValueData("NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung PD Buffer.CoatingStep[" + i + "].Schleudern.Rotor Drehzahl 3")));
                        values.Add(new XElement("V", GetValueData("NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung PD Buffer.CoatingStep[" + i + "].Schleudern.Rotor Drehzeit 1")));
                        values.Add(new XElement("V", GetValueData("NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung PD Buffer.CoatingStep[" + i + "].Schleudern.Rotor Drehzeit 3")));
                        break;
                    case "3":
                        values.Add(new XElement("V", GetValueData("NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung PD Buffer.CoatingStep[" + i + "].Wälzen.Kippwinkel")));
                        values.Add(new XElement("V", GetValueData("NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung PD Buffer.CoatingStep[" + i + "].Wälzen.Korbdrehzahl")));
                        values.Add(new XElement("V", GetValueData("NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung PD Buffer.CoatingStep[" + i + "].Wälzen.Reversierzeit")));
                        values.Add(new XElement("V", GetValueData("NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung PD Buffer.CoatingStep[" + i + "].Wälzen.Wälzzeit")));
                        break;
                }
            }
            return (string) (new XElement("Values", values.ToArray())).ToString();
        }
        List<object> GetValueData(string Item) 
        {
            List<object> ret_val = new List<object>();
            ret_val.Add(new XAttribute("Item", Item));
            ret_val.Add(new XAttribute("Type", VS.GetVariableDefinition(Item).DataType));
            ret_val.Add(new XAttribute("Hex", "00000000"));
            ret_val.Add(new XAttribute("Value", VS.GetValue(Item, true)));

           return ret_val;
        }
        #endregion

        #region - - - Station - - - 

        #region - - - Station Start - - - 

        IVariable VWV_Start;
        public string VN_Start
        {

            set
            {
                VWV_Start = VS.GetVariable(value);
                VWV_Start.Change += Event_StartChange;
            }
        }
        private void Event_StartChange(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue && (bool)e.Value)
            {
                VWV_Start.Value = false;
                Task.Run(() =>
                {
                    WriteDateTimeTORun(StationName + "_S");
                    if(ProtocolSetValuesActive) WriteSetValuesRun();
                });
            }
        }

        #endregion

        #region - - - Station End - - - 

        IVariable VWV_End;
        public string VN_End
        {
            set
            {
                VWV_End = VS.GetVariable(value);
                VWV_End.Change += Event_EndChange;
            }
        }
        private void Event_EndChange(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue && (bool)e.Value)
            {
                VWV_End.Value = false;
                Task.Run(() => { WriteDateTimeTORun(StationName + "_E"); });
            }
        }

        #endregion

        #region - - - Station Error - - - 

        IAlarmItem[] OLD_Error;
        ICurrentAlarms2 CurrentAlarmList;
        public int StartEr { set; get; }
        public int EndEr { set; get; }

        IVariable VWV_Error;
        public string VN_Error
        {
            set
            {
                VWV_Error = VS.GetVariable(value);
                VWV_Error.Change += Event_ErrorChange;
            }
        }

        private void Event_ErrorChange(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue && (bool)e.Value)
            {
                VWV_Error.Value = false;
                Task.Run(() => WriteErrors());
            }
        }

        private void WriteErrors()
        {
            IAlarmItem[] TT = CurrentAlarmList.Alarms.Where(x => x.Group.Name == "Errors" && x.AlarmState == AlarmState.Active && x.Param2 >= StartEr && x.Param2 <= EndEr).ToArray();

            if (TT.Length != 0)
                if (!Enumerable.SequenceEqual(TT, OLD_Error))
                {
                    WriteErrorToCharge(TT);
                    OLD_Error = TT;
                }
        }
        private void WriteErrorToCharge(IAlarmItem[] TT)
        {
            DataTable temp = (new LocalDBAdapter("SELECT Id " +
                                                "FROM Charges " +
                                                "WHERE Order_Id = " + VWV_Order_Id.Value + " AND Charge = " + VWV_Charge.Value + ";")).DB_Output();
            if (temp.Rows.Count > 0)
            {
                string Charge_Id = temp.Rows[0]["Id"].ToString();

                var a = (new LocalDBAdapter("UPDATE Charges " +
                                            "SET Error = 1 " +
                                            "WHERE Id = " + Charge_Id + ";").DB_Input());

                foreach (IAlarmItem AI in TT)
                {
                    bool result = (new LocalDBAdapter("INSERT " +
                                                      "INTO Errors (TimeStamp, Charge_Id, Text, User) " +
                                                      "VALUES ('" + GetDataTimeNowToFormat() + "'," + Charge_Id + ",'" + AI.Text + "','" + ApplicationService.GetVariableValue("__CURRENT_USER.FULLNAME").ToString() + "')")).DB_Input();
                }
            }
        }

        #endregion

        private string GetDataTimeNowToFormat()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        #endregion



    }
}
