using HMI.Views.DialogRegion;
using HMI.Views.MessageBoxRegion;
using System;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;
using VisiWin.Helper;
using VisiWin.Language;
using VisiWin.Logging;


namespace HMI.Views.MainRegion.MachineOverview
{

    [ExportView("MO_MainView")]
    public partial class MO_MainView : VisiWin.Controls.View
    {
        IVariableService VS;
        private readonly IVariable status;


        private readonly ILoggingService loggingService;
        ILanguageService textService;
        public MO_MainView()
        {
            InitializeComponent();
            VS = ApplicationService.GetService<IVariableService>();
            status = VS.GetVariable("NL.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Steuerspannung Status");
            status.Change += Status_Change;
            loggingService = ApplicationService.GetService<ILoggingService>();
            textService = ApplicationService.GetService<ILanguageService>();

        }

        private void Status_Change(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue)
            {
                if ((short)e.Value == 2 || (short)e.Value == 3)
                {
                    ONOFF.VariableName = "NL.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Steuerspannung Aus";
                    ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Steuerspannung Ein", 0);
                    string txt = textService.GetText("@Logging.Service.Text18");
                    this.loggingService.Log("Service", "Anlage Ein/Aus", txt, FastDateTime.Now);
                }
                else
                {
                    ONOFF.VariableName = "NL.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Steuerspannung Ein";
                    ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Steuerspannung Aus", 0);
                    string txt = textService.GetText("@Logging.Service.Text19");
                    this.loggingService.Log("Service", "Anlage Ein/Aus", txt, FastDateTime.Now);
                }
                if ((short)e.Value == 3)
                {
                    powerOFF.Visibility = Visibility.Visible;
                }
                else
                {
                    powerOFF.Visibility = Visibility.Hidden;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        
        }

     

        private void Reg_Loaded(object sender, RoutedEventArgs e)
        {
            if (Reg.Content == null)
            {
                Task obTask = Task.Run(() =>
                {
                    Application.Current.Dispatcher.InvokeAsync((Action)delegate
                    {
                        Reg.Content = new MO_MaschineView();
                    });
                });
            }
        }

    }
  
}



