using HMI.Interfaces;
using HMI.Module;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;

namespace HMI.Services
{
    [ExportService(typeof(IQS2))] 
    [Export(typeof(IQS2))]
    public class Service_H_QS2 : ServiceBase, IQS2
    {
        IVariableService VS;
        IVariable NLDataToPLC;

        BackgroundWorker loadNLData;

        public Service_H_QS2()
        {
            if (ApplicationService.IsInDesignMode)
                return;
        }

        #region - - - C Handshacke - - -

        void NLDataToPLC_Change(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue && bool.Parse(e.Value.ToString()))
            {
                NLDataToPLC.Value = false;
                loadNLData.RunWorkerAsync();
            }

        }
        void W1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                uint OrderId = (uint)ApplicationService.GetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.QS 2.Handshake.to PC.Order Id");
                
                DataTable DT = (new LocalDBAdapter("SELECT * " +
                                                    "FROM Orders " +
                                                    "WHERE Id = " + OrderId + "; ")).DB_Output();

                if (DT.Rows.Count > 0)
                {
                    ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.QS 2.Handshake.Data.Quality Order#STRING12", DT.Rows[0]["Data_1"]);
                    ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.QS 2.Handshake.Data.Quality Batch#STRING12", DT.Rows[0]["Data_2"]);
                    ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.QS 2.Handshake.Data.Quality Item#STRING12", DT.Rows[0]["Data_3"]);

                    ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.QS 2.Handshake.from PC.Loaded", true);
                    return;
                }
                else 
                {
                     ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.QS 2.Handshake.from PC.Not loaded", true); 
                }
            }
            catch 
            {
                 ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.QS 2.Handshake.from PC.Not loaded", true); 
            }
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

            NLDataToPLC = VS.GetVariable("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.QS 2.Handshake.to PC.GetData");
            NLDataToPLC.Change += NLDataToPLC_Change;

           loadNLData = new BackgroundWorker();
           loadNLData.DoWork += W1_DoWork;

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


        #endregion

    }
}
