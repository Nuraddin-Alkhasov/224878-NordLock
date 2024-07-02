using HMI.UserControls;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Parameter
{
    /// <summary>
    /// Interaction logic for DigitalIOView.xaml
    /// </summary>
    [ExportView("P_M2_Ventilators")]
    public partial class P_M2_Ventilators 
    {
		
        public P_M2_Ventilators()
        {
            this.InitializeComponent();
            
        }

        private void P_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            Reg.Content = new VentilatorStatus()
            {
                LocalizableHeaderText = "@Parameter.Text46",
                VentilatorOnVariable = "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.PC.Abluft.Ein",
                VentilatorOffVariable = "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.PC.Abluft.Aus",
                VentilatorStatusVariable = "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.Status.Abluft.Ventilator",
                NewStartVariableMin = "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.Status.Abluft.Zeit bis Wiederanlauf.Minute",
                NewStartVariableSec = "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.Status.Abluft.Zeit bis Wiederanlauf.Second",
                PurgeVariableMin = "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.Status.Abluft.Spühlzeit.Minute",
                PurgeVariableSec = "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.Status.Abluft.Spühlzeit.Second",
                PurgeStatusVariable = "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.Status.Abluft.Spühlzeit aktiv",
                PS1StatusVariable = "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.Status.Abluft.Druckschalter 1",
                PS2StatusVariable = "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.Status.Abluft.Druckschalter 2"
            };
        }

        private void B_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            Reg.Content = Reg.Content = new VentilatorStatus()
            {
                LocalizableHeaderText = "@Parameter.Text47",
                VentilatorOnVariable = "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.PC.Zuluft.Ein",
                VentilatorOffVariable = "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.PC.Zuluft.Aus",
                VentilatorStatusVariable = "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.Status.Zuluft.Ventilator",
                NewStartVariableMin = "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.Status.Zuluft.Zeit bis Wiederanlauf.Minute",
                NewStartVariableSec = "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.Status.Zuluft.Zeit bis Wiederanlauf.Second",
                PurgeVariableMin = "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.Status.Zuluft.Spühlzeit.Minute",
                PurgeVariableSec = "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.Status.Zuluft.Spühlzeit.Second",
                PurgeStatusVariable = "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.Status.Zuluft.Spühlzeit aktiv",
                PS1StatusVariable = "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.Status.Zuluft.Druckschalter 1",
                PS2StatusVariable = "NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.Status.Zuluft.Druckschalter 2"
            };
        }

        

        private void P_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Task obTask = Task.Run(() =>
            {
                Application.Current.Dispatcher.InvokeAsync((Action)delegate
                {
                    btn1.IsChecked = true;
                });
            });
        }
        private void Reg_Unloaded(object sender, RoutedEventArgs e)
        {
            btn1.IsChecked = false;
            Reg.Content = null;
        }
    }
}