using HMI.UserControls;
using System;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
namespace HMI.Parameter
{
    /// <summary>
    /// Interaction logic for DigitalIOView.xaml
    /// </summary>
    [ExportView("P_M4_Ventilators")]
    public partial class P_M4_Ventilators : View
    {
		
        public P_M4_Ventilators()
        {
            this.InitializeComponent();
            
        }

        private void B1_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            Reg.Content = new VentilatorStatus()
            {
                LocalizableHeaderText = "@Parameter.Text125",
                VentilatorOnVariable = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.01 Vorzone.DB Vorzone HMI.PC.Abluft.Ein",
                VentilatorOffVariable = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.01 Vorzone.DB Vorzone HMI.PC.Abluft.Aus",
                VentilatorStatusVariable = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.01 Vorzone.DB Vorzone HMI.Istwerte.Lüfter Abluft.Ventilator",
                NewStartVariableMin = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.01 Vorzone.DB Vorzone HMI.Istwerte.Lüfter Abluft.Zeit bis Wiederanlauf.Minute",
                NewStartVariableSec = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.01 Vorzone.DB Vorzone HMI.Istwerte.Lüfter Abluft.Zeit bis Wiederanlauf.Second",
                PurgeVariableMin = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.01 Vorzone.DB Vorzone HMI.Istwerte.Lüfter Abluft.Spühlzeit.Minute",
                PurgeVariableSec = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.01 Vorzone.DB Vorzone HMI.Istwerte.Lüfter Abluft.Spühlzeit.Second",
                PurgeStatusVariable = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.01 Vorzone.DB Vorzone HMI.Istwerte.Lüfter Abluft.Spühlzeit aktiv",
                PS1StatusVariable = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.01 Vorzone.DB Vorzone HMI.Istwerte.Lüfter Abluft.Druckschalter 1",
                PS2StatusVariable = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.01 Vorzone.DB Vorzone HMI.Istwerte.Lüfter Abluft.Druckschalter 2"
            };
        }

        private void B2_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            Reg.Content = Reg.Content = new VentilatorStatus()
            {
                LocalizableHeaderText = "@Parameter.Text126",
                VentilatorOnVariable = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.02 Zwischenzone.DB Zwischenzone HMI.PC.Abluft.Ein",
                VentilatorOffVariable = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.02 Zwischenzone.DB Zwischenzone HMI.PC.Abluft.Aus",
                VentilatorStatusVariable = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.02 Zwischenzone.DB Zwischenzone HMI.Istwerte.Lüfter Abluft.Ventilator",
                NewStartVariableMin = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.02 Zwischenzone.DB Zwischenzone HMI.Istwerte.Lüfter Abluft.Zeit bis Wiederanlauf.Minute",
                NewStartVariableSec = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.02 Zwischenzone.DB Zwischenzone HMI.Istwerte.Lüfter Abluft.Zeit bis Wiederanlauf.Second",
                PurgeVariableMin = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.02 Zwischenzone.DB Zwischenzone HMI.Istwerte.Lüfter Abluft.Spühlzeit.Minute",
                PurgeVariableSec = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.02 Zwischenzone.DB Zwischenzone HMI.Istwerte.Lüfter Abluft.Spühlzeit.Second",
                PurgeStatusVariable = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.02 Zwischenzone.DB Zwischenzone HMI.Istwerte.Lüfter Abluft.Spühlzeit aktiv",
                PS1StatusVariable = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.02 Zwischenzone.DB Zwischenzone HMI.Istwerte.Lüfter Abluft.Druckschalter 1",
                PS2StatusVariable = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.02 Zwischenzone.DB Zwischenzone HMI.Istwerte.Lüfter Abluft.Druckschalter 2"
            };
        }
        private void B3_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            Reg.Content = Reg.Content = new VentilatorStatus()
            {
                LocalizableHeaderText = "@Parameter.Text127",
                VentilatorOnVariable = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.03 Trockner.DB Trockner HMI.PC.Abluft.Ein",
                VentilatorOffVariable = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.03 Trockner.DB Trockner HMI.PC.Abluft.Aus",
                VentilatorStatusVariable = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.03 Trockner.DB Trockner HMI.Istwerte.Lüfter_Abluft.Ventilator",
                NewStartVariableMin = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.03 Trockner.DB Trockner HMI.Istwerte.Lüfter_Abluft.Zeit bis Wiederanlauf.Minute",
                NewStartVariableSec = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.03 Trockner.DB Trockner HMI.Istwerte.Lüfter_Abluft.Zeit bis Wiederanlauf.Second",
                PurgeVariableMin = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.03 Trockner.DB Trockner HMI.Istwerte.Lüfter_Abluft.Spühlzeit.Minute",
                PurgeVariableSec = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.03 Trockner.DB Trockner HMI.Istwerte.Lüfter_Abluft.Spühlzeit.Second",
                PurgeStatusVariable = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.03 Trockner.DB Trockner HMI.Istwerte.Lüfter_Abluft.Spühlzeit aktiv",
                PS1StatusVariable = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.03 Trockner.DB Trockner HMI.Istwerte.Lüfter_Abluft.Druckschalter 1",
                PS2StatusVariable = "NLM4.PLC.Blocks.4 Modul 4.08 Heizung / Ventilatoren.03 Trockner.DB Trockner HMI.Istwerte.Lüfter_Abluft.Druckschalter 2"
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