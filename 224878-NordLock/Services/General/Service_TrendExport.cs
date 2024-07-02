using HMI.Interfaces;
using HMI.Views.MainRegion;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;
using VisiWin.Trend;

namespace HMI.Services
{
    /// <summary>
    /// Interface Service TrendExport
    /// </summary>
  

    /// <summary>
    /// Service Trendexport
    /// </summary>
    [ExportService(typeof(ITrendExport))]
    public class Service_TrendExport : ServiceBase, ITrendExport
    {
       
       
        private TextWriter textWriter;

        
        private TrendExportSettings TES;
        private IArchive TrendArchive;
        private readonly BackgroundWorker BGW;

        private readonly ITrendService trendService = ApplicationService.GetService<ITrendService>();

       

        public Service_TrendExport()
        {
            this.BGW = new BackgroundWorker();
            this.BGW.DoWork += BGW_DoWork;
            this.BGW.RunWorkerCompleted += BGW_RunWorkerCompleted;
            this.BGW.ProgressChanged += BGW_ProgressChanged;
            this.BGW.WorkerReportsProgress = true;
            this.BGW.WorkerSupportsCancellation = true;
        }

        private void ExportTrendData()
        {
            if (this.trendService != null)
            {
                TrendArchive = trendService.GetArchive(TES.ArchiveName);
                TrendArchive.GetTrendsDataCompleted += TrendArchive_GetTrendsDataCompleted;
                TrendArchive.GetTrendsDataAsync(TrendArchive.TrendNames.ToArray(), TES.StartTime, TES.StopTime, null, 0);
            }
        }


        private void TrendArchive_GetTrendsDataCompleted(object sender, GetTrendsDataCompletedEventArgs e)
        {
            TrendArchive.GetTrendsDataCompleted -= TrendArchive_GetTrendsDataCompleted;
            SaveExportData(e);
        }

        private void SaveExportData(GetTrendsDataCompletedEventArgs e)
        {
            if (!BGW.IsBusy)
            {
                BGW.RunWorkerAsync(e);
            }
        }

        private void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            GetTrendsDataCompletedEventArgs trendsData = (GetTrendsDataCompletedEventArgs)e.Argument;

            List<List<TrendRecord>> temp = new List<List<TrendRecord>>();

            for (var n = 0; n < trendsData.Trends.Count; n++)
            {
                if (BGW.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                IDataSample[] dataSample = trendsData.Data.GetValue(n) as IDataSample[];
                List<TrendRecord> local = new List<TrendRecord>();
                    
                if (dataSample == null) 
                    return;
                if (dataSample[0].YValue.GetType().Name != "Boolean")
                {
                    local.AddRange(dataSample.Select(ds => new TrendRecord() { Time = ds.Time, Value = Math.Round((double)ds.YValue, 1), TrendIndex = n }));
                }
                else 
                {
                    local.AddRange(dataSample.Select(ds => new TrendRecord() { Time = ds.Time, Value = ds.YValue.ToString()=="false" ? 0 : 1 , TrendIndex = n }));

                }

                temp.Add(local);
            }
            temp.Add(new List<TrendRecord>());
            for (var n = 0; n < trendsData.Trends.Count; n++)
            {
                temp[temp.Count - 1].AddRange(temp[n]);
            }

            temp[temp.Count - 1] = temp[temp.Count - 1].OrderBy(dataSample => dataSample.Time).ToList();
            int index = 0;
            TrendRecord lastrecord = new TrendRecord() { Time= DateTime.MinValue, Value=0, TrendIndex=0};
            foreach (TrendRecord record in temp[temp.Count - 1])
            {
                index++;
                if (record.Time != lastrecord.Time) 
                {
                    if (record.Time >= TES.StartTime && record.Time <= TES.StopTime)
                    {
                        TrendRecord[] tr = new TrendRecord[2];
                        tr[record.TrendIndex] = record;

                        switch (record.TrendIndex)
                        {
                            case 0:
                                var found = temp[1].Where(x => x.Time <= record.Time).OrderBy(ds => ds.Time).ToArray();
                                if (found.Length > 0)
                                {
                                    tr[1] = found.Last();
                                }
                                else { tr[1] = new TrendRecord() { Time = tr[0].Time, Value = 0, TrendIndex = 0 }; }
                                break;
                            case 1:
                                found = temp[0].Where(x => x.Time <= record.Time).OrderBy(ds => ds.Time).ToArray();
                                if (found.Length > 0)
                                {
                                    tr[0] = found.Last();
                                }
                                else { tr[0] = new TrendRecord() { Time = tr[1].Time, Value = 0, TrendIndex = 0 }; }
                                break;
                        }

                        AddRecordSet(new Records() { Time = record.Time, Data = tr });
                    }
                    lastrecord = record;
                }
                   
                var percent = Convert.ToInt32(Math.Truncate((double)100 / temp[temp.Count - 1].Count * index));
                BGW.ReportProgress(percent);
            }
            BGW.ReportProgress(100);
            
        }
        private void BGW_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.TrendExportProgressChanged?.Invoke(sender, e);
        }

        private void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.textWriter.Close();
            this.textWriter.Dispose();
            this.textWriter = null;
        }

 
        public event ProgressChangedEventHandler TrendExportProgressChanged;
        public event EventHandler<TrendExportResult> TrendExportCompleted;

        public void CancelExport()
        {
            BGW.CancelAsync();
        }

     
        public void ExportTrendData(TrendExportSettings settings, TrendExportOptions options = null)
        {
            if (settings != null)
            {
                TES = settings;
                ExportTrendData();
            }
        }


        private void AddRecordSet(Records r)
        {
            if (this.textWriter == null)
            {
                if (File.Exists(TES.ExportFileName))
                {
                    File.Delete(TES.ExportFileName);
                }
                this.textWriter = File.CreateText(TES.ExportFileName);

                string header = ApplicationService.GetText("@Application.TrendSystem.Views.ExportView.DataGridColumnTime") + ";"
                        + TrendArchive.GetTrend(TrendArchive.TrendNames[0]).Text + ";" 
                        + TrendArchive.GetTrend(TrendArchive.TrendNames[1]).Text;

                this.textWriter.WriteLine(header);
            }

            var sB = new StringBuilder();
            sB.Append(r.Time.ToString(CultureInfo.InvariantCulture));

            foreach (TrendRecord tr in r.Data)
            {
                sB.Append(";");
                sB.Append(tr.Value);
            }

            this.textWriter.WriteLine(sB.ToString());
        }

        private class TrendRecord
        {
            public DateTime Time { get; set; }
            public double Value { get; set; }
            public int TrendIndex { get; set; }
        }


        private class Records
        {
            public TrendRecord [] Data { get; set; }
            public DateTime Time { get; set; }
        }
    }

  
}