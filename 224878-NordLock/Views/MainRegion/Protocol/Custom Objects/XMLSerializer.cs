using HMI.Module;
using HMI.Resources;
using HMI.Views.MainRegion.Recipe;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HMI.Views.MainRegion.Protocol.Custom_Objects
{
    public class XMLSerializer
    {

        Order O;
        XElement XML;

        public XMLSerializer(long _Order_Id)
        {
            GetData(_Order_Id);
        }

        #region - - - Data Collector - - -
        void GetData(long _Order_Id)
        {
            Task.Run(() =>
            {
                // GET Order

                DataTable DT_O = (new LocalDBAdapter("SELECT * " +
                                                     "FROM Orders " +
                                                     "WHERE Id = " + _Order_Id.ToString())).DB_Output();
                if (DT_O.Rows.Count > 0)
                {
                    foreach (DataRow r in DT_O.Rows)
                    {

                        O = new Order()
                        {
                            Id = (long)r["Id"],
                            TimeStamp = ((DateTime)r["TimeStamp"]).ToString("dd.MM.yyyy HH:mm:ss"),
                            Data_1 = (string)r["Data_1"],
                            Data_2 = (string)r["Data_2"],
                            Data_3 = (string)r["Data_3"],
                            MR_Id = (long)r["MR_Id"],
                            MRDetailed = GetMRDetailed((long)r["MR_Id"]),
                            Charges = (long)r["Charges"],
                            User = (string)r["User"]
                        };
                    }
                }


                // GET Charges
                DataTable DT_C = (new LocalDBAdapter("SELECT * " +
                                                     "FROM Charges " +
                                                     "WHERE Order_Id = " + _Order_Id.ToString())).DB_Output();

                if (DT_C.Rows.Count > 0)
                {
                    foreach (DataRow r in DT_C.Rows)
                    {
                        O.ChargesList.Add(new Charge()
                        {
                            Id = (long)r["Id"],
                            Order_Id = (long)r["Order_Id"],
                            Box_Id = (long)r["Box_Id"],
                            ChargeNr = (long)r["Charge"],
                            Weight = (double)r["Weight"],
                            Optimized = Convert.ToBoolean(r["Optimized"]),
                            Runs = (long)r["Runs"],
                            Error = (long)r["Error"],
                            // GET Errors
                            ErrorList = GetErrors((long)r["Id"]),
                            // Get Runs
                            RunList = GetRuns((long)r["Id"]),
                            Start = r["Start"].ToString() == "" ? "" : ((DateTime)r["Start"]).ToString("dd.MM.yyyy HH:mm:ss"),
                            End = r["End"].ToString() == "" ? "" : ((DateTime)r["End"]).ToString("dd.MM.yyyy HH:mm:ss")
                        });
                        O.Weight += (double)r["Weight"];
                    }
                }
            }).ContinueWith(x =>
            {
                Serialize();
            }, TaskContinuationOptions.OnlyOnRanToCompletion);

           
        }
        MR GetMRDetailed(long _MR_Id)
        {
            MR ret_val = new MR();

            DataTable DT = (new LocalDBAdapter("SELECT * " +
                                               "FROM Recipes_MR " +
                                               "WHERE Id = " + _MR_Id.ToString())).DB_Output();

            if (DT.Rows.Count > 0)
            {
                ret_val = (new MR()
                {
                    Id = (long)DT.Rows[0]["Id"],
                    Name = (string)DT.Rows[0]["Name"],
                    Description = (string)DT.Rows[0]["Description"],
                    Article_Id = (long)DT.Rows[0]["Article_Id"],
                    Article_Name = GetArticleName((long)DT.Rows[0]["Article_Id"]),
                    C1_Id = (long)DT.Rows[0]["C1_Id"],
                    C1_Name = GetCoatingName((long)DT.Rows[0]["C1_Id"]),
                    C2_Id = (long)DT.Rows[0]["C2_Id"],
                    C2_Name = GetCoatingName((long)DT.Rows[0]["C2_Id"]),
                    C3_Id = (long)DT.Rows[0]["C3_Id"],
                    C3_Name = GetCoatingName((long)DT.Rows[0]["C3_Id"]),
                    C4_Id = (long)DT.Rows[0]["C4_Id"],
                    C4_Name = GetCoatingName((long)DT.Rows[0]["C4_Id"]),
                       
                });
                ret_val.Coatings = new string[] { ret_val.C1_Name, ret_val.C2_Name, ret_val.C3_Name, ret_val.C4_Name };
                
            }
            return ret_val;
        }
        string GetArticleName(long _Article_Id)
        {
            string ret_val = " - - - ";

            DataTable DT = (new LocalDBAdapter("SELECT * " +
                                               "FROM Recipes_Article_VW " +
                                               "WHERE Id = " + _Article_Id.ToString())).DB_Output();
            if (DT.Rows.Count > 0)
            {
                ret_val = DT.Rows[0]["Name"].ToString();
            }
            return ret_val;
        }

        string GetCoatingName(long _C_Id)
        {
            string ret_val = "-";

            DataTable DT = (new LocalDBAdapter("SELECT * " +
                                               "FROM Recipes_Coating " +
                                               "WHERE Id = " + _C_Id.ToString())).DB_Output();
            if (DT.Rows.Count > 0)
            {
                ret_val = DT.Rows[0]["Name"].ToString();
            }
            return ret_val;
        }
        ObservableCollection<Error> GetErrors(long _Charge_Id)
        {
            ObservableCollection<Error> ret_val = new ObservableCollection<Error>();

            DataTable DT = (new LocalDBAdapter("SELECT * " +
                                               "FROM Errors " +
                                               "WHERE Charge_Id = " + _Charge_Id.ToString())).DB_Output();

            if (DT.Rows.Count > 0)
            {
                foreach (DataRow r in DT.Rows)
                {
                    ret_val.Add(new Error()
                    {
                        Id = (long)r["Id"],
                        Charge_Id = (long)r["Charge_Id"],
                        TimeStamp = ((DateTime)r["TimeStamp"]).ToString("dd.MM.yyyy HH:mm:ss"),
                        Text = (string)r["Text"],
                        Comment = (string)r["Comment"],
                        User = (string)r["User"]
                    });
                }
            }
            return ret_val;
        }
        ObservableCollection<Run> GetRuns(long _Charge_Id)
        {
            ObservableCollection<Run> ret_val = new ObservableCollection<Run>();

            DataTable DT = (new LocalDBAdapter("SELECT * " +
                                               "FROM Runs " +
                                               "WHERE Charge_Id = " + _Charge_Id.ToString())).DB_Output();

            if (DT.Rows.Count > 0)
            {
                foreach (DataRow r in DT.Rows)
                {
                    ret_val.Add(new Run()
                    {
                        Id = (long)r["Id"],
                        Charge_Id = (long)r["Charge_Id"],
                        RunNr = (long)r["Run"],
                        SetValues_Id = (long)r["SetValues_Id"],
                        // GET SetValues
                        SetValues = GetSetValues((long)r["SetValues_Id"]),
                        ActualValues_Id = (long)r["ActualValues_Id"],
                        // GET ActualValues
                        ActualValues = GetActualValues((long)r["ActualValues_Id"]),
                        Start = r["Start"].ToString() == "" ? "" : ((DateTime)r["Start"]).ToString("dd.MM.yyyy HH:mm:ss"),
                        Coating_S = r["Coating_S"].ToString() == "" ? "" : ((DateTime)r["Coating_S"]).ToString("dd.MM.yyyy HH:mm:ss"),
                        Coating_E = r["Coating_E"].ToString() == "" ? "" : ((DateTime)r["Coating_E"]).ToString("dd.MM.yyyy HH:mm:ss"),
                        Preheating_S = r["Preheating_S"].ToString() == "" ? "" : ((DateTime)r["Preheating_S"]).ToString("dd.MM.yyyy HH:mm:ss"),
                        Preheating_E = r["Preheating_E"].ToString() == "" ? "" : ((DateTime)r["Preheating_E"]).ToString("dd.MM.yyyy HH:mm:ss"),
                        Drying_S = r["Drying_S"].ToString() == "" ? "" : ((DateTime)r["Drying_S"]).ToString("dd.MM.yyyy HH:mm:ss"),
                        Drying_E = r["Drying_E"].ToString() == "" ? "" : ((DateTime)r["Drying_E"]).ToString("dd.MM.yyyy HH:mm:ss"),
                        Cooling_S = r["Cooling_S"].ToString() == "" ? "" : ((DateTime)r["Cooling_S"]).ToString("dd.MM.yyyy HH:mm:ss"),
                        Cooling_E = r["Cooling_E"].ToString() == "" ? "" : ((DateTime)r["Cooling_E"]).ToString("dd.MM.yyyy HH:mm:ss"),
                        End = r["End"].ToString() == "" ? "" : ((DateTime)r["End"]).ToString("dd.MM.yyyy HH:mm:ss"),
                    });
                }
            }
            return ret_val;
        }
        SetValues GetSetValues(long _SetValues_Id)
        {
            SetValues ret_val = new SetValues();

            DataTable DT = (new LocalDBAdapter("SELECT * " +
                                               "FROM SetValues " +
                                               "WHERE Id = " + _SetValues_Id.ToString())).DB_Output();

            if (DT.Rows.Count > 0)
            {
                foreach (DataRow r in DT.Rows)
                {
                    ret_val = (new SetValues()
                    {
                        Id = (long)r["Id"],
                        PaintType = r["PaintType"] != System.DBNull.Value ? (string)r["PaintType"] : "",
                        PaintTemp = r["PaintTemp"] != System.DBNull.Value ? (double)r["PaintTemp"] : 0,
                        PHZTemp = r["PHZTemp"] != System.DBNull.Value ? (double)r["PHZTemp"] : 0,
                        DryerTemp = r["DryerTemp"] != System.DBNull.Value ? (double)r["DryerTemp"] : 0,
                        CZTemp = r["CZTemp"] != System.DBNull.Value ? (double)r["CZTemp"] : 0,
                        Values = (new VWRecipe((string)r["Data"])).VWVariables
                    });
                }
            }
            return ret_val;
        }
        ActualValues GetActualValues(long _ActualValues_Id)
        {
            ActualValues ret_val = new ActualValues();

            DataTable DT = (new LocalDBAdapter("SELECT * " +
                                               "FROM ActualValues " +
                                               "WHERE Id = " + _ActualValues_Id.ToString())).DB_Output();

            if (DT.Rows.Count > 0)
            {
                foreach (DataRow r in DT.Rows)
                {
                    ret_val = (new ActualValues()
                    {
                        Id = (long)r["Id"],
                        
                        PaintTemp = r["PaintTemp"] != System.DBNull.Value ? (double)r["PaintTemp"] : 0,
                        PHZTempMin = r["PHZTempMin"] != System.DBNull.Value ? (double)r["PHZTempMin"] : 0,
                        PHZTemp = r["PHZTemp"] != System.DBNull.Value ? (double)r["PHZTemp"] : 0,
                        PHZTempMax = r["PHZTempMax"] != System.DBNull.Value ? (double)r["PHZTempMax"] : 0,
                        DryerTempMin = r["DryerTempMin"] != System.DBNull.Value ? (double)r["DryerTempMin"] : 0,
                        DryerTemp = r["DryerTemp"] != System.DBNull.Value ? (double)r["DryerTemp"] : 0,
                        DryerTempMax = r["DryerTempMax"] != System.DBNull.Value ? (double)r["DryerTempMax"] : 0,
                        CZTempMin = r["CZTempMin"] != System.DBNull.Value ? (double)r["CZTempMin"] : 0,
                        CZTemp = r["CZTemp"] != System.DBNull.Value ? (double)r["CZTemp"] : 0,
                        CZTempMax = r["CZTempMax"] != System.DBNull.Value ? (double)r["CZTempMax"] : 0,
                    });
                }
            }
            return ret_val;
        }

        #endregion

        #region - - - XML Serializer - - -
        void Serialize() 
        {
            XML = new XElement("DataPack", 
                        new XAttribute("Id", O.Id),
                        new XElement("Header",
                        new XAttribute("TimeStamp", O.TimeStamp),
                            new XAttribute("Order", O.Data_1),
                            new XAttribute("Batch", O.Data_2),
                            new XAttribute("Item", O.Data_3),
                            
                            new XElement("MachineProgram",
                                new XAttribute("Name", O.MRDetailed.Name),
                                new XAttribute("Description", O.MRDetailed.Description),
                                new XElement("Article",
                                    new XAttribute("Name", O.MRDetailed.Article_Name)),
                                new XElement("CoatingPrograms",
                                    new XAttribute("Count", 0))
                            ),
                            new XAttribute("User", O.User)
                       )
                    );

            for (int i=0; i<=3;i++) 
            {
                if (O.MRDetailed.Coatings[i] != "-") 
                {
                    XML.Element("Header").Element("MachineProgram").Element("CoatingPrograms").Add(new XElement("CoatingProgram", 
                                                                                                        new XAttribute("Number", (i + 1).ToString()),
                                                                                                        new XAttribute("Name", O.MRDetailed.Coatings[i])
                                                                                       ));
                    XML.Element("Header").Element("MachineProgram").Element("CoatingPrograms").Attribute("Count").Value = (i + 1).ToString();
                }
            }
          
            GetCharges(XML, O.ChargesList);

            WriteXMLToFile(XML);

        }
       
        void GetCharges(XElement _XML, ObservableCollection<Charge> cl)
        {
           List<BoxGroup> BG = cl.GroupBy(
                            p => p.Box_Id,
                            (key, g) => new BoxGroup() { Box = key, Charges = g.ToList() }).ToList();

            _XML.Add(new XElement("Boxes",
                       new XAttribute("Count", BG.Count),
                       new XAttribute("Weight", Math.Round(O.Weight, 1))));

            foreach (BoxGroup box in BG) 
            {
                XElement temp = (new XElement("Box",
                                       new XAttribute("Number", box.Charges[0].Box_Id),
                                       new XElement("Charges",
                                            new XAttribute("Count", box.Charges.Count),
                                            new XAttribute("Weight", Math.Round((from x in box.Charges select x.Weight).Sum(), 1)))
                                      ));
                foreach (Charge c in box.Charges)
                {
                        temp.Element("Charges").Add(new XElement("Charge",
                                                    new XAttribute("Number", c.ChargeNr),
                                                    new XAttribute("Weight", Math.Round(c.Weight, 1)),
                                                    new XAttribute("Optimized", c.Optimized),
                                                    new XElement("TimeStamps",
                                                        new XAttribute("Start", c.Start),
                                                        new XAttribute("End", c.End)),
                                                    GetRuns(c),
                                                    GetErrors(c)
                                                    )
                        );
                }

                _XML.Element("Boxes").Add(temp);

            }
        }
        XElement GetRuns(Charge c)
        {
            XElement ret_val = new XElement("Runs",
                                            new XAttribute("Count", c.Runs));
            foreach (Run r in c.RunList)
            {
                ret_val.Add(new XElement("Run",
                                            new XAttribute("Number", r.RunNr),
                                            new XElement("TimeStamps",
                                                new XAttribute("Start", r.Start),
                                                new XAttribute("CoatingStart", r.Coating_S),
                                                new XAttribute("CoatingEnd", r.Coating_E),
                                                new XAttribute("PreheatingStart", r.Preheating_S),
                                                new XAttribute("PreheatingEnd", r.Preheating_E),
                                                new XAttribute("DryingStart", r.Drying_S),
                                                new XAttribute("DryingEnd", r.Drying_E),
                                                new XAttribute("CoolingStart", r.Cooling_S),
                                                new XAttribute("CoolingEnd", r.Cooling_E),
                                                new XAttribute("End", r.End)),
                                            GetActualValues(r),
                                            GetSetValues(r)
                                            )
                            );
            }

            return ret_val;
        }
        XElement GetErrors(Charge c)
        {
            XElement ret_val = new XElement("Errors",
                                            new XAttribute("Count", c.ErrorList.Count));
            int i = 1;
            foreach (Error r in c.ErrorList)
            {
                ret_val.Add(new XElement("Error",
                                            new XAttribute("Number", i),
                                            new XAttribute("TimeStamp", r.TimeStamp),
                                            new XAttribute("Message", r.Text),
                                            new XAttribute("User", r.User)
                                            )
                            );
                i++;
            }

            return ret_val;
        }
        XElement GetActualValues(Run r)
        {
            XElement ret_val = new XElement("Temperatures",
                                            new XElement("Paint",
                                                        new XAttribute("SetValue", r.SetValues.PaintTemp),
                                                        new XAttribute("ActualValue", r.ActualValues.PaintTemp)),
                                            new XElement("PreheatingZone",
                                                        new XAttribute("SetValue", r.SetValues.PHZTemp),
                                                        new XAttribute("Min", r.ActualValues.PHZTempMin),
                                                        new XAttribute("Average", r.ActualValues.PHZTemp),
                                                        new XAttribute("Max", r.ActualValues.PHZTempMax)),
                                            new XElement("DryingZone",
                                                        new XAttribute("SetValue", r.SetValues.DryerTemp),
                                                        new XAttribute("Min", r.ActualValues.DryerTempMin),
                                                        new XAttribute("Average", r.ActualValues.DryerTemp),
                                                        new XAttribute("Max", r.ActualValues.DryerTempMax)),
                                            new XElement("CoolingZone",
                                                        new XAttribute("SetValue", r.SetValues.CZTemp),
                                                        new XAttribute("Min", r.ActualValues.CZTempMin),
                                                        new XAttribute("Average", r.ActualValues.CZTemp),
                                                        new XAttribute("Max", r.ActualValues.CZTempMax)));

            //new XElement("Temperatures",
            //                                new XAttribute("Paint", r.ActualValues.PaintTemp),
            //                                new XAttribute("PreheatingZoneMin", r.ActualValues.PHZTempMin),
            //                                new XAttribute("PreheatingZone", r.ActualValues.PHZTemp),
            //                                new XAttribute("PreheatingZoneMax", r.ActualValues.PHZTempMax),
            //                                new XAttribute("DryingZoneMin", r.ActualValues.DryerTempMin),
            //                                new XAttribute("DryingZone", r.ActualValues.DryerTemp),
            //                                new XAttribute("DryingZoneMax", r.ActualValues.DryerTempMax),
            //                                new XAttribute("CoolingZoneMin", r.ActualValues.CZTempMin),
            //                                new XAttribute("CoolingZone", r.ActualValues.CZTemp),
            //                                new XAttribute("CoolingZoneMax", r.ActualValues.CZTempMax));
            return ret_val;
        }
        XElement GetSetValues(Run r)
        {
           
            List<List<VWVariable>> t1 = new List<List<VWVariable>>();
            int j = 0;
            List<VWVariable> t2 = new List<VWVariable>();
            
            
            for (int i=0; i<= r.SetValues.Values.Count; i++)
            {
                if (i < r.SetValues.Values.Count && r.SetValues.Values[i].Item.ToString().Contains("[" + j + "]"))
                {
                    t2.Add(r.SetValues.Values[i]);
                }
                else 
                { 
                    j++;
                    if (i < r.SetValues.Values.Count)
                    {
                        i--;
                        t1.Add(t2);
                        t2 = new List<VWVariable>();
                    }
                    else
                    {
                        t1.Add(t2);
                        t2 = new List<VWVariable>();
                    } 
                }
                
                //ret_val.Add(new XElement(GetVariableName(v).Name,
                //                            new XAttribute("Value", GetVariableName(v).Value)
                //                            )
                //            );
            }

            XElement ret_val = new XElement("CoatingProgram",
                                           new XAttribute("Name", O.MRDetailed.Coatings[r.RunNr - 1]),
                                           new XAttribute("PaintType", r.SetValues.PaintType),
                                           new XElement("Steps",
                                            new XAttribute("Count", (from x in t1 where x.Count > 1 select x).Count())));

            int k = 1;
            foreach (List<VWVariable> l in t1) 
            {
                if (l.Count > 1) 
                {
                    XElement temp = new XElement("Step",
                        new XAttribute("Number", k),
                        new XAttribute("Type", GetVariableName(l[0]).Value));
                    
                    for (int i=1; i<l.Count;i++) 
                    {
                        temp.Add(new XAttribute(GetVariableName(l[i]).Name, GetVariableName(l[i]).Value));
                    }
                    ret_val.Element("Steps").Add(temp);
                    k++;
                }
            }

            return ret_val;
        }
        Helper GetVariableName(VWVariable v) 
        {
           return new Helper()
            {
                Name = ConvertName(v),
                Value = ConvertValue(v) 
            };
        }

        string ConvertName(VWVariable v)
        {
            string ret_val = "";

            ret_val = v.Item.ToString().Contains("Tauchen / Schleudern / Wälzen") ? "Step" : ret_val;

            ret_val = v.Item.ToString().Contains("Tauchen.Tauchzeit") ? "DippingTime" : ret_val;
            ret_val = v.Item.ToString().Contains("Tauchen.Reversierzeit") ? "ReversingTime" : ret_val;
            ret_val = v.Item.ToString().Contains("Tauchen.Rpm Körbe tauchen") ? "SpinningSpeed" : ret_val;


            ret_val = v.Item.ToString().Contains("Schleudern.Rotor Drehzeit 1") ? "SpinningTime1" : ret_val;
            ret_val = v.Item.ToString().Contains("Schleudern.Rotor Drehzahl 1") ? "SpinningSpeed1" : ret_val;
            ret_val = v.Item.ToString().Contains("Schleudern.Planet Drehzeit 2") ? "PlanetTime2" : ret_val;
            ret_val = v.Item.ToString().Contains("Schleudern.Planet Drehzahl 2") ? "PlanetSpeed2" : ret_val;
            ret_val = v.Item.ToString().Contains("Schleudern.Rotor Drehzahl 2") ? "SpinningSpeed2" : ret_val;
            ret_val = v.Item.ToString().Contains("Schleudern.Rotor Drehzeit 3") ? "SpinningTime3" : ret_val;
            ret_val = v.Item.ToString().Contains("Schleudern.Rotor Drehzahl 3") ? "SpinningSpeed3" : ret_val;

            ret_val = v.Item.ToString().Contains("Wälzen.Kippwinkel") ? "TiltAngle" : ret_val;
            ret_val = v.Item.ToString().Contains("Wälzen.Korbdrehzahl") ? "SpinningSpeed" : ret_val;
            ret_val = v.Item.ToString().Contains("Wälzen.Reversierzeit") ? "ReversingTime" : ret_val;
            ret_val = v.Item.ToString().Contains("Wälzen.Wälzzeit") ? "RollingTime" : ret_val;
            return ret_val;
        }
        string ConvertValue(VWVariable v)
        {
            string ret_val = "-";
            if (v.Item.ToString().Contains("Tauchen / Schleudern / Wälzen"))
            {
                switch (v.Value.ToString()) 
                {
                   
                    case "1": ret_val = "Dipping"; break;
                    case "2": ret_val = "Spinning"; break;
                    case "3": ret_val = "Rolling"; break;
                    default: ret_val = "-"; break;
                }
                
            }
            else 
            {
                ret_val = v.Value.ToString();
            }
               
            return ret_val;
        }

        class Helper 
        {
            public string Name { set; get; }
            public string Value { set; get; }
        }

        class BoxGroup 
        {
            public long Box { set; get; }
            public List<Charge> Charges { set; get; }
    }

        #endregion

        #region - - - File Writer - - -
        void WriteXMLToFile(XElement _XML)
        {
            string path = (new Resources.LocalResources()).Paths.Project.QData;
            if (!Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            File.WriteAllText((new LocalResources()).Paths.Project.QData + "\\" + O.Data_2+"_"+O.Id.ToString()+".xml", _XML.ToString());
        }

        #endregion

    }
}
