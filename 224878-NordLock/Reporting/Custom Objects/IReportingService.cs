using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Reporting.WinForms;


namespace HMI.Reporting
{
    /// <summary>
    /// Schnittstellendeklaration für den ReportingService
    /// </summary>
    public interface IReportingService
    {
        /// <summary>
        /// Aktualisiert den LocalReport so, dass er auf die übergebene ReportConfiguration passt.
        /// </summary>
        /// <param name="localReport"></param>
        /// <param name="reportConfiguration"></param>
        Task ConfigureReportAsync(LocalReport localReport, ReportConfiguration reportConfiguration);

        /// <summary>
        /// Erzeugt ein neues LocalReport-Objekt anhand der ReportConfiguration.
        /// </summary>
        /// <param name="reportConfiguration"></param>
        /// <returns></returns>
        LocalReport CreateNewReportBy(ReportConfiguration reportConfiguration);

        /// <summary>
        /// Exportiert den durch die ReportConfiguration definierten Report asynchron, dabei wird der Standard-Exportpfad aus dem
        /// ReportConfigrations-Objekt genutzt.
        /// Das Format wird durch RenderFormat bestimmt.
        /// Der Dateiname wird aus dem Standard Dateinamens-Präfix, einem Zeitstempel und dem exportContext gebildet.
        /// Wird ein Abbruch über das cancellationToken angefordert wird versucht, die Ausführung zu beenden.
        /// Wird ein Abbruch gefordert bevor die Ausführung beendet wurde beim awaiten des Task eine OperationCanceledException geworfen.
        /// </summary>
        /// <param name="reportConfiguration">Der zu exportierende Report</param>
        /// <param name="renderFormat">Das Ausgabeformat</param>
        /// <param name="exportContext">Der Kontext des exportieren</param>
        /// <param name="cancellationToken">Ein Token mit dem die Task abgebrochen werden kann</param>
        /// <returns>Ein Task-Objekt welches die Asynchrone-Operation repräsentiert</returns>
        Task ExportReportAsync(ReportConfiguration reportConfiguration, RenderFormat renderFormat, string exportContext,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Exportiert den durch die ReportConfiguration definierten Report asynchron zu dem spezifiziertem Dateipfad.
        /// Ansonsten genau wie ExportReportAsync().
        /// </summary>
        /// <param name="reportConfiguration">Der zu exportierende Report</param>
        /// <param name="path">Der Pfad an dem der Report gespeichert werden soll</param>
        /// <param name="renderFormat">Das Ausgabeformat</param>
        /// <param name="cancellationToken">Ein Token mit dem die Task abgebrochen werden kann</param>
        /// <returns>Ein Task-Objekt welches die Asynchrone-Operation repräsentiert</returns>
        Task ExportReportToAsync(ReportConfiguration reportConfiguration, string path, RenderFormat renderFormat,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Exportiert eine Liste von Reports. Die einzelnen Export-Anweisungen werden durch Werte Tupel gebildet.
        /// Die Tupel müssen eine ReportConfiguration und einen ExportContext enthalten.
        /// Alle Reports werden mit dem gleichen RenderFormat exportiert.
        /// Der Progress wird als Anzahl der bereits vollständig exportierten Reports und dem aktuellen ExportContext gebildet.
        /// </summary>
        /// <param name="exportConfigurations">Eine Liste von Tupeln die, die einzelnen Export-Operationen repräsentieren</param>
        /// <param name="renderFormat">Das Format mit dem die Reports gerendert werden sollen</param>
        /// <param name="cancellationToken">Ein Token um die Operation abzubrechen</param>
        /// <param name="progress">Dient als Callback um den Bearbeitungsstatus anzuzeigen.</param>
        /// <returns>Ein Task-Objekt welches die Asynchrone-Operation repräsentiert</returns>
        Task ExportReportAsync(IEnumerable<Tuple<ReportConfiguration, string>> exportConfigurations, RenderFormat renderFormat,
            CancellationToken cancellationToken = default(CancellationToken), IProgress<Tuple<int, string>> progress = null);

        /// <summary>
        /// Exportiert eine Liste von Reports. Die einzelnen Export-Anweisungen werden durch Werte Tupel gebildet.
        /// Die Tupel müssen eine ReportConfiguration und einen gültigen Datei-Pfad enthalten.
        /// Alle Reports werden mit dem gleichen RenderFormat exportiert.
        /// Der Progress wird als Anzahl der bereits vollständig exportierten Reports und dem aktuellen Datei-Pfad gebildet.
        /// </summary>
        /// <param name="exportConfigurations">Eine Liste von Tupeln die, die einzelnen Export-Operationen repräsentieren</param>
        /// <param name="renderFormat">Das Format mit dem die Reports gerendert werden sollen</param>
        /// <param name="cancellationToken">Ein Token um die Operation abzubrechen</param>
        /// <param name="progress">Dient als Callback um den Bearbeitungsstatus anzuzeigen.</param>
        /// <returns>Ein Task-Objekt welches die Asynchrone-Operation repräsentiert</returns>
        Task ExportReportToAsync(IEnumerable<Tuple<ReportConfiguration, string>> exportConfigurations, RenderFormat renderFormat,
            CancellationToken cancellationToken = default(CancellationToken), IProgress<Tuple<int, string>> progress = null);
    }
}