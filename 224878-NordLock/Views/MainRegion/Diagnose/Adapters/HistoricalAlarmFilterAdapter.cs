using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VisiWin.ApplicationFramework;
using System.ComponentModel.Composition;
using System.Windows.Data;
using VisiWin.Alarm;
using VisiWin.Language;
using System.Linq;
using HMI.Views.MainRegion.Diagnose;

namespace HMI.Diagnose
{
    [ExportAdapter("HistoricalAlarmFilterAdapter")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class HistoricalAlarmFilterAdapter : AdapterBase
    {
        #region Private Variablen

        private IAlarmService alarmService;
        private ObservableCollection<HistoricalTimeSpanFilterItem> timeSpanFilterTypes;
        private int selectedTimeSpanFilterTypeIndex;
        private bool customTimeSpanFilterEnabled;
        private DateTime maxTime;
        private DateTime minTime;
        private ListCollectionView desiredGroups;
        private ListCollectionView desiredClasses;
        private ObservableCollection<AlarmStateFilterItem> desiredStates;
        private int minPriority;
        private int maxPriority;
        private bool priorityFilterEnabled;

        #endregion

        #region Konstruktor

        public HistoricalAlarmFilterAdapter()
        {
            timeSpanFilterTypes = new ObservableCollection<HistoricalTimeSpanFilterItem>();
            desiredStates = new ObservableCollection<AlarmStateFilterItem>();

            if (ApplicationService.IsInDesignMode)
                return;

            this.alarmService = this.GetService<IAlarmService>();

            // TimeSpan Filtertypen auflisten
            timeSpanFilterTypes.Add(new HistoricalTimeSpanFilterItem(HistoricalTimeSpanFilterType.Today));
            timeSpanFilterTypes.Add(new HistoricalTimeSpanFilterItem(HistoricalTimeSpanFilterType.Yesterday));
            timeSpanFilterTypes.Add(new HistoricalTimeSpanFilterItem(HistoricalTimeSpanFilterType.ThisWeek));
            timeSpanFilterTypes.Add(new HistoricalTimeSpanFilterItem(HistoricalTimeSpanFilterType.LastWeek));
            timeSpanFilterTypes.Add(new HistoricalTimeSpanFilterItem(HistoricalTimeSpanFilterType.Custom));

            // Min- und MaxTime initialisieren
            SetTimeSpan(HistoricalTimeSpanFilterType.Today);

            // Alarmgruppen auflisten
            List<AlarmGroupsFilterItem> alarmGroups = new List<AlarmGroupsFilterItem>();
            IAlarmGroup alarmGroup;
            foreach (string alarmGroupName in this.alarmService.GroupNames)
            {
                if (alarmGroupName !=   "Systemalarm")
                {
                    alarmGroup = alarmService.GetAlarmGroup(alarmGroupName);

                    string serverPart = string.Empty;
                    int nServerPart = alarmGroup.GroupTree.IndexOf(':');
                    if (nServerPart >= 0)
                    {
                        serverPart = alarmGroup.GroupTree.Substring(0, nServerPart);
                    }

                    alarmGroups.Add(new AlarmGroupsFilterItem(alarmGroup.Name, alarmGroup.GroupTree, alarmGroup.LocalizableText, serverPart));
                }
               
            }

            this.desiredGroups = new ListCollectionView(alarmGroups);
            int serverCount = alarmGroups.Select(ag => ag.Server).Distinct().Count();
            if (serverCount > 1)
            {
                this.desiredGroups.GroupDescriptions.Add(new PropertyGroupDescription("Server"));
            }

            // Alarmklassen auflisten
            List<AlarmClassFilterItem> alarmClasses = new List<AlarmClassFilterItem>();
            IAlarmClass alarmClass;
            foreach (string alarmClassName in this.alarmService.ClassNames)
            {
                alarmClass = alarmService.GetAlarmClass(alarmClassName);

                string serverPart = string.Empty;
                int nServerPart = alarmClass.Name.IndexOf(':');
                if (nServerPart >= 0)
                {
                    serverPart = alarmClass.Name.Substring(0, nServerPart);
                }

                alarmClasses.Add(new AlarmClassFilterItem(alarmClass.Name, alarmClass.LocalizableText, serverPart));
            }

            this.desiredClasses = new ListCollectionView(alarmClasses);
            serverCount = alarmClasses.Select(ac => ac.Server).Distinct().Count();
            if (serverCount > 1)
            {
                this.desiredClasses.GroupDescriptions.Add(new PropertyGroupDescription("Server"));
            }

            // Alarmstati auflisten
            desiredStates.Add(new AlarmStateFilterItem(AlarmState.Active, "HistoricalAlarms.HistoricalAlarmFilterView.AlarmStates.Active"));
            desiredStates.Add(new AlarmStateFilterItem(AlarmState.ActiveAck, "HistoricalAlarms.HistoricalAlarmFilterView.AlarmStates.ActiveAck"));
            desiredStates.Add(new AlarmStateFilterItem(AlarmState.Inactive, "HistoricalAlarms.HistoricalAlarmFilterView.AlarmStates.Inactive"));
            desiredStates.Add(new AlarmStateFilterItem(AlarmState.InactiveAck, "HistoricalAlarms.HistoricalAlarmFilterView.AlarmStates.InactiveAck"));
            desiredStates.Add(new AlarmStateFilterItem(AlarmState.Cleared, "HistoricalAlarms.HistoricalAlarmFilterView.AlarmStates.Cleared"));

            // Prioritätenfilter initialisieren
            MinPriority = -1;
            MaxPriority = -1;
        }

        #endregion

        #region Überschreibungen

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == "SelectedTimeSpanFilterTypeIndex")
            {
                if (SelectedTimeSpanFilterTypeIndex >= 0)
                {
                    HistoricalTimeSpanFilterType selectedFilterType = TimeSpanFilterTypes[SelectedTimeSpanFilterTypeIndex].FilterType;
                    CustomTimeSpanFilterEnabled = selectedFilterType == HistoricalTimeSpanFilterType.Custom;
                    SetTimeSpan(selectedFilterType);
                }
            }
            else if (propertyName == "PriorityFilterEnabled")
            {
                if (!priorityFilterEnabled)
                {
                    MinPriority = -1;
                    MaxPriority = -1;
                }
            }
        }

        #endregion

        #region Bindungsmöglichkeiten

        /// <summary>
        /// Auflistung der unterstützten Zeitraumfiltertypen.
        /// </summary>
        public ObservableCollection<HistoricalTimeSpanFilterItem> TimeSpanFilterTypes
        {
            get
            {
                return this.timeSpanFilterTypes;
            }
        }

        /// <summary>
        /// Index des selektierten Zeitraumfiltertyps.
        /// </summary>
        public int SelectedTimeSpanFilterTypeIndex
        {
            get { return selectedTimeSpanFilterTypeIndex; }
            set
            {
                if (value != selectedTimeSpanFilterTypeIndex)
                {
                    selectedTimeSpanFilterTypeIndex = value;
                    OnPropertyChanged("SelectedTimeSpanFilterTypeIndex");
                }
            }
        }

        /// <summary>
        /// Zeigt an, ob der Zeitraumfiltertyp "Benutzerdefiniert" ausgewählt wurde.
        /// </summary>
        public bool CustomTimeSpanFilterEnabled
        {
            get { return customTimeSpanFilterEnabled; }
            set
            {
                if (value != customTimeSpanFilterEnabled)
                {
                    customTimeSpanFilterEnabled = value;
                    OnPropertyChanged("CustomTimeSpanFilterEnabled");
                }
            }
        }

        /// <summary>
        /// Zeitstempel des jüngsten Alarms, der bei der Abfrage berücksichtigt werden soll.
        /// </summary>
        public DateTime MaxTime
        {
            get { return maxTime; }
            set
            {
                if (value != maxTime)
                {
                    maxTime = value;
                    OnPropertyChanged("MaxTime");
                }
            }
        }

        /// <summary>
        /// Zeitstempel des ältesten Alarms, der bei der Abfrage berücksichtigt werden soll.
        /// </summary>
        public DateTime MinTime
        {
            get { return minTime; }
            set
            {
                if (value != minTime)
                {
                    minTime = value;
                    OnPropertyChanged("MinTime");
                }
            }
        }

        /// <summary>
        /// Auflistung der definierten Alarmgruppen.
        /// Wenn die Eigenschaft IsSelected eines AlarmGroupsFilterItems den Wert "true" enthält, 
        /// wird die entsprechende Alarmgruppe bei der Abfrage berücksichtigt.
        /// </summary>
        public ListCollectionView DesiredGroups
        {
            get
            {
                return this.desiredGroups;
            }
        }

        /// <summary>
        /// Auflistung der definierten Alarmklassen.
        /// Wenn die Eigenschaft IsSelected eines AlarmClassFilterItems den Wert "true" enthält, 
        /// wird die entsprechende Alarmklasse bei der Abfrage berücksichtigt.
        /// </summary>
        public ListCollectionView DesiredClasses
        {
            get
            {
                return this.desiredClasses;
            }
        }

        /// <summary>
        /// Auflistung der möglichen Alarmzustände.
        /// Wenn die Eigenschaft IsSelected eines AlarmStateFilterItem den Wert "true" enthält, 
        /// wird der entsprechende Zustand bei der Abfrage berücksichtigt.
        /// </summary>
        public ObservableCollection<AlarmStateFilterItem> DesiredStates
        {
            get
            {
                return this.desiredStates;
            }
        }

        /// <summary>
        /// Minimale Priorität, die bei der Abfrage berücksichtigt werden soll.
        /// </summary>
        public int MinPriority
        {
            get { return minPriority; }
            set
            {
                if (value != minPriority)
                {
                    minPriority = value;
                    OnPropertyChanged("MinPriority");
                }
            }
        }

        /// <summary>
        /// Maximale Priorität, die bei der Abfrage berücksichtigt werden soll.
        /// </summary>
        public int MaxPriority
        {
            get { return maxPriority; }
            set
            {
                if (value != maxPriority)
                {
                    maxPriority = value;
                    OnPropertyChanged("MaxPriority");
                }
            }
        }

        /// <summary>
        /// Zeigt an, ob der Prioritätsfilter aktiviert wurde.
        /// </summary>
        public bool PriorityFilterEnabled
        {
            get { return priorityFilterEnabled; }
            set
            {
                if (value != priorityFilterEnabled)
                {
                    priorityFilterEnabled = value;
                    OnPropertyChanged("PriorityFilterEnabled");
                }
            }
        }

        #endregion

        #region Methoden

        /// <summary>
        /// Gibt eine IHistoricalAlarmFilter Instanz zurück, die die ausgewählten Filtereinstellungen enthält.
        /// </summary>
        /// <returns></returns>
        public IHistoricalAlarmFilter GetHistoricalAlarmFilter()
        {
            IHistoricalAlarmFilter historicalAlarmFilter = alarmService.CreateHistoricalAlarmFilter();
            historicalAlarmFilter.MinTime = MinTime;
            historicalAlarmFilter.MaxTime = MaxTime;

            historicalAlarmFilter.DesiredGroups = "";
            foreach (AlarmGroupsFilterItem groupItem in this.DesiredGroups)
            {
                if (groupItem.IsSelected)
                    historicalAlarmFilter.DesiredGroups += groupItem.GroupTree + ";";
            }

            historicalAlarmFilter.DesiredClasses = "";
            foreach (AlarmClassFilterItem classItem in this.DesiredClasses)
            {
                if (classItem.IsSelected)
                    historicalAlarmFilter.DesiredClasses += classItem.ClassName + ";";
            }

            historicalAlarmFilter.DesiredStates = AlarmState.None;
            foreach (AlarmStateFilterItem stateItem in this.DesiredStates)
            {
                if (stateItem.IsSelected)
                    historicalAlarmFilter.DesiredStates |= stateItem.AlarmState;
            }

            historicalAlarmFilter.MinPriority = MinPriority;
            historicalAlarmFilter.MaxPriority = MaxPriority;

            return historicalAlarmFilter;
        }

        #endregion

        #region Hilfsfunktionen

        public void SetTimeSpan(HistoricalTimeSpanFilterType selectedTimeSpanFilterType)
        {
            switch (selectedTimeSpanFilterType)
            {
                case HistoricalTimeSpanFilterType.Custom:
                    break;
                case HistoricalTimeSpanFilterType.Today:
                    MinTime = DateTime.Now.Date;
                    MaxTime = MinTime.Add(TimeSpan.FromDays(1));
                    break;
                case HistoricalTimeSpanFilterType.Yesterday:
                    MinTime = DateTime.Now.Date.Add(TimeSpan.FromDays(-1.0));
                    MaxTime = MinTime.Add(TimeSpan.FromDays(1));
                    break;
                case HistoricalTimeSpanFilterType.ThisWeek:
                    MinTime = DateTime.Now.Date.Add(TimeSpan.FromDays(-(double)DateTime.Now.Date.DayOfWeek));
                    MaxTime = MinTime.Add(TimeSpan.FromDays(7));
                    break;
                case HistoricalTimeSpanFilterType.LastWeek:
                    MinTime = DateTime.Now.Date.Add(TimeSpan.FromDays(-(double)DateTime.Now.Date.DayOfWeek - 7));
                    MaxTime = MinTime.Add(TimeSpan.FromDays(7));
                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}