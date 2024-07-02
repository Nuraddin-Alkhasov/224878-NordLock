using System.Windows.Media;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using VisiWin.DataAccess;

namespace HMI.Handmenu
{
    /// <summary>
    /// Interaction logic for ButtonsView.xaml
    /// </summary>
    [ExportView("H_M4_Mani_Tilt")]
 
    public partial class H_M4_Mani_Tilt : View
    {
        IVariableService VS;
        IVariable Status;
        public H_M4_Mani_Tilt()
        {
            this.InitializeComponent();
            VS = ApplicationService.GetService<IVariableService>();
            Status = VS.GetVariable("NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.05 Mani Kippantrieb.DB Mani04 Palette Kippantrieb HMI.Actual value.Kippantrieb Position Textliste");
            Status.Change += Status_Change;
        }
        private void Status_Change(object sender, VariableEventArgs e)
        {
            if ((short)e.Value != 4)
            {
                Home.IsBlinkEnabled = true;
                Home.Background = (LinearGradientBrush)FindResource("FP_Gray_Gradient");
            }
            else
            {
                Home.IsBlinkEnabled = false;
                Home.Background = (LinearGradientBrush)FindResource("FP_Green_Gradient");
            }


        }
    }
}