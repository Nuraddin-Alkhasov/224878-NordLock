using HMI.Interfaces;
using System;
using System.ComponentModel.Composition;
using System.Timers;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;

namespace HMI.Services
{
    [ExportService(typeof(IPLCHandshacke))]
    [Export(typeof(IPLCHandshacke))]
    public class Service_PLC_Handshacke : ServiceBase, IPLCHandshacke
    {
        IVariableService VS;
        IVariable CPU1;
        IVariable CPU2;
        
        public Service_PLC_Handshacke()
        {
            if (ApplicationService.IsInDesignMode)
                return;
        }


       
        void CPU1_Change(object sender, VariableEventArgs e)
        {
            if (e.Quality.Data == DataQuality.Good)
            {
                CPU1.Value = false;
                VS.SetValue("IsAlive.CPU1", false);
            }
            else { VS.SetValue("IsAlive.CPU1", true); }

        }
      


        public bool IsAlive_CPU2 { get; set; }

        void CPU2_Change(object sender, VariableEventArgs e)
        {
            if (e.Quality.Data == DataQuality.Good)
            {
                CPU2.Value = false;
                VS.SetValue("IsAlive.CPU2", false);
            }
            else { VS.SetValue("IsAlive.CPU2", true); }

        }

        Timer CPU2Timer;

       
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
           
            CPU1 = VS.GetVariable("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.General.Toggle Bit");
            CPU1.Change += CPU1_Change;


          
            CPU2 = VS.GetVariable("NL.PLC.Blocks.50 HMI.01 PC.DB PC.General.Toggle Bit");
            CPU2.Change += CPU2_Change;
            

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

    }
}
