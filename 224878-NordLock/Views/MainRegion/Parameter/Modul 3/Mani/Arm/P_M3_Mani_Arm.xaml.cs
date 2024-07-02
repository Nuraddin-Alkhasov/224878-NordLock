﻿using System;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
namespace HMI.Parameter
{
    /// <summary>
    /// Interaction logic for DigitalIOView.xaml
    /// </summary>
    [ExportView("P_M3_Mani_Arm")]
    public partial class P_M3_Mani_Arm : View
    {
		
        public P_M3_Mani_Arm()
        {
            this.InitializeComponent();
            
        }

        private void P_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            Reg.Content = new P_M3_Mani_Arm_P();
        }

        private void B_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            Reg.Content = new P_M3_Mani_Arm_B();
        }

        

        private void P_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Task obTask = Task.Run(() =>
            {
                Application.Current.Dispatcher.InvokeAsync((Action)delegate
                {
                    btnH_P.IsChecked = true;
                });
            });
        }

        private void Reg_Unloaded(object sender, RoutedEventArgs e)
        {
            btnH_P.IsChecked = false;
            Reg.Content = null;
        }
    }
}