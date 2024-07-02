using HMI.Services;
using System;
using System.Data;

namespace HMI.Views.MainRegion
{
    public class TrendExportResult : EventArgs
    {
        public TrendExportResult(Exception exception, bool cancelled)
        {
            this.Exception = exception;
            this.Cancelled = cancelled;
        }

        public TrendExportResult(bool cancelled, Informations informations)
        {
            this.Cancelled = cancelled;
            this.Informations = informations;
        }

        public TrendExportResult(DataTable dataTable, bool cancelled, Informations informations)
        {
            this.DataTable = dataTable;
            this.Cancelled = cancelled;
            this.Informations = informations;
        }

        public bool Cancelled { get; private set; }

        public DataTable DataTable { get; private set; }

        public Exception Exception { get; private set; }

        public Informations Informations { get; private set; }
    }

}
