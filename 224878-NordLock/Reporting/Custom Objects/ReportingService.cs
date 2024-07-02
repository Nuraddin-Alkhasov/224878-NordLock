using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Reporting.WinForms;
using VisiWin.ApplicationFramework;

namespace HMI.Reporting
{
    [ExportService(typeof(IReportingService))]
    [Export(typeof(IReportingService))]
    public class ReportingService : ServiceBase, IReportingService
    {
        private static readonly SemaphoreSlim renderLocker = new SemaphoreSlim(10);
        private static readonly RenderModeConverter renderModeConverter = new RenderModeConverter();
        private static readonly SemaphoreSlim writeLocker = new SemaphoreSlim(1);

        public async Task ConfigureReportAsync(LocalReport localReport, ReportConfiguration reportConfiguration)
        {
            if (localReport == null || reportConfiguration == null)
                return;

            //Report-Definition aus Datei laden
            await Task.Run(() =>
                {
                    localReport.DataSources.Clear();

                    using (var fs = new FileStream(Path.GetFullPath(reportConfiguration.ReportPath), FileMode.Open, FileAccess.Read))
                    {
                        localReport.LoadReportDefinition(fs);
                    }

                    //Report-Definitionen für die SubReports aus Datei laden
                    foreach (var subReport in reportConfiguration.SubReportInfos)
                    {
                        localReport.SubreportProcessing += new SubreportProcessingEventHandler(this.CreateEventHandlerFor(subReport));
                        using (var fs = new FileStream(Path.GetFullPath(subReport.ReportPath), FileMode.Open, FileAccess.Read))
                        {
                            localReport.LoadSubreportDefinition(subReport.ReportName, fs);
                        }
                    }

                    //DataSets zum MainReport hinzufügen
                    foreach (var rds in reportConfiguration.DataSources)
                    {
                        localReport.DataSources.Add(rds);
                    }

                    //Übergabe der ParameterInfos an den MainReport
                    localReport.SetParameters(reportConfiguration.ReportParameters);
                });
        }

        public LocalReport CreateNewReportBy(ReportConfiguration reportConfiguration)
        {
            var r = new LocalReport();
            this.ConfigureReportAsync(r, reportConfiguration).Wait();
            return r;
        }

        //-----------------------------------------------

        public async Task ExportReportAsync(ReportConfiguration reportConfiguration, RenderFormat renderFormat, string exportContext,
                                            CancellationToken cancellationToken = default(CancellationToken))
        {
            await Task.Run(() => this.ExportReportAsyncInternal(reportConfiguration, renderFormat, exportContext, cancellationToken));
        }

        //-----------------------------------------------

        public async Task ExportReportAsync(IEnumerable<Tuple<ReportConfiguration, string>> exportConfigurations, RenderFormat renderFormat,
                                            CancellationToken cancellationToken = default(CancellationToken), IProgress<Tuple<int, string>> progress = null)
        {
            await Task.Run(() => this.ExportReportAsyncInternal(exportConfigurations, renderFormat, cancellationToken, progress));
        }

        //-----------------------------------------------

        public async Task ExportReportToAsync(ReportConfiguration reportConfiguration, string path, RenderFormat renderFormat,
                                              CancellationToken cancellationToken = default(CancellationToken))
        {
            await Task.Run(() => this.ExportReportToAsyncInternal(reportConfiguration, path, renderFormat, cancellationToken));
        }

        //-----------------------------------------------

        public async Task ExportReportToAsync(IEnumerable<Tuple<ReportConfiguration, string>> exportConfigurations, RenderFormat renderFormat,
                                              CancellationToken cancellationToken = default(CancellationToken), IProgress<Tuple<int, string>> progress = null)
        {
            await Task.Run(() => this.ExportReportToAsyncInternal(exportConfigurations, renderFormat, cancellationToken, progress));
        }

        private static string RaiseVersionCount(string fullPath)
        {
            var count = 1;

            var fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
            var extension = Path.GetExtension(fullPath);
            var path = Path.GetDirectoryName(fullPath);
            var newFullPath = fullPath;
            string tempFileName = null;

            while (File.Exists(newFullPath))
            {
                tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                newFullPath = Path.Combine(path, tempFileName + extension);
            }

            return newFullPath;
        }

        /// <summary>
        /// Diese Methode erzeugt eine CallBack-Action für den SubreportProcessingEventHandler für einen spezifischen SubReport.
        /// </summary>
        /// <param name="sri"></param>
        /// <returns></returns>
        private Action<object, SubreportProcessingEventArgs> CreateEventHandlerFor(SubReportInfo sri)
        {
            return (sender, e) =>
                {
                    if (e.ReportPath == sri.ReportName)
                    {
                        e.DataSources.Clear();
                        foreach (var rds in sri.ReportDataSources)
                        {
                            e.DataSources.Add(rds);
                        }
                    }
                };
        }

        //------------------------------------------------

        /// <summary>
        /// Diese Methode arbeitet die gegeben Liste von Export-Konfigurationen in einer foreach-Schleife ab und nutzt zum
        /// exportieren die übergeben Funktion.
        /// </summary>
        /// <param name="exportConfigurations"></param>
        /// <param name="exportFunction"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        private async Task ExportAsync(IEnumerable<Tuple<ReportConfiguration, string>> exportConfigurations, Func<Tuple<ReportConfiguration, string>, Task> exportFunction,
                                       IProgress<Tuple<int, string>> progress)
        {
            var i = 1;
            progress.Report(new Tuple<int, string>(0, string.Empty));
            foreach (var t in exportConfigurations)
            {
                progress.Report(new Tuple<int, string>(i, t.Item2));
                await exportFunction.Invoke(t);
                progress.Report(new Tuple<int, string>(i++, string.Empty));
            }
        }

        private async Task ExportReportAsyncInternal(ReportConfiguration reportConfiguration, RenderFormat renderFormat, string exportContext, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var completePath = this.GetCompletePathFor(reportConfiguration, renderFormat, exportContext);

            cancellationToken.ThrowIfCancellationRequested();

            await this.ExportReportToAsyncInternal(reportConfiguration, completePath, renderFormat, cancellationToken);
        }

        private async Task ExportReportAsyncInternal(IEnumerable<Tuple<ReportConfiguration, string>> exportConfigurations, RenderFormat renderFormat, CancellationToken cancellationToken,
                                                     IProgress<Tuple<int, string>> progress)
        {
            await this.ExportAsync(exportConfigurations, t => { return this.ExportReportAsyncInternal(t.Item1, renderFormat, t.Item2, cancellationToken); }, progress);
        }

        private async Task ExportReportToAsyncInternal(ReportConfiguration reportConfiguration, string path, RenderFormat renderFormat, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var localReport = new LocalReport())
            {
                //renderLock
                await renderLocker.WaitAsync(cancellationToken);
                try
                {
                    //LocalReport konfigurieren
                    await this.ConfigureReportAsync(localReport, reportConfiguration);

                    //warten, dass der LocalReport bereit ist zum Rendern
                    while (!localReport.IsReadyForRendering)
                    {
                        await Task.Delay(10);
                    }

                    //Report rendern
                    var renderTask = Task.Run(() => localReport.Render(renderModeConverter.Convert(renderFormat, null, false, CultureInfo.InvariantCulture) as string));

                    //Warten darauf, dass die Task fertig ist
                    //Ohne await um es möglich zu machen, dass das Rendern abgebrochen werden kann.
                    //denn wenn die Methode abgebrochen wird, wird das LocalReport-Objekt disposed
                    //und die Render-Methode erzeugt eine Exception die das Rendern beendet. 
                    while (!renderTask.IsCompleted)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await Task.Delay(10);
                    }

                    var renderedReport = await renderTask;

                    cancellationToken.ThrowIfCancellationRequested();

                    if (File.Exists(path))
                    {
                        //Wenn die Datei schon existiert wird hier die Operation abgebrochen und eine Exception geworfen
                        throw new ApplicationException($"File '{path}' does exist");
                    }

                    //Alle Verzeichnisse erzeugen, die nötig sind um die Datei abzuspeichern.
                    Directory.CreateDirectory(Path.GetDirectoryName(path));

                    //writeLock
                    await writeLocker.WaitAsync(cancellationToken);
                    try
                    {
                        //Die Daten in die Datei schreiben
                        using (var fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
                        {
                            //Daten raus schreiben
                            await fs.WriteAsync(renderedReport, 0, renderedReport.Length, cancellationToken);
                            fs.Flush();
                        }
                    }
                    catch
                    {
                        //wenn eine Datei erstellt wurde wird diese wieder gelöscht
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }

                        throw;
                    }
                    finally
                    {
                        writeLocker.Release();
                    }
                }
                finally
                {
                    renderLocker.Release();
                }
            }
        }

        private async Task ExportReportToAsyncInternal(IEnumerable<Tuple<ReportConfiguration, string>> exportConfigurations, RenderFormat renderFormat, CancellationToken cancellationToken,
                                                       IProgress<Tuple<int, string>> progress)
        {
            await this.ExportAsync(exportConfigurations, t => { return this.ExportReportToAsyncInternal(t.Item1, t.Item2, renderFormat, cancellationToken); }, progress);
        }

        private string GetCompletePathFor(ReportConfiguration reportConfiguration, RenderFormat renderFormat, string exportContext)
        {
            //relativen Pfad in einen absoluten Pfad umwandeln
            var aDicPath = Path.GetFullPath(reportConfiguration.DefaultExportPath);
            //Dateinamen mit Zeitstempel und der passenden Dateiendung erzeugen
            var fileName = reportConfiguration.FileNamePrefix + " - " + exportContext + " - " + DateTime.Now.ToString("yyyy-MM-dd _ HH-mm-ss", CultureInfo.InvariantCulture) +
                           (renderModeConverter.Convert(renderFormat, null, true, null) as string);
            //Pfad und Dateinamen zusammen setzen
            var completePath = Path.Combine(aDicPath, fileName);

            //Überprüfen, ob es schon eine Datei mit dem gleichen Namen gibt und den Dateinamen 
            //abändern falls dem so ist.
            if (File.Exists(completePath))
            {
                completePath = RaiseVersionCount(completePath);
            }

            return completePath;
        }
    }
}