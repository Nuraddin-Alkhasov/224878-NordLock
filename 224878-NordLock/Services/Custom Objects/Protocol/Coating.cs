using HMI.Module;
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
    class Coating
    {
        public Coating()
        {
            VS = ApplicationService.GetService<IVariableService>();
            OLD_Error = new IAlarmItem[0];
            CurrentAlarmList = ApplicationService.GetService<IAlarmService>().GetCurrentAlarms2();
        }

        IVariableService VS;
        public string StationName { get; set; }
        public bool ProtocolSetValuesActive { get; set; }
        public bool ProtocolActualValuesActive { get; set; }

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
        public string VN_PaintTemp { set; get; }
        public string VN_AverageTemperature { set; get; }
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
                string Paint_Id = ApplicationService.GetVariableValue("NL.PLC.Blocks.2 Modul 2.05 Tauchbecken.DB LTB HMI.Actual value.Dipping Vat.Lacktyp").ToString();
                string PaintType = ApplicationService.GetVariableValue("NL.PLC.Blocks.50 HMI.00 Allgemein.Lacktyp Namen.Lacktyp Name[" + Paint_Id + "]").ToString(); 
                    
                string PaintTemp = Math.Round((float)ApplicationService.GetVariableValue("NL.PLC.Blocks.2 Modul 2.05 Tauchbecken.DB LTB HMI.Set Value.Dipping Vat.Temperatur Kühlung.Prozess"), 1).ToString();
                
                bool result = (new LocalDBAdapter("INSERT " +
                                                  "INTO SetValues (Data, PaintType, PaintTemp) " +
                                                  "VALUES ('" + GetSetValues() + "','"+ PaintType + "','"+ PaintTemp + "');")).DB_Input();
               
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

        private void WriteActualValuesRun()
        {
            DataTable temp = (new LocalDBAdapter("SELECT Id " +
                                                "FROM Charges " +
                                                "WHERE Order_Id = " + VWV_Order_Id.Value + " AND Charge = " + VWV_Charge.Value + ";")).DB_Output();

            string temperature = GetActualValues();
            if (temp.Rows.Count > 0)
            {
                
                long Charge_Id = (long)temp.Rows[0]["Id"];
                bool result = (new LocalDBAdapter("INSERT " +
                                                  "INTO ActualValues (PaintTemp) " +
                                                  "VALUES ('" + temperature + "');")).DB_Input();


                temp = (new LocalDBAdapter("SELECT MAX(Id) as Id " +
                                           "FROM ActualValues;")).DB_Output();

                if (temp.Rows.Count > 0)
                {
                    long ActualValues_Id = (long)temp.Rows[0]["Id"];

                    var b = (new LocalDBAdapter("UPDATE Runs " +
                            "SET ActualValues_Id = '" + ActualValues_Id + "' " +
                            "WHERE Charge_Id = " + Charge_Id + " AND Run = " + VWV_Run.Value + ";")).DB_Input();

                }



            }
            VS.SetValue(VN_AverageTemperature + "[" + VWV_Run.Value + "]", temperature);

        }
        public string GetActualValues()
        {
            return Math.Round((float)VS.GetValue(VN_PaintTemp, true), 1).ToString();
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
                    if (ProtocolSetValuesActive) WriteSetValuesRun();
                    if (ProtocolActualValuesActive) WriteActualValuesRun();
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
                                                      "VALUES ('"+ GetDataTimeNowToFormat() + "'," + Charge_Id + ",'" + AI.Text + "','" + ApplicationService.GetVariableValue("__CURRENT_USER.FULLNAME").ToString() + "')")).DB_Input();
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
