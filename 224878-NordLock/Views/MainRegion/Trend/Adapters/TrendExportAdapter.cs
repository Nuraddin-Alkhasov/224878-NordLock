using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Input;
using HMI.Interfaces;
using HMI.Resources;
using HMI.Services;
using HMI.Views.MainRegion;
using VisiWin.ApplicationFramework;
using VisiWin.Commands;
using VisiWin.DataAccess;

namespace HMI.Adapter
{
    [ExportAdapter("TrendExportAdapter")]
    public class TrendExportAdapter : AdapterBase
    {
        public static readonly DependencyProperty ExportFileNameProperty = DependencyProperty.Register("ExportFileName", typeof(string), typeof(TrendExportAdapter), new PropertyMetadata("TrendExport.csv"));
        public static readonly DependencyProperty ExportProgressProperty = DependencyProperty.Register("ExportProgress", typeof(int), typeof(TrendExportAdapter), new PropertyMetadata(0));
        public static readonly DependencyProperty SelectedArchiveNameProperty = DependencyProperty.Register("SelectedArchiveName", typeof(string), typeof(TrendExportAdapter), new PropertyMetadata(null));
        public static readonly DependencyProperty StartTimeProperty = DependencyProperty.Register("StartTime", typeof(DateTime), typeof(TrendExportAdapter), new PropertyMetadata(DateTime.Now.AddHours(-1)));
        public static readonly DependencyProperty StopTimeProperty = DependencyProperty.Register("StopTime", typeof(DateTime), typeof(TrendExportAdapter), new PropertyMetadata(DateTime.Now));
        public static readonly DependencyProperty TrendDataSecondsProperty = DependencyProperty.Register("TrendDataSeconds", typeof(int), typeof(TrendExportAdapter), new PropertyMetadata(0));
        private readonly ITrendExport trendExportService;


        public TrendExportAdapter()
        {
            trendExportService = ApplicationService.GetService<ITrendExport>();
            //trendExportService.TrendExportCompleted += TrendExportService_TrendExportCompleted;
            trendExportService.TrendExportProgressChanged += TrendExportService_TrendExportProgressChanged;

            ExportToFileCommand = new ActionCommand(ExportToFileCommandExecuted);
            ExportStopCommand = new ActionCommand(ExportStopCommandExecuted);

            TrendDataSeconds = 10;
            StartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour - 1, 0, 0);
            StopTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour + 1, 0, 0);
           
        }


        public string ExportFileName
        {
            get { return (string)this.GetValue(ExportFileNameProperty); }
            set { this.SetValue(ExportFileNameProperty, value); }
        }
        public int ExportProgress
        {
            get { return (int)this.GetValue(ExportProgressProperty); }
            set { this.SetValue(ExportProgressProperty, value); }
        }
        public ICommand ExportStopCommand { get; set; }
        public ICommand ExportToFileCommand { get; set; }
        public string SelectedArchiveName
        {
            get { return (string)this.GetValue(SelectedArchiveNameProperty); }
            set { this.SetValue(SelectedArchiveNameProperty, value); }
        }
        public DateTime StartTime
        {
            get { return (DateTime)this.GetValue(StartTimeProperty); }
            set { this.SetValue(StartTimeProperty, value); }
        }
        public DateTime StopTime
        {
            get { return (DateTime)this.GetValue(StopTimeProperty); }
            set { this.SetValue(StopTimeProperty, value); }
        }
        public int TrendDataSeconds
        {
            get { return (int)this.GetValue(TrendDataSecondsProperty); }
            set { this.SetValue(TrendDataSecondsProperty, value); }
        }

        private void ExportStopCommandExecuted(object parameter)
        {
            this.trendExportService.CancelExport();
        }

        private void ExportToFileCommandExecuted(object parameter)
        {
            if (StartTime < StopTime) 
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + "_" + SelectedArchiveName; // Default file name
                dlg.DefaultExt = ".csv";
                dlg.Filter = "CSV (Trennzeichen-getrennt) (.csv)|*.csv";
                dlg.InitialDirectory = (new LocalResources()).Paths.Project.ExportPath;
                dlg.CheckPathExists = true;
                dlg.RestoreDirectory = false;

                bool? result = dlg.ShowDialog();
                if (result == true)
                {
                    ExportFileName = dlg.FileName;
                    if (this.trendExportService != null)
                    {
                        this.ExportProgress = 0;

                        var settings = new TrendExportSettings
                        {
                            ArchiveName = this.SelectedArchiveName,
                            ExportFileName = this.ExportFileName,
                            StartTime = this.StartTime,
                            StopTime = this.StopTime,
                            TrendDataSeconds = new TimeSpan(0, 0, this.TrendDataSeconds)
                        };

                        var options = new TrendExportOptions();
                        this.trendExportService.ExportTrendData(settings, options);
                    }
                }
            }
        }



        private void TrendExportService_TrendExportCompleted(object sender, TrendExportResult e)
        {
         
        }

        private void TrendExportService_TrendExportProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.ExportProgress = e.ProgressPercentage;
        }

    }
}