using HMI.Interfaces;
using HMI.Module;
using HMI.Views.MainRegion.Recipe;
using HMI.Views.MessageBoxRegion;
using HMI.Views.TouchpadRegion;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;
using VisiWin.Recipe;

namespace HMI.Services
{
    [ExportService(typeof(ILiftTiltingDevice))] 
    [Export(typeof(ILiftTiltingDevice))]
    public class Service_H_LiftTiltingDevice : ServiceBase, ILiftTiltingDevice
    {
        IVariableService VS;
        IVariable MRToPLC;

        BackgroundWorker loadMR;

        public Service_H_LiftTiltingDevice()
        {
            if (ApplicationService.IsInDesignMode)
                return;
        }

        #region - - - MR Handshacke - - -

        void MRToPLC_Change(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue && bool.Parse(e.Value.ToString()))
            {
                MRToPLC.Value = false;
                loadMR.RunWorkerAsync();
            }

        }
        void W1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Application.Current.Dispatcher.InvokeAsync((Action)delegate
            {
                ApplicationService.SetView("TouchpadRegion", "WaitScreen", new WaitData(1, "@WaitScreen.Text2"));
            });

            try
            {
                MR_To_PLC();
            }
            catch { ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Handshake.from PC.Not loaded", true); }
        }

        #endregion

        #region OnProject

  
        // Hier stehen noch keine VisiWin Funktionen zur Verfügung
        protected override void OnLoadProjectStarted()
        {
            base.OnLoadProjectStarted();
        }

        // Hier kann auf die VisiWin Funktionen zugegriffen werden
        protected override void OnLoadProjectCompleted()
        {
            VS = ApplicationService.GetService<IVariableService>();
            MRToPLC = VS.GetVariable("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Handshake.to PC.GetRecipe");
            MRToPLC.Change += MRToPLC_Change;

            loadMR = new BackgroundWorker();
            loadMR.DoWork += W1_DoWork;

            base.OnLoadProjectCompleted();
        }

        // Hier stehen noch die VisiWin Funktionen zur Verfügung
        protected override void OnUnloadProjectStarted()
        {
            base.OnUnloadProjectStarted();
        }

        // Hier sind keine VisiWin Funktionen mehr verfügbar. Bei C/S ist die Verbindung zum Server schon getrennt.
        protected override void OnUnloadProjectCompleted()
        {
            base.OnUnloadProjectCompleted();
        }

        #endregion

        #region - - - Methods - - -

        #region - - - MachineRecipe - - - 

        void MR_To_PLC()
        {
            long Order_Id = SetOrderId();
            if (Order_Id != -1)
            {
                MachineRecipe MR = GetMachineRecipe(Order_Id);
                SetHeader(MR);
                LoadArticleToProcessAsync(MR);
            }
            else 
            {
                switch (Order_Id) 
                {
                    case -1 : ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Handshake.from PC.WrongSpinSpeed",true); break;
                    case -2: new MessageBoxTask("@RecipeSystem.Results.Text8", "@MessageBox.Text1", MessageBoxIcon.Error); break;
                    case -3: new MessageBoxTask("@RecipeSystem.Results.Text7", "@MessageBox.Text1", MessageBoxIcon.Error); break;

                }
            
            }
        }
        MachineRecipe GetMachineRecipe(long _id)
        {
            DataTable DT = (new LocalDBAdapter("SELECT MR_Id " +
                                                "FROM Orders " +
                                                "WHERE Id = " + _id + "; ")).DB_Output();

            if (DT.Rows.Count > 0)
            {
                long MR_Id = (long)DT.Rows[0]["MR_Id"];
                DT = (new LocalDBAdapter("SELECT * " +
                                         "FROM Recipes_MR " +
                                         "WHERE Id = " + MR_Id + "; ")).DB_Output();

                if (DT.Rows.Count > 0)
                {
                    return new MachineRecipe()
                    {
                        Id = (long)DT.Rows[0]["Id"],
                        Name = (string)DT.Rows[0]["Name"],
                        Description = (string)DT.Rows[0]["Description"],
                        Article = GetArticleData((long)DT.Rows[0]["Article_Id"]),
                        CoatingLayers = GetCoatingLayers(DT.Rows[0]),
                        Version = (string)DT.Rows[0]["Version"],
                        LastChanged = (DateTime)DT.Rows[0]["LastChanged"],
                        User = (string)DT.Rows[0]["User"]
                    };
                }
            }

            return new MachineRecipe();
        }
        ArticleRecipe GetArticleData(long A_Id)
        {
            if (A_Id != -1)
            {
                DataTable DT = (new LocalDBAdapter("SELECT Id, Name, Data " +
                                                   "FROM Recipes_Article_VW " +
                                                   "WHERE Id = " + A_Id + "; ")).DB_Output();
                if (DT.Rows.Count > 0)
                {
                    return new ArticleRecipe() 
                    {
                        Id = (long)DT.Rows[0]["Id"],
                        Name = (string)DT.Rows[0]["Name"],
                        Data = (string)DT.Rows[0]["Data"]
                    };
                }
            }
            return new ArticleRecipe();
        }
        ObservableCollection<CoatingRecipe> GetCoatingLayers(DataRow _dr) 
        {
            ObservableCollection<CoatingRecipe> CR = new ObservableCollection<CoatingRecipe>();
            for (int i = 1; i <= 10; i++)
            {
                if ((long)_dr["C" + i + "_Id"] != - 1)
                {
                    CR.Add(GetCoatingData((long)_dr["C" + i + "_Id"]));
                }
                else { CR.Add(new CoatingRecipe() { Id = 0 , Paint_Id=0}); }
            }
            return CR;
        }
        CoatingRecipe GetCoatingData(long _id)
        {
            if (_id != -1)
            {
                DataTable DT = (new LocalDBAdapter("SELECT Id, Name, Paint_Id " +
                                                  "FROM Recipes_Coating " +
                                                  "WHERE Id = " + _id + "; ")).DB_Output();

                if (DT.Rows.Count > 0)
                {
                    return new CoatingRecipe()
                    {
                        Id = (long)DT.Rows[0]["Id"],
                        Name= (string)DT.Rows[0]["Name"],
                        Paint_Id = (long)DT.Rows[0]["Paint_Id"]
                    };
                }
            }
            return new CoatingRecipe() { Id = 0, Paint_Id = 0 };
        }
        void SetHeader(MachineRecipe MR)
        {
            ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Header.MR.Article Id", MR.Article.Id);
            for (int i = 1; i <= 4; i++)
            {
                ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Header.MR.CoatingLayer " + i + ".Paint Id", MR.CoatingLayers[i - 1].Paint_Id);
                ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Header.MR.CoatingLayer " + i + ".Recipe Id", MR.CoatingLayers[i - 1].Id);
            }
        }
        async void LoadArticleToProcessAsync(MachineRecipe MR)
        {
           // Task obTask = Task.Run(() =>
          //  {
                System.IO.File.WriteAllText(MR.Article.Class.CurrentPath + @"\\" + MR.Article.Name + ".R", MR.Article.Data);
           // });

          //  obTask.ContinueWith(async (xs) =>
           // {
                var result = await MR.Article.Class.LoadFromFileToProcessAsync(MR.Article.Name);

                if (result.Result != SendRecipeResult.Succeeded)
                {
                    ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Handshake.from PC.Not loaded", true);
                    new MessageBoxTask("@RecipeSystem.Results.LoadError", "@MessageBox.Text1", MessageBoxIcon.Error);
                }
                ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Handshake.from PC.Loaded", true);
                await Dispatcher.BeginInvoke((Action)(() =>
                {
                    ApplicationService.SetView("TouchpadRegion", "EmptyView");
                }));
           // },
           // TaskContinuationOptions.OnlyOnRanToCompletion
           // );
        }

        long SetOrderId()
        {
            string Data_1 = ApplicationService.GetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Handshake.to PC.NL Data.Order#STRING12").ToString();
            string Data_2 = ApplicationService.GetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Handshake.to PC.NL Data.Batch#STRING12").ToString();
            string Data_3 = ApplicationService.GetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Handshake.to PC.NL Data.Item#STRING12").ToString();
            string Barcode = ApplicationService.GetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Handshake.to PC.NL Data.Recipe number").ToString();

            DataTable DT = (new LocalDBAdapter("SELECT MR_Id " +
                                               "FROM Barcodes " +
                                               "WHERE Barcode = '" + Barcode + "'; ")).DB_Output();
            if (DT.Rows.Count != 0)
            {
                long MR_Id = (long)(DT.Rows[0]["MR_Id"]);
                
                DT = (new LocalDBAdapter("SELECT * " +
                                         "FROM Recipes_MR " +
                                         "WHERE Id = " + MR_Id + "; ")).DB_Output();

                DataTable Article = (new LocalDBAdapter("SELECT * " +
                                                        "FROM Recipes_Article_VW " +
                                                        "WHERE Id = " + (long)(DT.Rows[0]["Article_Id"]) + "; ")).DB_Output();
                VWRecipe VWR_A = new VWRecipe("Article", Article.Rows[0]["Data"].ToString());
                long weight = Convert.ToInt64(VWR_A.VWVariables.Where(x => (string)x.Item == "NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Station.Feeding.BS.Weight per basket").ToArray()[0].Value);
                double RPM = Math.Floor(0.05 * weight * weight - 6 * weight + 365);
                for (int i = 1; i <= 4; i++) 
                {
                    long C_Id = (long)(DT.Rows[0]["C" + i.ToString() + "_Id"]);
                    if (C_Id != -1) 
                    {
                        DataTable DT2 = (new LocalDBAdapter("SELECT * " +
                                                            "FROM Recipes_Coating " +
                                                            "WHERE Id = " + C_Id + "; ")).DB_Output();

                        for (int j = 1; j <= 8; j++)
                        {
                            long S_Id = (long)(DT2.Rows[0]["S" + j.ToString() + "_Id"]);
                            if (S_Id != -1)
                            {
                                DataTable DT3 = (new LocalDBAdapter("SELECT * " +
                                                                    "FROM Recipes_CoatingStep_VW " +
                                                                    "WHERE Id = " + S_Id + " AND Type_Id = 2; ")).DB_Output();

                                if (DT3.Rows.Count > 0) 
                                {
                                    VWRecipe VWR_CS = new VWRecipe("CoatingStep", DT3.Rows[0]["Data"].ToString());
                                   // double RPM_1 = Convert.ToInt64(VWR_A.VWVariables.Where(x => (string)x.Item == "Recipe.CoatingStep.Schleudern.Rotor Drehzahl 1").ToArray()[0].Value);
                                    double RPM_2 = Convert.ToInt64(VWR_CS.VWVariables.Where(x => (string)x.Item == "Recipe.CoatingStep.Schleudern.Rotor Drehzahl 2").ToArray()[0].Value);
                                   // double RPM_3 = Convert.ToInt64(VWR_A.VWVariables.Where(x => (string)x.Item == "Recipe.CoatingStep.Schleudern.Rotor Drehzahl 3").ToArray()[0].Value);

                                    if ( RPM_2 > RPM ) 
                                    {
                                        return -1;
                                    }
                                }
                            }
                        }
                    }
                }

                if (Data_2 != "") 
                {
                    DT = (new LocalDBAdapter("SELECT Id " +
                                        "FROM Orders " +
                                        "WHERE Data_2 ='" + Data_2 + "'; ")).DB_Output();
                    if (DT.Rows.Count == 0)
                    {
                        bool result = (new LocalDBAdapter("INSERT INTO Orders " +
                                                          "(TimeStamp, Data_1, Data_2, Data_3, MR_Id, User) " +
                                                          "VALUES ('" + GetDataTimeNowToFormat() + "','" + Data_1 + "','" + Data_2 + "','" + Data_3 + "'," + MR_Id + ",'" + ApplicationService.GetVariableValue("__CURRENT_USER.FULLNAME").ToString() + "')")).DB_Input();
                        DT = (new LocalDBAdapter("SELECT Id " +
                                                 "FROM Orders " +
                                                 "WHERE Data_2 ='" + Data_2 + "'; ")).DB_Output();
                    }
                    else 
                    {
                        bool result = (new LocalDBAdapter("Update Orders SET " +
                                      "TimeStamp = '" + GetDataTimeNowToFormat() + "', " +
                                      "Data_1 = '" + Data_1 + "', " +
                                      "Data_3 = '" + Data_3 + "', " +
                                      "MR_Id = " + MR_Id + "," +
                                      "User = '" + ApplicationService.GetVariableValue("__CURRENT_USER.FULLNAME").ToString() + "' " +
                                      "WHERE Id = " + DT.Rows[0]["Id"].ToString())).DB_Input();

                    }
                  

                    long Order_Id = (long)(DT.Rows[0]["Id"]);
                    long Old_Order_Id = (uint)ApplicationService.GetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Header.Order Id");
                    long Box_Id = 1;

                    if (Old_Order_Id == Order_Id)
                    {
                        Box_Id = 1 + (short)ApplicationService.GetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Handshake.from PC.BoxNumber");
                    }
                    ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Header.Order Id", Order_Id);
                    ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.LD.Handshake.from PC.BoxNumber", Box_Id);
                    return Order_Id;
                }

                
                return -2;
            }
           
            
            return -3;
        }
        #endregion

        #endregion
        private string GetDataTimeNowToFormat()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
