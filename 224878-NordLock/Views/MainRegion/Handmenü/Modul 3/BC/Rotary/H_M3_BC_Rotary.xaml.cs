using System.Windows.Media;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using VisiWin.DataAccess;

namespace HMI.Handmenu
{
    /// <summary>
    /// Interaction logic for ButtonsView.xaml
    /// </summary>
    [ExportView("H_M3_BC_Rotary")]
 
    public partial class H_M3_BC_Rotary : View
    {
        IVariableService VS;
        IVariable Status;
        public H_M3_BC_Rotary()
        {
            this.InitializeComponent();
            VS = ApplicationService.GetService<IVariableService>();
            Status = VS.GetVariable("NL.PLC.Blocks.3 Modul 3.10 Reinigung.02 Drehen / Greifer / Paddel.DB Reinigen DGP HMI.Actual value.Drehantrieb.Position Textliste");
            Status.Change += Status_Change;
        }
        private void Status_Change(object sender, VariableEventArgs e)
        {
            if ((short)e.Value != 3)
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