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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;
using VisiWin.Recipe;

namespace HMI.Services
{
    [ExportService(typeof(IPDOverwrite))] 
    [Export(typeof(IPDOverwrite))]
    public class Service_H_PDOverwrite : ServiceBase, IPDOverwrite
    {
        //IVariableService VS;
      //  IVariable MRToPLC;
       // IVariable CToPLC;

        BackgroundWorker loadMR;
        BackgroundWorker loadC;

        public Service_H_PDOverwrite()
        {
            if (ApplicationService.IsInDesignMode)
                return;
        }

        #region - - - MR Handshacke - - -

        void MRToPLC_Change(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue && bool.Parse(e.Value.ToString()))
            {
          //      MRToPLC.Value = false;
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
                LoadFromDB_MR_TOProcess();
            }
            catch { ApplicationService.SetVariableValue("NL.PLC.Blocks.1 Modul 1.01 Lifting Tilting Device.01 Main.DB LD HMI.PC.Handshake.from PC.Recipe not loaded", true); }
        }

        #endregion

        #region - - - C Handshacke - - -

        void CToPLC_Change(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue && bool.Parse(e.Value.ToString()))
            {
                //CToPLC.Value = false;
                loadC.RunWorkerAsync();
            }

        }
        void W2_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Application.Current.Dispatcher.InvokeAsync((Action)delegate
            {
                ApplicationService.SetView("TouchpadRegion", "WaitScreen", new WaitData(1, "@WaitScreen.Text3"));
            });

            try
            {
                LoadFromDB_C_TOProcessAsync();
            }
            catch { ApplicationService.SetVariableValue("NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.PC.Handshake.from PC.Recipe not loaded", true); }
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
            //VS = ApplicationService.GetService<IVariableService>();
            //MRToPLC = VS.GetVariable("NL.PLC.Blocks.1 Modul 1.01 Lifting Tilting Device.01 Main.DB LD HMI.PC.Handshake.to PC.MR request");
            //MRToPLC.Change += MRToPLC_Change;

            //CToPLC = VS.GetVariable("NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.PC.Handshake.to PC.MR request");
            //CToPLC.Change += CToPLC_Change;

            loadMR = new BackgroundWorker();
            //loadMR.DoWork += W1_DoWork;

            loadC = new BackgroundWorker();
            //loadC.DoWork += W2_DoWork;

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

        #region - - - MR - - - 

        void LoadFromDB_MR_TOProcess()
        {
            object temp = ApplicationService.GetVariableValue("NL.PLC.Blocks.1 Modul 1.01 Lifting Tilting Device.01 Main.DB LD PD Buffer.Header.Order Id");
            long O_Id = (long)Convert.ToUInt64(temp);

            MachineRecipe MR = GetMachineRecipe(O_Id);
            SetHeader(MR);
            LoadArticleToProcess(MR);
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
                        Type_Id = 0,
                        Type = "Article",
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
                else { CR.Add(new CoatingRecipe() { Id = 0 }); }
            }
            return CR;
        }
        void SetHeader(MachineRecipe MR)
        {
            ApplicationService.SetVariableValue("NL.PLC.Blocks.1 Modul 1.01 Lifting Tilting Device.01 Main.DB LD PD Buffer.Header.MR.Article Id", MR.Article.Id);
            for (int i = 1; i <= 10; i++)
            {
                ApplicationService.SetVariableValue("NL.PLC.Blocks.1 Modul 1.01 Lifting Tilting Device.01 Main.DB LD PD Buffer.Header.MR.CoatingLayer " + i + ".Paint Id", MR.CoatingLayers[i - 1].Paint_Id);
                ApplicationService.SetVariableValue("NL.PLC.Blocks.1 Modul 1.01 Lifting Tilting Device.01 Main.DB LD PD Buffer.Header.MR.CoatingLayer " + i + ".Recipe Id", MR.CoatingLayers[i - 1].Id);
            }
        }
        void LoadArticleToProcess(MachineRecipe MR)
        {
            Task obTask = Task.Run(() =>
            {
                System.IO.File.WriteAllText(MR.Article.Class.CurrentPath + @"\\" + MR.Article.Name + ".R", MR.Article.Data);
            });

            obTask.ContinueWith(async (xs) =>
            {
                var result = await MR.Article.Class.LoadFromFileToProcessAsync(MR.Article.Name);

                if (result.Result != SendRecipeResult.Succeeded)
                {
                    ApplicationService.SetVariableValue("NL.PLC.Blocks.1 Modul 1.01 Lifting Tilting Device.01 Main.DB LD HMI.PC.Handshake.from PC.Recipe not loaded", true);
                    new MessageBoxTask("@RecipeSystem.Results.LoadError", "@MessageBox.Text1", MessageBoxIcon.Error);
                }
                ApplicationService.SetVariableValue("NL.PLC.Blocks.1 Modul 1.01 Lifting Tilting Device.01 Main.DB LD HMI.PC.Handshake.from PC.Recipe loaded", true);
                await Dispatcher.BeginInvoke((Action)(() =>
                {
                    ApplicationService.SetView("TouchpadRegion", "EmptyView");
                }));
            },
            TaskContinuationOptions.OnlyOnRanToCompletion
            );
        }

        #endregion

        #region - - - C - - -

        void LoadFromDB_C_TOProcessAsync()
        {
            object obj = ApplicationService.GetVariableValue("NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung PD.Status.Charge.Beschichtungen.Ist");
            int CoatingLayer = Convert.ToInt32(obj);
            obj = ApplicationService.GetVariableValue("NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung PD.Header.MR.CoatingLayer " + (CoatingLayer + 1).ToString() + ".Recipe Id");
            long C_Id = Convert.ToInt32(obj);
            CoatingRecipe C = GetCoatingData(C_Id);

            LoadCoatingToProcessAsync(C);
        }

        CoatingRecipe GetCoatingData(long _id)
        {
            if (_id != -1) 
            {
                DataTable DT = (new LocalDBAdapter("SELECT * " +
                                                  "FROM Recipes_Coating " +
                                                  "WHERE Id = " + _id + "; ")).DB_Output();

                if (DT.Rows.Count > 0)
                {
                    ObservableCollection<CoatingStepRecipe> temp = new ObservableCollection<CoatingStepRecipe>();
                    for (int i = 1; i <= 8; i++)
                    {
                        temp.Add(GetCoatingStepData((long)DT.Rows[0]["S" + i + "_Id"]));
                    }

                    return new CoatingRecipe()
                    {
                        Id = (long)DT.Rows[0]["Id"],
                        Name = (string)DT.Rows[0]["Name"],
                        Description = (string)DT.Rows[0]["Description"],
                        Paint_Id = (long)DT.Rows[0]["Paint_Id"],
                        CoatingSteps = temp,
                        Version = (string)DT.Rows[0]["Version"],
                        LastChanged = (DateTime)DT.Rows[0]["LastChanged"],
                        User = (string)DT.Rows[0]["User"]
                    };

                }
            }          
            return new CoatingRecipe() { Id = 0 };
        }

        CoatingStepRecipe GetCoatingStepData(long _id) 
        {
            DataTable DT = (new LocalDBAdapter("SELECT Id, Name, Type_Id, Data " +
                                                   "FROM Recipes_CoatingStep_VW " +
                                                   "WHERE Id = " + _id + "; ")).DB_Output();
            if (DT.Rows.Count > 0)
            {
                var x=  new CoatingStepRecipe() 
                {
                    Id = (long)DT.Rows[0]["Id"],
                    Name = (string)DT.Rows[0]["Name"],
                    Type_Id = (long)DT.Rows[0]["Type_Id"],
                    Data = (string)DT.Rows[0]["Data"]
                };
                return x;
            }
            else 
            {
                return new CoatingStepRecipe();
            }

        }

        async void LoadCoatingToProcessAsync(CoatingRecipe C)
        {
            IRecipeClass T = ApplicationService.GetService<IRecipeService>().GetRecipeClass("Coating");
            for (int i = 1; i <= 8; i++) 
            {
                for (int j = 0; j < C.CoatingSteps[i - 1].VWR.VWVariables.Count; j++) 
                {
                    T.SetValue(C.CoatingSteps[i - 1].VWR.VWVariables[j].ToString().Replace("Recipe.CoatingStep", "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung PD Buffer.CoatingStep[" + i + "]"), C.CoatingSteps[i - 1].VWR.VWVariables[j].Value);
                }
            }

            WriteBufferToProcessResult result = await T.WriteBufferToProcessAsync();
            if (result.Result != SetRecipeResult.Succeeded)
            {
                ApplicationService.SetVariableValue("NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.PC.Handshake.from PC.Recipe not loaded", true);
                new MessageBoxTask("@RecipeSystem.Results.LoadError", "@MessageBox.Text1", MessageBoxIcon.Error);
            }
            ApplicationService.SetVariableValue("NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.PC.Handshake.from PC.Recipe loaded", true);
            await Dispatcher.BeginInvoke((Action)(() =>
            {
                ApplicationService.SetView("TouchpadRegion", "EmptyView");
            }));

        }

        #endregion


        #endregion

    }
}
