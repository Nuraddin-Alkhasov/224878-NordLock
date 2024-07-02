using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Handmenu
{
    /// <summary>
    /// Interaction logic for ButtonsView.xaml
    /// </summary>
    [ExportView("H_M4_Mani_Arm")]
 
    public partial class H_M4_Mani_Arm : View
    {
        public H_M4_Mani_Arm()
        {
            this.InitializeComponent();
        }

        private void aaa_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.IsVisible)
            {
                short Drive = (short)ApplicationService.GetVariableValue("NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.01 Mani Fahr.DB Mani04 Fahr HMI.Actual value.Position Textliste");
                short Rotary = (short)ApplicationService.GetVariableValue("NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.03 Mani Drehen.DB Mani04 Drehen HMI.Actual value.Position Textliste");
                short Lift = (short)ApplicationService.GetVariableValue("NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.02 Mani Hub.DB Mani04 Hub HMI.Actual value.Position Textliste");

                if (Drive == 8 && Rotary == 8 && (Lift == 13 || Lift == 14))
                {
                    ApplicationService.SetVariableValue("NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.04 Mani Arm.DB Mani04 Arm HMI.Set Value.Vorwahl Position Textliste", 7);
                }

            }

        }
        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (picker.SelectedIndex != 6)
            {
                Buffer.Visibility = System.Windows.Visibility.Hidden;
                ApplicationService.SetVariableValue("NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.01 Mani Fahr.DB Mani04 Fahr HMI.Set Value.Vorwahl Gasse Tablettpuffer", 0);
            }
            else
            {
                btn1.IsChecked = true;
                Buffer.Visibility = System.Windows.Visibility.Visible;
            }
        }
        private void Buffer_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (Buffer.IsVisible)
            {
                short B = (short)ApplicationService.GetVariableValue("NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.01 Mani Fahr.DB Mani04 Fahr HMI.Set Value.Vorwahl Gasse Tablettpuffer");
                switch (B)
                {
                    case 0: btn1.IsChecked = true; break;
                    case 1: btn2.IsChecked = true; break;
                    case 2: btn3.IsChecked = true; break;
                    case 3: btn4.IsChecked = true; break;
                    case 4: btn5.IsChecked = true; break;
                    case 5: btn6.IsChecked = true; break;
                    case 6: btn7.IsChecked = true; break;
                    case 7: btn8.IsChecked = true; break;
                    case 8: btn9.IsChecked = true; break;
                    case 9: btn10.IsChecked = true; break;
                    case 10: btn11.IsChecked = true; break;
                    case 11: btn12.IsChecked = true; break;
                }
            }
        }
        private void Key_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ApplicationService.SetVariableValue("NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.01 Mani Fahr.DB Mani04 Fahr HMI.Set Value.Vorwahl Gasse Tablettpuffer", ((VisiWin.Controls.RadioButton)sender).Tag);
        }
    }
}