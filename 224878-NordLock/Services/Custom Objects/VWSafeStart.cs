using HMI.Views.MessageBoxRegion;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HMI.Services.Custom_Objects
{
    class VWSafeStart
    {
        private void InitializeSafeStart()
        {
           
        }
        private bool KillVisiWin()
        {
            foreach (Process pr in VW_Processes)
            {
                try
                {
                    pr.Kill();
                }
                catch
                {

                    MessageBox.Show("Houston we have a problem. VisiWin process is running. Our space fleet was unable to destroy it. Admiral SpongeBob requires some backup.Try to run the visualisation as Administrator.", "You shall not pass", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                    return false;
                }
            }

            return true;
        }

        ObservableCollection<Process> VW_Processes= new ObservableCollection<Process>();
        private ObservableCollection<Process> GetVW_Processes()
        {
            Process[] temp = Process.GetProcesses();
            ObservableCollection<Process> temp2 = new ObservableCollection<Process>();
            foreach (Process pr in temp)
            {
                if (pr.ProcessName == "VisiWin.CfgAccess" || pr.ProcessName == "VisiWin.Manager")
                {
                    temp2.Add(pr);
                }
            }
            return temp2;
        }

        public void DoWork()
        {

            VW_Processes = GetVW_Processes();
            int i = 0;
          
            while (VW_Processes.Count!=0)
            {
                if (i == 10)
                {

                    Task obTask = Task.Run(() =>
                    {
                        Thread.Sleep(4000);
                        ProcessStartInfo proc = new ProcessStartInfo
                        {
                            WindowStyle = ProcessWindowStyle.Hidden,
                            FileName = "cmd",
                            Arguments = "/C shutdown -f -r -t 15"
                        };
                        Process.Start(proc);
                    });

               
                    MessageBox.Show("Houston we have a problem. VisiWin process is running. Our space fleet was unable to destroy it. Admiral SpongeBob has pressed on the big red button which will restart this panel shortly.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                }

                if (KillVisiWin())
                {
                    VW_Processes = GetVW_Processes();
                    Thread.Sleep(500);
                    i++;
                }
                else
                {
                    Task obTask = Task.Run(() =>
                    {
                        Environment.Exit(0);
                    });

                    break;
                }

            }
        }
    }

    
}
