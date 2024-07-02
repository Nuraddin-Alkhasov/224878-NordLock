using System.Windows.Media;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using VisiWin.DataAccess;

namespace HMI.Handmenu
{
    /// <summary>
    /// Interaction logic for ButtonsView.xaml
    /// </summary>
    [ExportView("H_M3_KA_Tilt")]
 
    public partial class H_M3_KA_Tilt : View
    {
        IVariableService VS;
        IVariable Status;
        public H_M3_KA_Tilt()
        {
            this.InitializeComponent();
            VS = ApplicationService.GetService<IVariableService>();
            Status = VS.GetVariable("NL.PLC.Blocks.3 Modul 3.12 Korb Auskippen.01 Kippantrieb.DB Korb Kippantrieb HMI.Actual value.Kippantrieb Position Textliste");
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