using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Handmenu
{
    /// <summary>
    /// Interaction logic for ButtonsView.xaml
    /// </summary>
    [ExportView("H_M4_Mani_Lift")]
 
    public partial class H_M4_Mani_Lift : View
    {
        public H_M4_Mani_Lift()
        {
            this.InitializeComponent();
        }
        private void Key_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ApplicationService.SetVariableValue("NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.02 Mani Hub.DB Mani04 Hub HMI.Set Value.Vorwahl Etage Tablettpuffer", ((VisiWin.Controls.RadioButton)sender).Tag);
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (Picker.SelectedIndex != 11 && Picker.SelectedIndex != 12)
            {
                Buffer.Visibility = System.Windows.Visibility.Hidden;
                ApplicationService.SetVariableValue("NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.02 Mani Hub.DB Mani04 Hub HMI.Set Value.Vorwahl Etage Tablettpuffer", 0);
            }
            else
            {
                btn1.IsChecked = true;
                Buffer.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Buffer_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                short Drive = (short)ApplicationService.GetVariableValue("NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.01 Mani Fahr.DB Mani04 Fahr HMI.Actual value.Position Textliste");
                short Rotary = (short)ApplicationService.GetVariableValue("NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.03 Mani Drehen.DB Mani04 Drehen HMI.Actual value.Position Textliste");
                if (Drive == 8 && Rotary == 8) 
                {
                    bool B = (bool)ApplicationService.GetVariableValue("NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.00 Allgemein.DB Mani04 PD.Status.Tablett.Belegt");
                    if (B)
                    {
                        ApplicationService.SetVariableValue("NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.02 Mani Hub.DB Mani04 Hub HMI.Set Value.Vorwahl Position Textliste",12);

                    }
                    else
                    {
                        ApplicationService.SetVariableValue("NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.02 Mani Hub.DB Mani04 Hub HMI.Set Value.Vorwahl Position Textliste", 13);
                    }
                }
                
            }
           
            
        }
    }
}