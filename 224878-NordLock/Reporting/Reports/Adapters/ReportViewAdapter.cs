using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using VisiWin.ApplicationFramework;
using VisiWin.Commands;

namespace HMI.Reporting
{
    [ExportAdapter("ReportViewAdapter")]
    public class ReportViewAdapter : AdapterBase
    {
        private string callingView;
        private readonly IRegionService regionService = ApplicationService.GetService<IRegionService>();

        private ReportConfiguration reportConfiguration;
        private string targetRegion;
        private CancellationTokenSource _cts;
        private readonly SemaphoreSlim _ctsLock;

        public ReportViewAdapter()
        {
            if (ApplicationService.IsInDesignMode)
            {
                return;
            }

            _cts = null;
            _ctsLock = new SemaphoreSlim(1, 1);

            this.CloseViewCommand = new ActionCommand(this.CloseViewCommandExecuted);
            this.ViewUnloaded += ReportViewAdapter_ViewUnloaded;
        }

        public ICommand CloseViewCommand { get; private set; }

        public ReportConfiguration ReportConfiguration
        {
            get { return this.reportConfiguration; }
            set
            {
                if (!Equals(value, this.reportConfiguration))
                {
                    this.reportConfiguration = value;
                    this.OnPropertyChanged("ReportConfiguration");
                }
            }
        }

        public async void OpenView(string targetRegion, Func<CancellationToken, Task<ReportConfiguration>> reportConfigurationTask)
        {
            this.callingView = this.regionService.GetCurrentViewName(this.targetRegion = targetRegion);

            var cts = new CancellationTokenSource();

            await _ctsLock.WaitAsync();
            try
            {
                this._cts?.Cancel();
                this._cts = cts;
            }
            finally
            {
                _ctsLock.Release();
            }

            var task = reportConfigurationTask(cts.Token);

            ApplicationService.SetView(targetRegion, "ReportView");

            ReportConfiguration r;
            try
            {
                r = await task;
            }
            catch (OperationCanceledException)
            {
                r = null;
            }

            await _ctsLock.WaitAsync();
            try
            {
                if (this._cts == cts)
                {
                    this._cts = null;
                }

                cts.Dispose();
            }
            finally
            {
                _ctsLock.Release();
            }

            if (r != null)
            {
                ReportConfiguration = r; 
            }
        }

        public void OpenView(string targetRegion, ReportConfiguration reportConfiguration)
        {
            this.OpenView(targetRegion, (t) => Task.FromResult(reportConfiguration));
        }

        private void CloseViewCommandExecuted(object parameter)
        {
            ApplicationService.SetView("DialogRegion", "EmptyView");
            this.targetRegion = null;
            this.callingView = null;
        }

        private async void ReportViewAdapter_ViewUnloaded(object sender, ViewUnloadedEventArg e)
        {
            this.ReportConfiguration = null;

            await _ctsLock.WaitAsync();
            try
            {
                _cts?.Cancel();
            }
            finally
            {
                _ctsLock.Release();
            }
        }
    }

    public class UnderlayNotifer : ContentUnderlayNotifer
    {
        private int autoHideCounter;
        private readonly List<string> toplevelRegions = new List<string>();

        public UnderlayNotifer()
        {
            var regionService = ApplicationService.GetService<IRegionService>();
            regionService.ViewChanged += this.RegionService_ViewChanged;
            this.toplevelRegions.Add("TouchpadRegion");
            this.toplevelRegions.Add("MessageBoxRegion");
            this.toplevelRegions.Add("DialogRegion");
        }

        private void RegionService_ViewChanged(object sender, ViewChangedEventArgs e)
        {
            var regionName = e.RegionName;
            var viewName = e.ViewName;

            if (this.toplevelRegions.Contains(regionName))
            {
                if (viewName != "EmptyView")
                {
                    if (autoHideCounter++ == 0)
                        this.OnContentIsCovered(true);
                    //this.autoHideCounter++;
                }
                else
                {
                    this.autoHideCounter--;
                    if (this.autoHideCounter == 0)
                    {
                        this.OnContentIsCovered(false);
                    }
                }
            }
        }
    }
}