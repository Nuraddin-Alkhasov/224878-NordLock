using System;
using System.Windows.Media;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using VisiWin.DataAccess;

namespace HMI.Handmenu
{
    /// <summary>
    /// Interaction logic for ButtonsView.xaml
    /// </summary>
    [ExportView("H_M1_LD_Tilt")]

    public partial class H_M1_LD_Tilt : View
    {
        IVariableService VS;
        IVariable Status;
        public H_M1_LD_Tilt()
        {
            this.InitializeComponent();

            VS = ApplicationService.GetService<IVariableService>();

            Status = VS.GetVariable("NL.PLC.Blocks.1 Modul 1.01 Lifting Tilting Device.03 Tilt.DB LD Tilt HMI.Actual value.Kippantrieb Textliste");
            Status.Change += Status_Change;
        }

        private void Status_Change(object sender, VariableEventArgs e)
        {
            if ((short)e.Value != 4)
            {
                Home.IsBlinkEnabled = true;
                Home.Background = (LinearGradientBrush) FindResource("FP_Gray_Gradient");
            }
            else
            {
                Home.IsBlinkEnabled = false;
                Home.Background = (LinearGradientBrush)FindResource("FP_Green_Gradient");
            }


        }
    }
}