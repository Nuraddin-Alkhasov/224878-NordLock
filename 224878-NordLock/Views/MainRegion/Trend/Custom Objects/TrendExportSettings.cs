
using System;

namespace HMI.Views.MainRegion
{
    public class TrendExportSettings
    {
        public string ArchiveName { get; set; }
        public string ExportFileName { get; set; } = "TrendExport.csv";
        public DateTime StartTime { get; set; } = DateTime.MinValue;
        public DateTime StopTime { get; set; } = DateTime.MaxValue;
        public TimeSpan TrendDataSeconds { get; set; }
    }
}
