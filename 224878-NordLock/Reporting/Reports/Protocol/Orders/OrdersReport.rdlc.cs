using VisiWin.ApplicationFramework;
using System.Threading.Tasks;
using HMI.Views.MainRegion.Protocol;
using System.Data;
using HMI.Module;
using Microsoft.Reporting.WinForms;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HMI.Reporting
{
    internal class OrdersReport
    {
        public static readonly string DefaultExportPath = @"Reports\Protocol\Orders\OrdersReport";
        public static readonly string FileNamePrefix = "OrdersReport";
        public static readonly string Path = @"Reporting\Reports\Protocol\Orders\OrdersReport.rdlc";

        public static async Task<ReportConfiguration> GetReportConfiguration()
        {
            await Task.Run(() => { });


            ProtocolAdapter adapter = ApplicationService.GetAdapter(nameof(ProtocolAdapter)) as ProtocolAdapter;

            DataTable Orders = (new LocalDBAdapter(adapter.GenerateSQLQuery().Replace("datetime(Orders.TimeStamp)", "TimeStamp"))).DB_Output();


            var config = new ReportConfiguration
            {
                //Standard Konfiguration des Reports übertragen
                ReportPath = Path,
                DefaultExportPath = DefaultExportPath,
                FileNamePrefix = FileNamePrefix
            };
            //DataSet in die Konfiguration einfügen

            config.DataSources.Add(new ReportDataSource("GetOrders", Orders));

            //Lokalisierbare ReportParameter in die Konfiguration einfügen
            foreach (var paraInfo in Parameters.LocalizableParameter)
            {
                config.ReportParameters.Add(new ReportParameter(paraInfo.Name, ApplicationService.GetText(paraInfo.LocaliableString)));
            }

            return config;
            
            //Erzeugen und Befüllen des DataSets
         
        }

        private class Parameters
        {
            public static readonly IEnumerable<ParameterInfo> LocalizableParameter = new Collection<ParameterInfo>
                                                                                         {
                                                                                             new ParameterInfo("OrdersLabel", "@Protocol.Text41"),
                                                                                             new ParameterInfo("StartLabel", "@Protocol.Text7"),
                                                                                             new ParameterInfo("OrderLabel", "@Protocol.Text1"),
                                                                                             new ParameterInfo("BatchLabel", "@Protocol.Text2"),
                                                                                             new ParameterInfo("ItemLabel", "@Protocol.Text3"),
                                                                                             new ParameterInfo("MRLabel", "@Protocol.Text4"),
                                                                                             new ParameterInfo("ChargesLabel", "@Protocol.Text6"),
                                                                                             new ParameterInfo("UserLabel", "@Protocol.Text5")
                                                                                         };
        }
    }
}