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
using System.Windows;
using System.Windows.Threading;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;
using VisiWin.Recipe;

namespace HMI.Services
{
    [ExportService(typeof(IForplanet))] 
    [Export(typeof(IForplanet))]
    public class Service_H_Forplanet : ServiceBase, IForplanet
    {
        IVariableService VS;
        IVariable CToPLC;

        BackgroundWorker loadC;

        public Service_H_Forplanet()
        {
            if (ApplicationService.IsInDesignMode)
                return;
        }

        #region - - - C Handshacke - - -

        void CToPLC_Change(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue && bool.Parse(e.Value.ToString()))
            {
                CToPLC.Value = false;
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
                C_To_PLC();
            }
            catch { ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.Forplanet.Handshake.from PC.Not loaded", true); }
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

            CToPLC = VS.GetVariable("NL.PLC.Blocks.50 HMI.01 PC.DB PC.Forplanet.Handshake.to PC.GetRecipe");
            CToPLC.Change += CToPLC_Change;

            loadC = new BackgroundWorker();
            loadC.DoWork += W2_DoWork;

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

        #region - - - Coating - - -

        void C_To_PLC()
        {

            short CoatingLayer = (short)ApplicationService.GetVariableValue("NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung PD.Status.Charge.Beschichtungen.Ist");
            short SetCoatingLayer = (short)ApplicationService.GetVariableValue("NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung PD.Status.Charge.Beschichtungen.Soll");

            if (SetCoatingLayer - CoatingLayer <= 0)
            {
                ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.Forplanet.Handshake.from PC.Not loaded", true);
                new MessageBoxTask("@RecipeSystem.Results.Text8", "@MessageBox.Text1", MessageBoxIcon.Error);
            }
            else 
            {
                long C_Id = (uint)ApplicationService.GetVariableValue("NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung PD.Header.MR.CoatingLayer " + (CoatingLayer + 1).ToString() + ".Recipe Id");

                CoatingRecipe C = GetCoatingData(C_Id);
                if (C.Id == -1)
                {
                    ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.Forplanet.Handshake.from PC.Not loaded", true);
                    new MessageBoxTask("@RecipeSystem.Results.Text7", "@MessageBox.Text1", MessageBoxIcon.Error);
                }
                else
                {
                    VS.SetValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.Forplanet.Gesammtzeit Programm", GetProgramTime(C));
                    LoadCoatingToProcessAsync(C);
                }
            }

          
           
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
                    for (int i = 0; i <= 7; i++)
                    {
                        temp.Add(GetCoatingStepData((long)DT.Rows[0]["S" + (i+1).ToString() + "_Id"]));
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
            return new CoatingRecipe() { Id = -1 };
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
            await T.SetDefaultValuesToBufferAsync();
            try
            {
                for (int i = 0; i <= 7; i++)
                {
                    for (int j = 0; j < C.CoatingSteps[i].VWR.VWVariables.Count; j++)
                    {
                        T.SetValue(C.CoatingSteps[i].VWR.VWVariables[j].ToString().Replace("Recipe.CoatingStep", "NL.PLC.Blocks.50 HMI.01 PC.DB PC.Forplanet.CoatingStep[" + i + "]"), C.CoatingSteps[i].VWR.VWVariables[j].Value);
                    }
                }
                WriteBufferToProcessResult result = await T.WriteBufferToProcessAsync();
                if (result.Result != SetRecipeResult.Succeeded)
                {
                    ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.Forplanet.Handshake.from PC.Not loaded", true);
                    new MessageBoxTask("@RecipeSystem.Results.LoadError", "@MessageBox.Text1", MessageBoxIcon.Error);
                }
                ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.Forplanet.Handshake.from PC.Loaded", true);
                await Dispatcher.BeginInvoke((Action)(() =>
                {
                    ApplicationService.SetView("TouchpadRegion", "EmptyView");
                }));
            }
            catch { ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.Forplanet.Handshake.from PC.Not loaded", true); }
            
            
            

        }

        int GetProgramTime(CoatingRecipe _c) 
        {
            int retValue = 0;
            foreach (CoatingStepRecipe csr in _c.CoatingSteps) 
            {
                if (csr.Id != -1) 
                {
                    foreach (VWVariable vwv in csr.VWR.VWVariables)
                    {
                        if (vwv.Item.ToString().Contains("Drehzeit") || vwv.Item.ToString().Contains("Tauchzeit") || vwv.Item.ToString().Contains("Wälzzeit")) 
                        {
                            retValue += Convert.ToInt32(vwv.Value);
                        }
                    }   
                }
            }
            return retValue;
        }
        #endregion


        #endregion

    }
}
