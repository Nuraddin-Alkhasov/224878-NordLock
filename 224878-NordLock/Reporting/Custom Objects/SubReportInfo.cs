using System.Collections.ObjectModel;
using Microsoft.Reporting.WinForms;

namespace HMI.Reporting
{
    /// <summary>
    /// Container für die notwendigen Informationen über einen Subreport.
    /// </summary>
    public class SubReportInfo
    {
        public SubReportInfo()
        {
            this.ReportDataSources = new Collection<ReportDataSource>();
        }

        /// <summary>
        /// Sammlung mit den DataSets, die im SubReport definiert sind.
        /// </summary>
        public Collection<ReportDataSource> ReportDataSources { get; private set; }

        public string ReportName { get; set; }

        /// <summary>
        /// Der Pfad zu der rdlc-Datei. Dieser kann ein absoluter Pfad oder ein Pfad relativ zu der ausführenden exe-Datei sein.
        /// </summary>
        public string ReportPath { get; set; }
    }
}