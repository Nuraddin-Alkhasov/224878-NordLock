using System.Windows.Input;
using VisiWin.ApplicationFramework;
using VisiWin.Commands;
using System.ComponentModel.Composition;
using VisiWin.Logging;
using System.Collections.Generic;
using HMI.Views.MessageBoxRegion;
using System.Collections.ObjectModel;
using VisiWin.UserManagement;

namespace HMI
{
    [ExportAdapter("LoggingAdapter")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class LoggingAdapter : AdapterBase
    {
        #region Private Variablen

        private ILoggingService loggingService;
        private IHistoricalLoggingFilter loggingFilter;
        private IEnumerable<IHistoricalLoggingEntry> loggingEntries;
        private bool isRequestingLoggingEntries;
        private bool canRequestLoggingEntries;
        
        #endregion

        #region Konstruktor

        public LoggingAdapter()
        {
            if (ApplicationService.IsInDesignMode)
                return;

            this.loggingService = this.GetService<ILoggingService>();
            this.loggingFilter = this.loggingService.CreateHistoricalLoggingFilter();
            this.ShowRequestLoggingDialogCommand = new ActionCommand(ShowRequestLoggingDialog);
           
            loggingService.GetHistoricalLoggingEntriesAsyncCompleted += loggingService_GetLoggingEntriesAsyncCompleted;
            
            CanRequestLoggingEntries = true;
           
        }

        private void loggingService_GetLoggingEntriesAsyncCompleted(object sender, GetHistoricalLoggingEntriesCompletedEventArgs e)
        {
            this.LoggingEntries = e.HistoricalLoggingEntries;
            this.IsRequestingLoggingEntries = false;

            if (e.Success == HistoricalLoggingEntriesSuccess.Failed) {
                new MessageBoxTask("@Logging.LoggingFilterView.GetLogEntriesError", "@Logging.LoggingFilterView.Caption", MessageBoxIcon.Exclamation);
            }
        }

        #endregion

        #region Bindungsmöglichkeiten

        /// <summary>
        /// Auflistung der ermittelten historischen Alarme.
        /// </summary>
        public IEnumerable<IHistoricalLoggingEntry> LoggingEntries
        {
            get 
            { 
                return this.loggingEntries; 
            }
            private set
            {
                if (this.loggingEntries != value)
                {
                    this.loggingEntries = value;
                    this.OnPropertyChanged("LoggingEntries");
                }
            }
        }

     

        /// <summary>
        /// Filtereinstellungen, die beim Ermitteln der historischen Logbucheinträge angewendet werden.
        /// </summary>
        public IHistoricalLoggingFilter LoggingFilter
        {
            get
            {
                return this.loggingFilter;
            }
            set
            {
                if (this.loggingFilter != value)
                {
                    this.loggingFilter = value;
                    this.OnPropertyChanged("LoggingFilter");
                }
            }
        }

        /// <summary>
        /// Zeigt an, dass im Moment eine Abfrage aktiv ist.
        /// Falls diese Eigenschaft den Wert true enthält, können keine weiteren Abfragen gestartet werden.
        /// </summary>
        public bool IsRequestingLoggingEntries
        {
            get { return isRequestingLoggingEntries; }
            set 
            {
                if (value != isRequestingLoggingEntries)
                {
                    this.isRequestingLoggingEntries = value;
                    this.OnPropertyChanged("IsRequestingLoggingEntries");
                    this.CanRequestLoggingEntries = !isRequestingLoggingEntries;
                }
            }
        }

        /// <summary>
        /// Zeigt an, ob eine neue Abfrage gestartet werden kann.
        /// </summary>
        public bool CanRequestLoggingEntries
        {
            get { return canRequestLoggingEntries; }
            set
            {
                if (value != this.canRequestLoggingEntries)
                {
                    this.canRequestLoggingEntries = value;
                    this.OnPropertyChanged("CanRequestLoggingEntries");
                }
            }
        }

        /// <summary>
        /// Gibt einen Command zurück, der einen Dialog mit Filtereinstellungen zur Abfrage der historischen Alarme anzeigt.
        /// </summary>
        public ICommand ShowRequestLoggingDialogCommand { get; set; }

        #endregion

        #region Befehle
        
        /// <summary>
        /// Zeigt einen Dialog mit Filtereinstellungen zur Abfrage der historischen Alarme an.
        /// Wenn der Dialog mit OK bestätigt wird, wird eine neue Abfrage gestartet.
        /// </summary>
        public void ShowRequestLoggingDialog(object parameter)
        {
			//	erst Filter dann Daten
			LoggingFilterAdapter filterAdapter = VisiWin.ApplicationFramework.ApplicationService.GetAdapter("LoggingFilterAdapter") as LoggingFilterAdapter;
			if (filterAdapter != null) {
				this.loggingFilter = filterAdapter.CreateFilter();
				if (this.loggingFilter == null)
					return;
			}

			//	Daten ermitteln
            this.RequestLoggingEntries();
        }

        /// <summary>
        /// Ermittelt die historischen Alarme, die den aktuellen Filtereinstellungen entsprechen
        /// </summary>
        public void RequestLoggingEntries()
        {
			if (loggingService != null) {
				loggingService.GetHistoricalLoggingEntriesAsync(this.loggingFilter);
				this.IsRequestingLoggingEntries = true;
			} else {
				new MessageBoxTask("@Logging.LoggingFilterView.GetLogEntriesError", "@Logging.LoggingFilterView.Caption", MessageBoxIcon.Exclamation);
			}
        }



        #endregion
    }

}