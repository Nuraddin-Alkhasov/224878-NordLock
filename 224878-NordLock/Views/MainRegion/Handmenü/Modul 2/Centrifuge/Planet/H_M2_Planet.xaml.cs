using System.Windows.Media;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using VisiWin.DataAccess;

namespace HMI.Handmenu
{
    /// <summary>
    /// Interaction logic for ButtonsView.xaml
    /// </summary>
    [ExportView("H_M2_Planet")]
 
    public partial class H_M2_Planet : View
    {
        IVariableService VS;
        IVariable Status;
        public H_M2_Planet()
        {
            this.InitializeComponent();
            VS = ApplicationService.GetService<IVariableService>();
            Status = VS.GetVariable("NL.PLC.Blocks.2 Modul 2.10 Zentrifuge.5 Planet.DB BS Planet HMI.Istwerte.Planet Position Textliste");
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