using HMI.Interfaces;
using HMI.Module;
using HMI.Views.MainRegion.MachineOverview;
using HMI.Views.MessageBoxRegion;
using HMI.Views.TouchpadRegion;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;
using VisiWin.Recipe;

namespace HMI.Services
{
    [ExportService(typeof(ITrayBuffer))] 
    [Export(typeof(ITrayBuffer))]
    public class Service_H_TrayBuffer : ServiceBase, ITrayBuffer
    {
        IVariableService VS;
        IVariable DataToSQL;
        IVariable DataToPLC;
        IRegionService iRS = ApplicationService.GetService<IRegionService>();

        BackgroundWorker SetData;
        BackgroundWorker GetData;

        public Service_H_TrayBuffer()
        {
            if (ApplicationService.IsInDesignMode)
                return;
        }

        #region - - - MR Handshacke - - -

        void DataToSQL_Change(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue && bool.Parse(e.Value.ToString()))
            {
                DataToSQL.Value = false;
                SetData.RunWorkerAsync();
            }

        }
        void W1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Application.Current.Dispatcher.InvokeAsync((Action)delegate
            {
                ApplicationService.SetView("TouchpadRegion", "WaitScreen", new WaitData(1, "@WaitScreen.Text4"));
            });

            try
            {
                Data_To_SQLAsync();
            }
            catch { ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Handshake.from PC.Error", true); }
        }


        void DataToPLC_Change(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue && bool.Parse(e.Value.ToString()))
            {
                DataToPLC.Value = false;
                GetData.RunWorkerAsync();
            }

        }

        void W2_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Application.Current.Dispatcher.InvokeAsync((Action)delegate
            {
                ApplicationService.SetView("TouchpadRegion", "WaitScreen", new WaitData(1, "@WaitScreen.Text4"));
            });

            try
            {
                Data_To_PLCAsync();
            }
            catch { ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Handshake.from PC.Error", true); }
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
            DataToSQL = VS.GetVariable("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Handshake.to PC.SetData");
            DataToSQL.Change += DataToSQL_Change;

            SetData = new BackgroundWorker();
            SetData.DoWork += W1_DoWork;

            DataToPLC = VS.GetVariable("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Handshake.to PC.GetData");
            DataToPLC.Change += DataToPLC_Change;

            GetData = new BackgroundWorker();
            GetData.DoWork += W2_DoWork;

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

        async void Data_To_SQLAsync()
        {

            uint Order_Id = (uint)ApplicationService.GetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Header.Order Id");
            short Run = (short)ApplicationService.GetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Charge.Aktueller Durchlauf");
            string Coord = (string)ApplicationService.GetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Handshake.to PC.Coord#STRING254");
            string Status = await GetStatusFromPLCToStringAsync();
            
            int T_Order_Id = -1;
            if (Order_Id != 0)
            {
                T_Order_Id = Convert.ToInt32(Order_Id);
            }

            if (Status != "")
            {
                if ((new LocalDBAdapter("UPDATE Tray_Buffer " +
                                         "SET Order_Id =" + T_Order_Id + ", Status='" + Status + "' " +
                                         "WHERE Coord = '" + Coord + "';")).DB_Input())
                {
                    short Shelve = (short)ApplicationService.GetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Handshake.to PC.X");
                    short Level = (short)ApplicationService.GetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Handshake.to PC.Y");

                    await Dispatcher.InvokeAsync(delegate
                    {
                        MO_TrayBuffer MO_TB = (MO_TrayBuffer)iRS.GetView("MO_TrayBuffer");
                        MO_TB.GetTBTray(Coord).UpdateTrayStatus();
                    });

                    ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Handshake.from PC.SetData Executed", true);

                }
                else { ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Handshake.from PC.Error", true); }

            }
            else
            {
                ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Handshake.from PC.Error", true);
                new MessageBoxTask("@RecipeSystem.Results.LoadError", "@MessageBox.Text1", MessageBoxIcon.Error); ///neeed text
            }
            await Dispatcher.BeginInvoke((Action)(() =>
            {
                ApplicationService.SetView("TouchpadRegion", "EmptyView");
            }));

           
        }

        async Task<string> GetStatusFromPLCToStringAsync() 
        {
            IRecipeClass TBStatus = ApplicationService.GetService<IRecipeService>().GetRecipeClass("TBStatus");
            var result1 = await TBStatus.ReadProcessToBufferAsync();

            if (result1.Result == GetRecipeResult.Succeeded)
            {
                var result2 = await TBStatus.SaveToFileFromBufferAsync("Status", "", true);

                if (result2.Result == SaveRecipeResult.Succeeded)
                {
                    return File.ReadAllText(TBStatus.CurrentPath + @"\\Status.R");
                }
            }
            return "";           
        }

        async void Data_To_PLCAsync()
        {
            // Phase 1 PLC does itself the data tracking management SQL has only to delete data on request, to keep the data sync.  
            string Coord = (string)ApplicationService.GetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Handshake.to PC.Coord#STRING254");
            string Status = await SetDefaultValuesToStringAsync();

            if (Status != "")
            {
                if ((new LocalDBAdapter("UPDATE Tray_Buffer " +
                                         "SET Order_Id = -1, Status='" + Status + "' " +
                                         "WHERE Coord = '" + Coord + "';")).DB_Input())
                {
           

                    short Shelve = (short)ApplicationService.GetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Handshake.to PC.X");
                    short Level = (short)ApplicationService.GetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Handshake.to PC.Y");

                    await Dispatcher.InvokeAsync(delegate
                    {
                        MO_TrayBuffer MO_TB = (MO_TrayBuffer)iRS.GetView("MO_TrayBuffer");
                        MO_TB.GetTBTray(Coord).UpdateTrayStatus();
                    });

                    ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Handshake.from PC.GetData Executed", true);
                }
                else { ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Handshake.from PC.Error", true); }

            }
            else
            {
                ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Handshake.from PC.Error", true);
                new MessageBoxTask("@RecipeSystem.Results.LoadError", "@MessageBox.Text1", MessageBoxIcon.Error); ///neeed text
            }
            await Dispatcher.BeginInvoke((Action)(() =>
            {
                ApplicationService.SetView("TouchpadRegion", "EmptyView");
            }));
        }

        async Task<string> SetDefaultValuesToStringAsync()
        {
            IRecipeClass TBStatus = ApplicationService.GetService<IRecipeService>().GetRecipeClass("TBStatus");
            var result1 = await TBStatus.SetDefaultValuesToBufferAsync();
            
            if (result1.Result == LoadRecipeResult.Succeeded)
            {
                var result2 = await TBStatus.SaveToFileFromBufferAsync("Status", "", true);
                
                if (result2.Result == SaveRecipeResult.Succeeded)
                {
                    await TBStatus.WriteBufferToProcessAsync();
                    ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Header.Order Id", 0);
                    return File.ReadAllText(TBStatus.CurrentPath + @"\\Status.R");
                }
            }
            return "";
        }


        #endregion



        #endregion

    }
}
