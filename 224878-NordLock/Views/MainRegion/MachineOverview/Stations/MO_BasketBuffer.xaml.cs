using HMI.Views.DialogRegion;
using System;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Views.MainRegion.MachineOverview
{

    [ExportView("MO_BasketBuffer")]
    public partial class MO_BasketBuffer : VisiWin.Controls.View
    {

        public MO_BasketBuffer()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int M3_Temp = Convert.ToInt32(((VisiWin.Controls.Button)sender).Tag);
            string Header_Temp = "";
            switch (M3_Temp)
            {
                case 2: Header_Temp = "@Status.Text28"; break;
                case 3: Header_Temp = "@Status.Text29"; break;
                case 4: Header_Temp = "@Status.Text30"; break;
                case 5: Header_Temp = "@Status.Text31"; break;
                case 6: Header_Temp = "@Status.Text32"; break;
                case 7: Header_Temp = "@Status.Text33"; break;
            }
            (new SP
            {
                Module = 3,
                M1_Station = 0,
                M2_Station = 0,
                M3_Station = M3_Temp,
                M4_Station = 0,
                OvenTray = 0,
                TB_Shelve = 0,
                TB_Level = 0,
                Header = Header_Temp,
                Type = "Basket"
            }).Execute();
        }

    }
}



