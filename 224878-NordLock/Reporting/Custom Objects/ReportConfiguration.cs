using System.Collections.ObjectModel;
using Microsoft.Reporting.WinForms;

namespace HMI.Reporting
{
    /// <summary>
    /// Container der alle Informationen hält, die der ReportingService benötigt um einen LocalReport zu konfigurieren.
    /// </summary>
    public class ReportConfiguration
    {
        /// <summary>
        /// Erstellt ein leeres ReportConfig-Objekt
        /// </summary>
        public ReportConfiguration()
        {
            this.DataSources = new Collection<ReportDataSource>();
            this.ReportParameters = new Collection<ReportParameter>();
            this.SubReportInfos = new Collection<SubReportInfo>();
        }

        /// <summary>
        /// Sammlung der Daten, die als DataSet im Report definiert wurden.
        /// </summary>
        public Collection<ReportDataSource> DataSources { get; private set; }

        public string DefaultExportPath { get; set; }

        public string FileNamePrefix { get; set; }

        /// <summary>
        /// Eine Sammlung von Parametern die dem Report übergeben werden sollen.
        /// </summary>
        public Collection<ReportParameter> ReportParameters { get; private set; }

        /// <summary>
        /// Der Pfad zu der rdlc-Datei. Dieser kann ein absoluter Pfad oder ein Pfad relativ zu der ausführenden exe-Datei sein.
        /// </summary>
        public string ReportPath { get; set; }

        /// <summary>
        /// Sammlung mit allen notwendigen Informationen zu den SubReports
        /// </summary>
        public Collection<SubReportInfo> SubReportInfos { get; private set; }
    }
}