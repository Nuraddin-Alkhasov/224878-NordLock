namespace HMI.Reporting
{
    /// <summary>
    /// Leiten Sie diese Klasse ab, um eine Klasse zu bauen, die ein ReportConfiguration-Object zu einem bestimmten rdlc-Report erzeugt.
    /// </summary>
    public abstract class Report
    {
        /// <summary>
        /// Stellt einen Container dar, der Daten über einen konstanten, sprachabhängigen Reportparameter hält.
        /// </summary>
        protected class ParameterInfo
        {
            /// <summary>
            /// Initialisiert ein Parameterobjekt mit den angegbenen Daten.
            /// </summary>
            /// <param name="name"></param>
            /// <param name="localiableString"></param>
            public ParameterInfo(string name, string localiableString)
            {
                this.Name = name;
                this.LocaliableString = localiableString;
            }

            /// <summary>
            /// Name des im Report definierten Parameters
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// Lokalisierbare Zeichenkette für den Wert des Parameters.
            /// </summary>
            public string LocaliableString { get; private set; }
        }
    }
    
}

//Verwenden Sie dies als Vorlage für die Klassen, die sie von VisiWin.Reporting.Components.Report ableiten.

//class CurrentAlarmsReport : VisiWin.Reporting.Components.Report
//{
//    private class DataSetNames
//    {
//        public const string exampleDataSetName = "exampleDataSetName";
//        ...
//    }
//    private class Parameters
//    {
//        public readonly static IEnumerable<ParameterInfo> AllParameterInfoInOne = new System.Collections.ObjectModel.Collection<ParameterInfo>() {
//            new ParameterInfo("ParameterName", "@Example.Localizable.String.ParameterName"),
//            ...
//        };

//        public const string ExampleParameterName = "ExampleParameterName";
//        ...
//    }
//    public const string Path = @"Example\ExampleReport.rdlc";
//    public const string DefaultExportPath = @"Example\ExampleReport";
//    public const string FileNamePrefix = "ExampleReport";


//    public static ReportConfiguration GetReportConfiguration(IEnumerable<ExampleDataItem> exampleData)
//    {
//        var dataset = new ExampleDataSet.ExampleDataTable();
//        dataset.FillWith(exampleData);

//        var config = new ReportConfiguration();
//        config.ReportPath = Path;
//        config.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource(DataSetNames.exampleDataSetName, dataset as System.Data.DataTable));
//        foreach (var paraInfo in Parameters.AllParameterInfoInOne)
//            config.ReportParameters.Add(new Microsoft.Reporting.WinForms.ReportParameter(paraInfo.Name, VisiWin.ApplicationFramework.ApplicationService.GetText(paraInfo.LocaliableString)));
//        config.ReportParameters.Add(new Microsoft.Reporting.WinForms.ReportParameter(Parameters.ExampleParameterName, "exampleParameterValue"));

//        return config;
//    }

//}