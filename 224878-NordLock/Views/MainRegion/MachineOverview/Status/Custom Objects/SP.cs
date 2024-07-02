using System;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;

namespace HMI.Views.MainRegion.MachineOverview
{
    public class SP
    {

        public SP()
        {
            Initialize();
        }
        public int Module { set; get; }
        public int M1_Station { set; get; }
        public int M2_Station { set; get; }
        public int M3_Station { set; get; }
        public int M4_Station { set; get; }
        public int OvenTray { set; get; }
        public int CZTray { set; get; }
        public int TB_Shelve { set; get; }
        public int TB_Level { set; get; }
        public string Header { set; get; }
        public string Type { set; get; }
        public void SetValues()
        {
            switch (Module) 
            {
                case 1-3:
                    ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.Module", Module);
                    ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.Station.1", M1_Station);
                    ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.Station.2", M2_Station);
                    ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.Station.3", M3_Station); break;
                case 4:
                    ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.Module", Module);
                    ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.Station.4", M4_Station);
                    ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.Oven.Place", OvenTray);
                    ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.CoolingZone.Place", CZTray);
                    ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.TrayBuffer.Shelve", TB_Shelve+1);
                    ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.TrayBuffer.Level", TB_Level+1); break;
                default:
                    ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.Module", Module);
                    ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.Station.1", M1_Station);
                    ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.Station.2", M2_Station);
                    ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.Station.3", M3_Station);
                    ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.Module", Module);
                    ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.Station.4", M4_Station);
                    ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.Oven.Place", OvenTray);
                    ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.CoolingZone.Place", CZTray);
                    ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.TrayBuffer.Shelve", TB_Shelve);
                    ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.TrayBuffer.Level", TB_Level); break;
            }
        }
        public void Execute()
        {
            SetValues();
            if (Type == "TrayTB")
            {
                Task obTask = Task.Run(async () =>
                {
                    await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                    {
                        ApplicationService.SetView("DialogRegion", "MOM4_Status_TB", TB_Shelve + "*" + TB_Level);
                    });
                });
              
            }
            else
            {
                Task obTask;
                switch (Module) 
                {
                    case 4:
                        obTask = Task.Run(async () =>
                        {
                            await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                            {
                                ApplicationService.SetView("DialogRegion", "MOM4_Status", Type + "*" + Header);
                            });
                        });
                        break;
                    default:
                        obTask = Task.Run(async () =>
                        {
                            await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                            {
                                ApplicationService.SetView("DialogRegion", "MO_Status", Type + "*" + Header);
                            });
                        });
                        break;
                }
            }
        }
        public void Initialize() 
        {
            Module = 0;
            M1_Station = 0;
            M2_Station = 0;
            M3_Station = 0;
            M4_Station = 0;
            OvenTray = 0;
            CZTray = 0;
            TB_Shelve = 0;
            TB_Level = 0;
        }
        public void Reset()
        {
            Initialize();
            SetValues();
        }
    }
}
