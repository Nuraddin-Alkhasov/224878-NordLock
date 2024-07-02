﻿using HMI.Module;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VisiWin.Alarm;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;

namespace HMI.Services.Custom_Objects
{
    class Preheating : PHZTemperatureAverage
    {
        public Preheating()
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

        IVariable VWV_Order_Id_MC1;
        public string VN_Order_Id_MC1 { set { VWV_Order_Id_MC1 = VS.GetVariable(value); } }

        IVariable VWV_Order_Id_MC2;
        public string VN_Order_Id_MC2 { set { VWV_Order_Id_MC2 = VS.GetVariable(value); } }

        IVariable VWV_Order_Id_E;
        public string VN_Order_Id_E { set { VWV_Order_Id_E = VS.GetVariable(value); } }

        public string VW_Station { get; set; }
        #endregion

        #region - - - Charge - - -  

        IVariable VWV_Charge_MC1;
        public string VN_Charge_MC1 { set { VWV_Charge_MC1 = VS.GetVariable(value); } }
        IVariable VWV_Charge_MC2;
        public string VN_Charge_MC2 { set { VWV_Charge_MC2 = VS.GetVariable(value); } }
        IVariable VWV_Charge_E;
        public string VN_Charge_E { set { VWV_Charge_E = VS.GetVariable(value); } }

        #endregion

        #region - - - Run - - - 
        
        IVariable VWV_Run_MC1;
        public string VN_Run_MC1 { set { VWV_Run_MC1 = VS.GetVariable(value); } }
        IVariable VWV_Run_E;
        public string VN_Run_E { set { VWV_Run_E = VS.GetVariable(value); } }

        public string VN_AverageTemperature { set; get; }
        private void WriteStartTORun(string clmn)
        {
            DataTable temp = (new LocalDBAdapter("SELECT Id " +
                                                 "FROM Charges " +
                                                 "WHERE Order_Id = " + VWV_Order_Id_MC1.Value + " AND Charge = " + VWV_Charge_MC1.Value + ";")).DB_Output();

            if (temp.Rows.Count > 0)
            {
                long Charge_Id = (long)temp.Rows[0]["Id"];

                var b = (new LocalDBAdapter("UPDATE Runs " +
                                            "SET " + clmn + " = '" + GetDataTimeNowToFormat() + "' " +
                                            "WHERE Charge_Id = " + Charge_Id + " AND Run = " + VWV_Run_MC1.Value + ";")).DB_Input();
            }
        }
        private void WriteEndTORun(string clmn)
        {
            DataTable temp = (new LocalDBAdapter("SELECT Id " +
                                                 "FROM Charges " +
                                                 "WHERE Order_Id = " + VWV_Order_Id_E.Value + " AND Charge = " + VWV_Charge_E.Value + ";")).DB_Output();

            if (temp.Rows.Count > 0)
            {
                long Charge_Id = (long)temp.Rows[0]["Id"];

                var b = (new LocalDBAdapter("UPDATE Runs " +
                                            "SET " + clmn + " = '" + GetDataTimeNowToFormat() + "' " +
                                            "WHERE Charge_Id = " + Charge_Id + " AND Run = " + VWV_Run_E.Value + ";")).DB_Input();
            }
        }

        private void WriteSetValuesTORun()
        {
            DataTable temp = (new LocalDBAdapter("SELECT Id " +
                                                 "FROM Charges " +
                                                 "WHERE Order_Id = " + VWV_Order_Id_MC1.Value + " AND Charge = " + VWV_Charge_MC1.Value + ";")).DB_Output();

            if (temp.Rows.Count > 0)
            {
                long Charge_Id = (long)temp.Rows[0]["Id"];
                string PHZTemp = Math.Round((float)ApplicationService.GetVariableValue("NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.01 Vorzone.DB Vorzone HMI.Sollwerte.Temperatur"),1).ToString();
                temp = (new LocalDBAdapter("SELECT SetValues_Id " +
                                           "FROM Runs " +
                                           "WHERE Charge_Id = " + Charge_Id + " AND Run = " + VWV_Run_MC1.Value + ";")).DB_Output();
                if (temp.Rows.Count > 0)
                {
                    long SetValues_Id = (long)temp.Rows[0]["SetValues_Id"];
                    var b = (new LocalDBAdapter("UPDATE SetValues " +
                                            "SET PHZTemp = '" + PHZTemp + "' " +
                                            "WHERE Id = " + SetValues_Id + ";")).DB_Input();
                }
                    
            }

        }
        public string GetSetValues() 
        {
           
            return "";
        }
        private void WriteActualValuesTORun()
        {
            DataTable temp = (new LocalDBAdapter("SELECT Id " +
                                                 "FROM Charges " +
                                                 "WHERE Order_Id = " + VWV_Order_Id_E.Value + " AND Charge = " + VWV_Charge_E.Value + ";")).DB_Output();
            double[] gavPHZ = GetActualValues_PHZ();
            double PHZTempMin = gavPHZ[0];
            double PHZTemp = gavPHZ[1];
            double PHZTempMax = gavPHZ[2];
            
            if (temp.Rows.Count > 0)
            {
                long Charge_Id = (long)temp.Rows[0]["Id"];
                temp = (new LocalDBAdapter("SELECT ActualValues_Id " +
                                            "FROM Runs " +
                                            "WHERE Charge_Id = " + Charge_Id + " AND Run = " + VWV_Run_E.Value + ";")).DB_Output();
                
                if (temp.Rows.Count > 0)
                {
                    long ActualValues_Id = (long)temp.Rows[0]["ActualValues_Id"];
                    

                    var b = (new LocalDBAdapter("UPDATE ActualValues " +
                                                "SET PHZTemp = '" + PHZTemp + "', PHZTempMin = '" + PHZTempMin + "', PHZTempMax = '" + PHZTempMax + "' " +
                                                "WHERE Id = " + ActualValues_Id + ";")).DB_Input();

                }
            }
           
            VS.SetValue(VN_AverageTemperature + "[" + VWV_Run_E.Value + "]", PHZTemp);
        }
        public double[] GetActualValues_PHZ()
        {
            return PHZTA.GetTemperature((uint)VWV_Order_Id_E.Value, (short)VWV_Charge_E.Value);
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
                    WriteStartTORun(StationName + "_S");
                    if (ProtocolSetValuesActive) WriteSetValuesTORun();
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
                Task.Run(() => { 
                    WriteEndTORun(StationName + "_E");
                    if (ProtocolActualValuesActive) WriteActualValuesTORun();
                });
            }
        }

        #endregion

        #region - - - Station Error - - - 

        public string VN_isMaterial_MC1 { set { VWV_isMaterial_MC1 = VS.GetVariable(value); } }
        IVariable VWV_isMaterial_MC1;
        public string VN_isMaterial_MC2 { set { VWV_isMaterial_MC2 = VS.GetVariable(value); } }
        IVariable VWV_isMaterial_MC2;

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
                    if((bool)VWV_isMaterial_MC1.Value)
                        WriteErrorToChargeMC1(TT);
                    if ((bool)VWV_isMaterial_MC2.Value)
                        WriteErrorToChargeMC2(TT);

                    for (int i = 12; i <= 16; i++)
                    {
                        if ((bool)VS.GetValue(VW_Station + "[" + i + "].Status.Charge.Material vorhanden", true))
                            WriteErrorToCharge(TT, i);
                    }
                    OLD_Error = TT;
                }
        }


        private void WriteErrorToCharge(IAlarmItem[] TT, int i)
        {

            long Order_Id = (uint)VS.GetValue(VW_Station + "[" +i + "].Header.Order Id", true);
            long Charge = (uint)VS.GetValue(VW_Station + "["+ i + "].Status.Charge.Chargen Nummer", true);
            DataTable temp = (new LocalDBAdapter("SELECT Id " +
                                                "FROM Charges " +
                                                "WHERE Order_Id = " + Order_Id + " AND Charge = " + Charge + ";")).DB_Output();
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

        private void WriteErrorToChargeMC1(IAlarmItem[] TT)
        {
            DataTable temp = (new LocalDBAdapter("SELECT Id " +
                                                "FROM Charges " +
                                                "WHERE Order_Id = " + VWV_Order_Id_MC1.Value + " AND Charge = " + VWV_Charge_MC1.Value + ";")).DB_Output();
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
                                                      "VALUES ('"+GetDataTimeNowToFormat()+"'," + Charge_Id + ",'" + AI.Text + "','" + ApplicationService.GetVariableValue("__CURRENT_USER.FULLNAME").ToString() + "')")).DB_Input();
                }
            }
        }

        private void WriteErrorToChargeMC2(IAlarmItem[] TT)
        {
            DataTable temp = (new LocalDBAdapter("SELECT Id " +
                                                "FROM Charges " +
                                                "WHERE Order_Id = " + VWV_Order_Id_MC2.Value + " AND Charge = " + VWV_Charge_MC2.Value + ";")).DB_Output();
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


        #region - - - Data Colector - - - 

        PHZTemperatureAverage PHZTA;

        static Timer PHZtimer;

        void SetUpTimer()
        {
            TimeSpan timeToGo = new TimeSpan(0, 1, 0);

            PHZtimer = new Timer(x =>
            {
                Start();
            }, null, timeToGo, Timeout.InfiniteTimeSpan);

        }
        void Start()
        {
            PHZTA.DoWork();
            SetUpTimer();
        }
        public void Stop()
        {
            if (PHZtimer != null)
            {
                PHZtimer.Dispose();
                PHZtimer = null;
            }
        }

        public void StartDataColector() 
        {
            PHZTA = new PHZTemperatureAverage();
            Start();
        }

        #endregion


    }
}