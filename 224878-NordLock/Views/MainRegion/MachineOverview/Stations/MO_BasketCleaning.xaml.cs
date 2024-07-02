using HMI.Views.DialogRegion;
using System;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Views.MainRegion.MachineOverview
{

    [ExportView("MO_BasketCleaning")]
    public partial class MO_BasketCleaning : VisiWin.Controls.View
    {

        public MO_BasketCleaning()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (new SP
            {
                Module = 3,
                M1_Station = 0,
                M2_Station = 0,
                M3_Station = 8,
                M4_Station = 0,
                OvenTray = 0,
                TB_Shelve = 0,
                TB_Level = 0,
                Header = "@Status.Text34",
                Type = "Basket"
            }).Execute();
        }

    }
}



