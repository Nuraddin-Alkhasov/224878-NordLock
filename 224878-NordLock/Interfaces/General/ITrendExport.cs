using HMI.Views.MainRegion;
using System;
using System.ComponentModel;

namespace HMI.Interfaces
{
    public interface ITrendExport
    {

        event EventHandler<TrendExportResult> TrendExportCompleted;
        event ProgressChangedEventHandler TrendExportProgressChanged;
        void CancelExport();
        void ExportTrendData(TrendExportSettings settings, TrendExportOptions options = null);
    }
}
