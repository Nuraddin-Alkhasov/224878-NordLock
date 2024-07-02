using System;
using HMI.Interfaces;
using HMI.Module;
using HMI.Services;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;

namespace HMI.Diagnose
{
	/// <summary>
	/// Interaction logic for UserAdd.xaml
	/// </summary>
    [ExportView("Backup")]
	public partial class Backup : VisiWin.Controls.View, IView
	{
        IBackup BackupService = ApplicationService.GetService<IBackup>();

        public Backup()
		{
			this.InitializeComponent();
        }

   
        private void BackupNow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new DoBackup();

        }

        private void Start_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BackupService.Start();
            start.IsEnabled = false;
            stop.IsEnabled = true;
        }

        private void Stop_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BackupService.Stop();
            start.IsEnabled = true;
            stop.IsEnabled = false;
        }

        private void View_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                tbTime.Value = new DateTime(2000,01,01,BackupService.Time.Hours, BackupService.Time.Minutes, BackupService.Time.Seconds);
                if (BackupService.isRunning)
                {
                    start.IsEnabled = false;
                    stop.IsEnabled = true;
                }
                else
                {
                    start.IsEnabled = true;
                    stop.IsEnabled = false;
                }
            }
        }

        private void TbTime_ValueChanged(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue)
            {
                Stop_Click(this, null);
                bool b = (new LocalDBAdapter("UPDATE Config SET Value = '" + GetDataTimeToFormat() + "' WHERE Variable = 'BackupTime' ;").DB_Input());

                BackupService.Time = new TimeSpan(tbTime.Value.Hour, tbTime.Value.Minute, tbTime.Value.Second);
                Start_Click(this, null);
            }
            
        }
        private string GetDataTimeToFormat()
        {
            return DateTime.Now.ToString("yyyy-MM-dd") + " " + tbTime.Value.ToLongTimeString();
        }
    }
}