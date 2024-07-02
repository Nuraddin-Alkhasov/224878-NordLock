using System;
using System.Windows.Input;
using VisiWin.ApplicationFramework;
using VisiWin.Commands;
using System.ComponentModel.Composition;
using VisiWin.Alarm;
using System.Collections.ObjectModel;
using HMI.Views.DialogRegion;
using HMI.Views.MessageBoxRegion;
using System.Collections.Generic;

namespace HMI.Diagnose
{
    [ExportAdapter("HistoricalAlarmAdapter")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class HistoricalAlarmAdapter : AdapterBase
    {
        #region Private Variablen

        private IAlarmService alarmService;
        private IHistoricalAlarmFilter alarmFilter;
        private Collection<IHistoricalAlarmItem> historicalAlarms;
        private bool isRequestingHistoricalAlarms;
        private bool canRequestHistoricalAlarms;
		private IHistoricalAlarmRequest alarmRequest;

        #endregion

        #region Konstruktor

        public HistoricalAlarmAdapter()
        {
            if (ApplicationService.IsInDesignMode)
                return;

            this.alarmService = this.GetService<IAlarmService>();
            this.alarmFilter = this.alarmService.GetHistoricalAlarmFilter();
            this.ShowRequestHistoricalAlarmsDialogCommand = new ActionCommand(ShowRequestHistoricalAlarmsDialog);
        
            CanRequestHistoricalAlarms = true;
        }

        #endregion

        #region Bindungsmöglichkeiten

        /// <summary>
        /// Auflistung der ermittelten historischen Alarme.
        /// </summary>
        public Collection<IHistoricalAlarmItem> HistoricalAlarms
        {
            get 
            { 
                return this.historicalAlarms; 
            }
            private set
            {
                if (this.historicalAlarms != value)
                {
                    this.historicalAlarms = value;
                    this.OnPropertyChanged("HistoricalAlarms");
                }
            }
        }

        /// <summary>
        /// Filtereinstellungen, die beim Ermitteln der historischen Alarme angewendet werden.
        /// </summary>
        public IHistoricalAlarmFilter AlarmFilter
        {
            get
            {
                return this.alarmFilter;
            }
            set
            {
                if (this.alarmFilter != value)
                {
                    this.alarmFilter = value;
                    this.OnPropertyChanged("AlarmFilter");
                }
            }
        }

        /// <summary>
        /// Zeigt an, dass im Moment eine Abfrage aktiv ist.
        /// Falls diese Eigenschaft den Wert true enthält, können keine weiteren Abfragen gestartet werden.
        /// </summary>
        public bool IsRequestingHistoricalAlarms
        {
            get { return isRequestingHistoricalAlarms; }
            set 
            {
                if (value != isRequestingHistoricalAlarms)
                {
                    this.isRequestingHistoricalAlarms = value;
                    this.OnPropertyChanged("IsRequestingHistoricalAlarms");
                    this.CanRequestHistoricalAlarms = !isRequestingHistoricalAlarms;
                }
            }
        }

        /// <summary>
        /// Zeigt an, ob eine neue Abfrage gestartet werden kann.
        /// </summary>
        public bool CanRequestHistoricalAlarms
        {
            get { return canRequestHistoricalAlarms; }
            set
            {
                if (value != this.canRequestHistoricalAlarms)
                {
                    this.canRequestHistoricalAlarms = value;
                    this.OnPropertyChanged("CanRequestHistoricalAlarms");
                }
            }
        }

        /// <summary>
        /// Gibt einen Command zurück, der einen Dialog mit Filtereinstellungen zur Abfrage der historischen Alarme anzeigt.
        /// </summary>
        public ICommand ShowRequestHistoricalAlarmsDialogCommand { get; set; }

        #endregion

        #region Befehle
        
        /// <summary>
        /// Zeigt einen Dialog mit Filtereinstellungen zur Abfrage der historischen Alarme an.
        /// Wenn der Dialog mit OK bestätigt wird, wird eine neue Abfrage gestartet.
        /// </summary>
        public void ShowRequestHistoricalAlarmsDialog(object parameter)
        {
            if ( DialogView.Show("Diagnose_AlarmsLogs_Filter", "@HistoricalAlarms.HistoricalAlarmFilterView.Caption") == DialogResult.OK)
            {
                HistoricalAlarmFilterAdapter filterAdapter = VisiWin.ApplicationFramework.ApplicationService.GetAdapter("HistoricalAlarmFilterAdapter") as HistoricalAlarmFilterAdapter;
                if (filterAdapter != null)
                    this.AlarmFilter = filterAdapter.GetHistoricalAlarmFilter();

                this.RequestHistoricalAlarms();
            }
        }

        /// <summary>
        /// Ermittelt die historischen Alarme, die den aktuellen Filtereinstellungen entsprechen
        /// </summary>
        public void RequestHistoricalAlarms()
        {            
			if (this.alarmRequest != null)
	            this.alarmRequest.GetHistoricalDataCompleted -= new EventHandler<GetHistoricalAlarmsCompletedEventArgs>(GetHistoricalDataCompleted);
            this.alarmRequest = this.alarmService.CreateHistoricalAlarmRequest(this.alarmFilter);
            this.alarmRequest.GetHistoricalDataCompleted += new EventHandler<GetHistoricalAlarmsCompletedEventArgs>(GetHistoricalDataCompleted);
			if (this.alarmRequest.GetHistoricalData())
				this.IsRequestingHistoricalAlarms = true;
			else
				new MessageBoxTask("@HistoricalAlarms.HistoricalAlarmFilterView.GetAlarmsError", "@HistoricalAlarms.HistoricalAlarmFilterView.Caption", MessageBoxIcon.Exclamation);
        }

        #endregion

        #region Hilfsfunktionen

        private void GetHistoricalDataCompleted(object sender, GetHistoricalAlarmsCompletedEventArgs e)
        {
            this.HistoricalAlarms = e.HistoricalAlarms;
            this.IsRequestingHistoricalAlarms = false;
        }

        #endregion
    }

}
