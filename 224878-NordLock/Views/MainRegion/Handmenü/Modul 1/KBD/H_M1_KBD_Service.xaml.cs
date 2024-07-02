using HMI.Views.MessageBoxRegion;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using VisiWin.Helper;
using VisiWin.Language;
using VisiWin.Logging;

namespace HMI.Handmenu
{
    /// <summary>
    /// Interaction logic for KeyAndSwitchView.xaml
    /// </summary>
    [ExportView("H_M1_KBD_Service")]

    public partial class H_M1_KBD_Service : View
    {
        public H_M1_KBD_Service()
        {
            this.InitializeComponent();
        }
    }
}