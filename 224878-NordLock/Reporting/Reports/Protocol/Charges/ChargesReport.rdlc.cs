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
    internal class ChargesReport
    {
        public static async Task<ReportConfiguration> GetReportConfiguration()
        {
            await Task.Run(() => { });
            //Erzeugen und Befüllen des DataSets
            ProtocolAdapter adapter = ApplicationService.GetAdapter(nameof(ProtocolAdapter)) as ProtocolAdapter;
            long OrderId;
            
            if (adapter.SelectedOrder != null) 
                OrderId = adapter.SelectedOrder.Id;
            else 
                OrderId = -1;

            DataTable Order = (new LocalDBAdapter("SELECT * " +
                                                  "FROM Orders " +
                                                  "WHERE Id=" + OrderId)).DB_Output();
            DataTable Recipes_MR = (new LocalDBAdapter("SELECT * " +
                                                       "FROM Recipes_MR " +
                                                       "WHERE Id=" + Order.Rows[0]["MR_Id"])).DB_Output();
            DataTable C1 = (new LocalDBAdapter("SELECT * " +
                                               "FROM Recipes_Coating " +
                                               "WHERE Id=" + Recipes_MR.Rows[0]["C1_Id"])).DB_Output();
            DataTable C2 = (new LocalDBAdapter("SELECT * " +
                                               "FROM Recipes_Coating " +
                                               "WHERE Id=" + Recipes_MR.Rows[0]["C2_Id"])).DB_Output();
            DataTable C3 = (new LocalDBAdapter("SELECT * " +
                                               "FROM Recipes_Coating " +
                                               "WHERE Id=" + Recipes_MR.Rows[0]["C3_Id"])).DB_Output();
            DataTable C4 = (new LocalDBAdapter("SELECT * " +
                                               "FROM Recipes_Coating " +
                                               "WHERE Id=" + Recipes_MR.Rows[0]["C4_Id"])).DB_Output();
            DataTable Charges = (new LocalDBAdapter("SELECT * " +
                                                    "FROM Charges " +
                                                    "WHERE Order_Id=" + OrderId)).DB_Output();


            var config = new ReportConfiguration
            {
                ReportPath = @"Reporting\Reports\Protocol\Charges\ChargesReport.rdlc",
                DefaultExportPath = @"Reports\Protocol\Charges\ChargesReport",
                FileNamePrefix = "ChargesReport"
            };
            //DataSet in die Konfiguration einfügen

            config.DataSources.Add(new ReportDataSource("Order", Order));
            config.DataSources.Add(new ReportDataSource("Recipes_MR", Recipes_MR));
            config.DataSources.Add(new ReportDataSource("C1", C1));
            config.DataSources.Add(new ReportDataSource("C2", C2));
            config.DataSources.Add(new ReportDataSource("C3", C3));
            config.DataSources.Add(new ReportDataSource("C4", C4));
            config.DataSources.Add(new ReportDataSource("Charges", Charges));

            //Lokalisierbare ReportParameter in die Konfiguration einfügen
            foreach (var paraInfo in Parameters.LocalizableParameter)
            {
                config.ReportParameters.Add(new ReportParameter(paraInfo.Name, ApplicationService.GetText(paraInfo.LocaliableString)));
            }

            return config;
        }

        private class Parameters
        {
            public static readonly IEnumerable<ParameterInfo> LocalizableParameter = new Collection<ParameterInfo>
            {
                new ParameterInfo("OrderLabel", "@Protocol.Text1"),
                new ParameterInfo("BatchLabel", "@Protocol.Text2"),
                new ParameterInfo("ItemLabel", "@Protocol.Text3"),
                new ParameterInfo("MRLabel", "@Protocol.Text4"),
                new ParameterInfo("UserLabel", "@Protocol.Text5"),
                new ParameterInfo("WeightLabel", "@Protocol.Text13"),
                new ParameterInfo("C1Label", "@Protocol.Text18"),
                new ParameterInfo("C2Label", "@Protocol.Text19"),
                new ParameterInfo("C3Label", "@Protocol.Text20"),
                new ParameterInfo("C4Label", "@Protocol.Text21"),
                new ParameterInfo("BoxLabel", "@Protocol.Text36"),
                new ParameterInfo("ChargeLabel", "@Protocol.Text12"),
                new ParameterInfo("OptimizedLabel", "@Protocol.Text14"), 
                new ParameterInfo("RunsLabel", "@Protocol.Text15"), 
                new ParameterInfo("StartLabel", "@Protocol.Text7"),
                new ParameterInfo("EndLabel", "@Protocol.Text8")
            };
        }
    }
}