//*****************************************************************************
//
//	FUNKTION:
//	Adapter zum Filtern von historischen Logbucheinträgen
//
//
//	Befehle 
//	----------------------------------------------------------------------------
//
//
//	Bindungsmöglichkeiten
//	-----------------------------------------------------------------------------
//	TimeSpanFilterTypes						--> Auflistung der unterstützten Zeitraumfiltertypen
//	SelectedTimeSpanFilterTypeIndex			--> Index des selektierten Zeitraumfiltertyps
//	CustumTimeSpanFilterEnabled				--> zeigt an, ob der Zeitraumfiltertyp "Benutzerdefiniert" ausgewählt wurde
//	MinTime									--> Abfragezeitraum ab diesem Zeitpunkt
//	MaxTime									--> Abfragezeitraum bis zu diesem Zeitpunkt
//	DesiredCategories						--> Auflistung der definierten Kategorien
//	DesiredEvents							--> Auflistung der definierten Ereignisse
//	DesiredUsers							--> Auflistung der definierten Benutzer
//	SelectedCategory						--> die angewählte Kategorie, hierzu werden jeweils die passenden Ererignisse aufgelistet
//	IsAllCategoriesSelected					--> alle Kategorien sind gewählt, die Kategorieauswahlliste braucht man nicht
//	IsAllEventsSelected						--> alle Ereignisse sind gewählt, die Ereignisauswahlliste braucht man nicht
//	IsAllUsersSelected						--> alle Benutzer sind gewählt, die Benutzerauswahlliste braucht man nicht
//	IsCategorySelectionEnabled				--> Kategorieauswahlliste anzeigen oder nicht
//	IsEventSelectionEnabled					--> Ereignisauswahlliste anzeigen oder nicht
//	IsUserSelectionEnabled					--> Kategorieauswahlliste anzeigen oder nicht
//
//
//	Funktionen
//	-----------------------------------------------------------------------------
//	RefreshUsers							--> füllt die Benutzerliste neu; muss aufgerufen werden, wenn sich der Benutzerstamm ändert
//
//
//	notwendige Dateien: 
//	-----------------------------------------------------------------------------
//	LoggingFilterAdapter.cs					--> dieser Adapter
//	LoggingFilterView.xaml(.cs)				-->	Anzeige des Loggingfilters
//	DialogAdapter.cs						-->	Adapter für die MessageBox- und Dialogfunktionen
//	MessageBoxView.xaml(.cs)				--> MessageBox mit statischem Aufruf
//	DialogView.xaml(.cs)					--> Basis-Dialog mit statischem Aufruf
//
//*****************************************************************************

using System;
using System.Collections.ObjectModel;
using VisiWin.ApplicationFramework;
using System.ComponentModel.Composition;
using VisiWin.Language;
using VisiWin.Logging;
using System.Collections.Generic;
using VisiWin.UserManagement;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using HMI.Views.DialogRegion;
using HMI.Views.MessageBoxRegion;

namespace HMI
{
	[ExportAdapter("LoggingFilterAdapter")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class LoggingFilterAdapter : AdapterBase
    {
        #region Private Variablen

        private ILoggingService loggingService;
		private ILanguageService languageService;
		private ObservableCollection<LoggingTimeSpanFilterItem> timeSpanFilterTypes;
        private int selectedTimeSpanFilterTypeIndex;
        private bool customTimeSpanFilterEnabled;
		private bool isAllCategoriesSelected;
		private bool isAllEventsSelected;
		private bool isAllUsersSelected;
		private bool isCategorySelectionEnabled;
		private bool isEventSelectionEnabled;
		private bool isUserSelectionEnabled;
		private DateTime maxTime;
        private DateTime minTime;
        private ListCollectionView desiredCategories;
        private ObservableCollection<EventFilterItem> desiredEvents;
		private ObservableCollection<UserFilterItem> desiredUsers;
		private IHistoricalLoggingFilter filter;

        #endregion

        #region Konstruktor

		public LoggingFilterAdapter()
        {
			this.timeSpanFilterTypes = new ObservableCollection<LoggingTimeSpanFilterItem>();
            this.desiredEvents = new ObservableCollection<EventFilterItem>();
			this.desiredUsers = new ObservableCollection<UserFilterItem>();

            if (ApplicationService.IsInDesignMode)
                return;

            this.loggingService = this.GetService<ILoggingService>();

            // TimeSpan Filtertypen auflisten
			timeSpanFilterTypes.Add(new LoggingTimeSpanFilterItem(LoggingTimeSpanFilterType.Today));
			timeSpanFilterTypes.Add(new LoggingTimeSpanFilterItem(LoggingTimeSpanFilterType.Yesterday));
			timeSpanFilterTypes.Add(new LoggingTimeSpanFilterItem(LoggingTimeSpanFilterType.ThisWeek));
			timeSpanFilterTypes.Add(new LoggingTimeSpanFilterItem(LoggingTimeSpanFilterType.LastWeek));
			timeSpanFilterTypes.Add(new LoggingTimeSpanFilterItem(LoggingTimeSpanFilterType.Custom));

            // Min- und MaxTime initialisieren
			SetTimeSpan(LoggingTimeSpanFilterType.Today);

            // Kategorien auflisten
			bool bFirstCategory = true;
			IEnumerable<ILoggingCategory> categories = this.loggingService.Categories;
		    List<CategoryFilterItem> catFilterItems = new List<CategoryFilterItem>();
			foreach (ILoggingCategory cat in categories)
			{
                string serverPart = string.Empty;
                int nServerPart = cat.Name.IndexOf(':');
                if (nServerPart >= 0)
                {
                    serverPart = cat.Name.Substring(0, nServerPart);
                }
			    
				//	die zugehörigen Ereignisse merken
                ObservableCollection<EventFilterItem> catEvents = new ObservableCollection<EventFilterItem>();
                IEnumerable<ILoggingEvent> events = this.loggingService.GetEvents(cat.Name);
                foreach (ILoggingEvent evt in events)
                {
                    string eventNameWithServer = evt.Name;
                    if (!string.IsNullOrEmpty(serverPart))
                    {
                        eventNameWithServer = serverPart + ":" + evt.Name;
                    }

                    catEvents.Add(new EventFilterItem(eventNameWithServer, evt.FullName, evt.LocalizedName));
                }

                catFilterItems.Add(new CategoryFilterItem(this, cat.Name, cat.LocalizedName, catEvents, serverPart));

				//	für die erste Kategorie die Ereignisse anzeigen
				if (bFirstCategory) {
                    this.DesiredEvents = catEvents;
					bFirstCategory = false;
				}
            }

		    this.desiredCategories = new ListCollectionView(catFilterItems);

            int serverCount = catFilterItems.Select(cat => cat.Server).Distinct().Count();
            if (serverCount > 1)
            {
                this.desiredCategories.GroupDescriptions.Add(new PropertyGroupDescription("Server"));
            }

			//	an der Sprachumschaltung anmelden
			this.languageService = this.GetService<ILanguageService>();
			if (this.languageService != null) {
				this.languageService.LanguageChanged += languageService_LanguageChanged;
			}

			//	Benutzer auflisten
			RefreshUsers();

			//	alle an
			IsAllEventsSelected = true;
			IsAllCategoriesSelected = true;
			IsAllUsersSelected = true;
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
					LoggingTimeSpanFilterType selectedFilterType = TimeSpanFilterTypes[SelectedTimeSpanFilterTypeIndex].FilterType;
					CustomTimeSpanFilterEnabled = selectedFilterType == LoggingTimeSpanFilterType.Custom;
                    SetTimeSpan(selectedFilterType);
                }
            }
        }

        #endregion

        #region Bindungsmöglichkeiten

        /// <summary>
        /// Auflistung der unterstützten Zeitraumfiltertypen.
        /// </summary>
		public ObservableCollection<LoggingTimeSpanFilterItem> TimeSpanFilterTypes
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

		private CategoryFilterItem selectedCategory = null;
		/// <summary>
		/// Zeitstempel des ältesten Alarms, der bei der Abfrage berücksichtigt werden soll.
		/// </summary>
		public CategoryFilterItem SelectedCategory
		{
			get { return this.selectedCategory; }
			set
			{
				if (value != this.selectedCategory) {
					this.selectedCategory = value;
					OnPropertyChanged("SelectedCategory");

					// Ereignisse auflisten
					this.DesiredEvents = this.selectedCategory.DesiredEvents;
				}
			}
		}

        /// <summary>
        /// alle Kategorien sollen ermittelt werden
        /// </summary>
		public bool IsAllCategoriesSelected
        {
			get { return this.isAllCategoriesSelected; }
            set
            {
				if (value != this.isAllCategoriesSelected)
                {
					this.isAllCategoriesSelected = value;
					OnPropertyChanged("IsAllCategoriesSelected");

					if (this.isAllCategoriesSelected) {
						IsCategorySelectionEnabled = false;
						IsEventSelectionEnabled = false;
					} else {
						IsCategorySelectionEnabled = true;
						IsEventSelectionEnabled = !this.isAllEventsSelected;
					}
                }
            }
        }

		/// <summary>
		/// alle Ereignisse sollen ermittelt werden
		/// </summary>
		public bool IsAllEventsSelected
		{
			get { return this.isAllEventsSelected; }
			set
			{
				if (value != this.isAllEventsSelected) {
					this.isAllEventsSelected = value;
					OnPropertyChanged("IsAllEventsSelected");

					IsEventSelectionEnabled = !this.isAllEventsSelected;
				}
			}
		}

		/// <summary>
		/// alle Benutzer sollen berücksichtigt werden
		/// </summary>
		public bool IsAllUsersSelected
		{
			get { return this.isAllUsersSelected; }
			set
			{
				if (value != this.isAllUsersSelected) {
					this.isAllUsersSelected = value;
					OnPropertyChanged("IsAllUsersSelected");

					IsUserSelectionEnabled = !this.isAllUsersSelected;
				}
			}
		}

		/// <summary>
		/// die Kategorieauswahl ist freigeschaltet
		/// </summary>
		public bool IsCategorySelectionEnabled
		{
			get { return this.isCategorySelectionEnabled; }
			set
			{
				if (value != this.isCategorySelectionEnabled) {
					this.isCategorySelectionEnabled = value;
					OnPropertyChanged("IsCategorySelectionEnabled");
				}
			}
		}

		/// <summary>
		/// die Ereignisauswahl ist freigeschaltet
		/// </summary>
		public bool IsEventSelectionEnabled
		{
			get { return this.isEventSelectionEnabled; }
			set
			{
				if (value != this.isEventSelectionEnabled) {
					this.isEventSelectionEnabled = value;
					OnPropertyChanged("IsEventSelectionEnabled");
				}
			}
		}

		/// <summary>
		/// die Benutzerauswahl ist freigeschaltet
		/// </summary>
		public bool IsUserSelectionEnabled
		{
			get { return this.isUserSelectionEnabled; }
			set
			{
				if (value != this.isUserSelectionEnabled) {
					this.isUserSelectionEnabled = value;
					OnPropertyChanged("IsUserSelectionEnabled");
				}
			}
		}

		/// <summary>
        /// Auflistung der definierten Kategorien.
        /// Wenn die Eigenschaft IsSelected eines CategoryFilterItems den Wert "true" enthält, 
        /// wird die entsprechende Kategorie bei der Abfrage berücksichtigt.
        /// </summary>
        public ListCollectionView DesiredCategories
        {
            get
            {
                return this.desiredCategories;
            }
			set
			{
				if (value != this.desiredCategories) {
					this.desiredCategories = value;
					OnPropertyChanged("DesiredCategories");
				}
			}
		}

        /// <summary>
        /// Auflistung der definierten Ereignissen.
        /// Wenn die Eigenschaft IsSelected eines EventFilterItems den Wert "true" enthält, 
        /// wird das entsprechende Ereignis bei der Abfrage berücksichtigt.
        /// </summary>
        public ObservableCollection<EventFilterItem> DesiredEvents
        {
            get
            {
                return this.desiredEvents;
            }
			set
			{
				if (value != this.desiredEvents) {
					this.desiredEvents = value;
					OnPropertyChanged("DesiredEvents");
				}
			}
		}

		/// <summary>
		/// Auflistung der definierten Benutzer.
		/// Wenn die Eigenschaft IsSelected eines UserFilterItems den Wert "true" enthält, 
		/// wird der entsprechende Benutzer bei der Abfrage berücksichtigt.
		/// </summary>
		public ObservableCollection<UserFilterItem> DesiredUsers
		{
			get
			{
				return this.desiredUsers;
			}
			set
			{
				if (value != this.desiredUsers) {
					this.desiredUsers = value;
					OnPropertyChanged("DesiredUsers");
				}
			}
		}

		#endregion

        #region Methoden

		/// <summary>
		/// die Sprache hat sich geändert, die Kategorie- und Ereignistexte müssen angepasst werden
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void languageService_LanguageChanged(object sender, LanguageChangedEventArgs e)
		{
			foreach (CategoryFilterItem cat in this.DesiredCategories) {
				ILoggingCategory lC = this.loggingService.GetCategory(cat.Name);
				if (lC != null)
					cat.LocalizedText = lC.LocalizedName;

				foreach (EventFilterItem evt in cat.DesiredEvents) {
					ILoggingEvent lE = this.loggingService.GetEvent(cat.Name, evt.Name);
					if (lE != null)
						evt.LocalizedText = lE.LocalizedName;
				}
			}
		}

		/// <summary>
		/// falls die Benutzer auch in dieser Anwendung verwaltet werden, dann muss diese Funktion benutzt werden,
		/// wenn Benutzer hinzugekommen oder gelöscht worden sind
		/// </summary>
		void RefreshUsers()
		{
			desiredUsers.Clear();

			//	Benutzer auflisten
			IUserManagementService uMS = ApplicationService.GetService<IUserManagementService>();
			if (uMS != null) {
				foreach (string user in uMS.UserNames) {
					IUserDefinition uD = uMS.GetUserDefinition(user);
					if (uD != null)
						desiredUsers.Add(new UserFilterItem(user, uD.FullName));
				}
			}
		}

		/// <summary>
        /// Loggingfilter erstellen
        /// </summary>
        /// <returns></returns>
		public IHistoricalLoggingFilter CreateFilter()
		{
			if (DialogView.Show("LoggingFilterView", "@Logging.LoggingFilterView.Caption", VerifyDialogResultFunction: CheckFilter) == DialogResult.OK)
				return this.filter;

			return null;
		}

		/// <summary>
        /// Testen auf gültigen Loggingfilter
        /// </summary>
        /// <returns></returns>
        private void CheckFilter(object sender, DialogResultEventArgs e)
        {
            //	Dialog muss bleiben
            e.CancelDialogClosing = true;

            //	Anmelden oder Abmelden?
            if (e.Result == DialogResult.OK)
            {
				IHistoricalLoggingFilter LoggingFilter = this.loggingService.CreateHistoricalLoggingFilter();
				LoggingFilter.MinTime = MinTime;
				LoggingFilter.MaxTime = MaxTime;

				//	sollen Kategorien ausgefiltert werden?
				if (!this.isAllCategoriesSelected) {
					List<CategoryFilterItem> selectedCats = new List<CategoryFilterItem>();
					foreach (CategoryFilterItem cat in this.DesiredCategories) {
						if (cat.IsSelected)
							selectedCats.Add(cat);
					}

					//	keine Kategorien gewählt?
					if (selectedCats.Count == 0) {
						this.filter = null;
						new MessageBoxTask("@Logging.LoggingFilterView.NoCategoriesFilterError", "@Logging.LoggingFilterView.Caption", MessageBoxIcon.Exclamation);
						return;
					}

					//	genau eine Kategorie gewählt?
					if (selectedCats.Count == 1) {
						//	dann diese Kategorie und alle ihre Ereignisse angeben

						if (!this.isAllEventsSelected) {
							LoggingFilter.EventNames = new List<string>();
							foreach (EventFilterItem evt in selectedCats[0].DesiredEvents) {
								if (evt.IsSelected)
									LoggingFilter.EventNames.Add(evt.Name);
							}

							//	keine Ereignisse gewählt?
							if (LoggingFilter.EventNames.Count == 0) {
								this.filter = null;
								new MessageBoxTask("@Logging.LoggingFilterView.NoEventsFilterError", "@Logging.LoggingFilterView.Caption", MessageBoxIcon.Exclamation);
								return;
							}
						}

						LoggingFilter.CategoryNames = new List<string>();
                        LoggingFilter.CategoryNames.Add(selectedCats[0].Name);
					} else {
						//	mehr als eine Kategorie gewählt, dann müssen wir Pärchen packen

						LoggingFilter.CategoryNames = new List<string>();
						LoggingFilter.EventNames = new List<string>();
						foreach (CategoryFilterItem cat in selectedCats) {
							foreach (EventFilterItem evt in cat.DesiredEvents) {
								if (this.isAllEventsSelected || evt.IsSelected) {
                                    LoggingFilter.CategoryNames.Add(cat.Name);
									LoggingFilter.EventNames.Add(evt.Name);
								}
							}
						}
					}
				}

				//	soll nach Benutzern gefiltert werden?
				if (!this.isAllUsersSelected) {
					LoggingFilter.Users = new List<string>(); ;
					foreach (UserFilterItem user in this.DesiredUsers) {
						if (user.IsSelected)
							LoggingFilter.Users.Add(user.Name);
					}

					//	kein Benutzer gewählt?
					if (LoggingFilter.Users.Count == 0) {
						this.filter = null;
						new MessageBoxTask("@Logging.LoggingFilterView.NoUserFilterError", "@Logging.LoggingFilterView.Caption", MessageBoxIcon.Exclamation);
						return;
					}
				}

				//	der neue Filter gilt jetzt
				this.filter = LoggingFilter;
			}

            //	fertig, Dialog muss doch nicht bleiben
            e.CancelDialogClosing = false;
        }

        #endregion

        #region Hilfsfunktionen

		public void SetTimeSpan(LoggingTimeSpanFilterType selectedTimeSpanFilterType)
        {
            switch (selectedTimeSpanFilterType)
            {
				case LoggingTimeSpanFilterType.Custom:                    
                    break;
				case LoggingTimeSpanFilterType.Today:
                    MinTime = DateTime.Now.Date;
                    MaxTime = MinTime.Add(TimeSpan.FromDays(1));
                    break;
				case LoggingTimeSpanFilterType.Yesterday:
                    MinTime = DateTime.Now.Date.Add(TimeSpan.FromDays(-1.0));
                    MaxTime = MinTime.Add(TimeSpan.FromDays(1));
                    break;
				case LoggingTimeSpanFilterType.ThisWeek:
                    MinTime = DateTime.Now.Date.Add(TimeSpan.FromDays(-(double)DateTime.Now.Date.DayOfWeek));
                    MaxTime = MinTime.Add(TimeSpan.FromDays(7));
                    break;
				case LoggingTimeSpanFilterType.LastWeek:
                    MinTime = DateTime.Now.Date.Add(TimeSpan.FromDays(-(double)DateTime.Now.Date.DayOfWeek - 7));
                    MaxTime = MinTime.Add(TimeSpan.FromDays(7));
                    break;
                default:
                    break;
            }
        }

        #endregion
    }

    /// <summary>
    /// Aufzählung der unterstützten Zeitraumfiltertypen
    /// </summary>
    public enum LoggingTimeSpanFilterType
    {
        Custom,
        Today,
        Yesterday,
        ThisWeek,
        LastWeek
    }

    /// <summary>
    /// Stellt ein Objekt zur Auflistung der untertützten Zeitraumfiltertypen zur Verfügung.
    /// </summary>
    public class LoggingTimeSpanFilterItem
    {
        private LoggingTimeSpanFilterType filterType;

        public LoggingTimeSpanFilterItem()
        {
        }

		public LoggingTimeSpanFilterItem(LoggingTimeSpanFilterType filterType)
        {
            this.filterType = filterType;
        }

		public LoggingTimeSpanFilterType FilterType
        {
            get { return filterType; }
            set { filterType = value; }
        }

        public override string ToString()
        {
            string retVal;
            switch (filterType)
            {
				case LoggingTimeSpanFilterType.Custom:
                    retVal = ApplicationService.GetText(TextType.ClientApplication, "Logging.LoggingFilterView.TimespanFilter.Custom");
                    break;
				case LoggingTimeSpanFilterType.Today:
                    retVal = ApplicationService.GetText(TextType.ClientApplication, "Logging.LoggingFilterView.TimespanFilter.Today");;
                    break;
				case LoggingTimeSpanFilterType.Yesterday:
					retVal = ApplicationService.GetText(TextType.ClientApplication, "Logging.LoggingFilterView.TimespanFilter.Yesterday");
                    break;
				case LoggingTimeSpanFilterType.ThisWeek:
					retVal = ApplicationService.GetText(TextType.ClientApplication, "Logging.LoggingFilterView.TimespanFilter.ThisWeek");
                    break;
				case LoggingTimeSpanFilterType.LastWeek:
					retVal = ApplicationService.GetText(TextType.ClientApplication, "Logging.LoggingFilterView.TimespanFilter.LastWeek");
                    break;
                default:
                    retVal = filterType.ToString();
                    break;
            }
            return retVal;
        }
    }

    /// <summary>
    /// Stellt ein Objekt zur Auflistung und Auswahl von Kategorien zur Verfügung.
    /// </summary>
    public class CategoryFilterItem : INotifyPropertyChanged
    {
        private bool isSelected;
        private string name;
        private string localizedText;
        private LoggingFilterAdapter adapter;
        private ObservableCollection<EventFilterItem> desiredEvents;
        private string server;

        public CategoryFilterItem()
        {
        }

        public CategoryFilterItem(LoggingFilterAdapter adapter, string name, string localizedText, ObservableCollection<EventFilterItem> desiredEvents, string server)
        {
            this.adapter = adapter;
            this.Name = name;
            this.localizedText = localizedText;
            this.desiredEvents = desiredEvents;
            this.server = server;
        }

        public bool IsSelected
        {
            get { return this.isSelected; }
            set {
                this.isSelected = value;
                if (this.adapter != null)
                    this.adapter.SelectedCategory = this;
            }
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public string LocalizedText
        {
            get { return this.localizedText; }
            set
            {
                if (this.localizedText != value) {
                    this.localizedText = value;
                    this.OnPropertyChanged("LocalizedText");
                }
            }
        }

        public ObservableCollection<EventFilterItem> DesiredEvents
        {
            get { return this.desiredEvents; }
        }

        public string Server
        {
            get { return this.server; }
            set { this.server = value; }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    /// <summary>
    /// Stellt ein Objekt zur Auflistung und Auswahl von Ereignissen zur Verfügung.
    /// </summary>
	public class EventFilterItem : INotifyPropertyChanged
    {
        private bool isSelected;
        private string name;
		private string fullName;
		private string localizedText;

        public EventFilterItem()
        {
        }

		public EventFilterItem(string name, string fullName, string localizedText)
        {
            this.name = name;
            this.localizedText = localizedText;
        }

        public bool IsSelected
        {
            get { return this.isSelected; }
			set { this.isSelected = value; }
        }

        public string Name
        {
			get { return this.name; }
			set { this.name = value; }
        }

		public string FullName
		{
			get { return this.fullName; }
			set { this.fullName = value; }
		}
		
		public string LocalizedText
        {
			get { return this.localizedText; }
			set
			{
				if (this.localizedText != value) {
					this.localizedText = value;
					this.OnPropertyChanged("LocalizedText");
				}
			}
        }

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
    }

	/// <summary>
	/// Stellt ein Objekt zur Auflistung und Auswahl von Benutzern zur Verfügung.
	/// </summary>
	public class UserFilterItem
	{
		private bool isSelected;
		private string name;
		private string fullName;
		private string bothNames;

		public UserFilterItem()
		{
		}

		public UserFilterItem(string name, string fullName)
		{
			this.name = name;
			this.fullName = fullName;
			this.bothNames = string.Format(fullName);
		}

		public bool IsSelected
		{
			get { return this.isSelected; }
			set { this.isSelected = value; }
		}

		public string Name
		{
			get { return this.name; }
			set { this.name = value; }
		}

		public string FullName
		{
			get { return this.fullName; }
			set { this.fullName = value; }
		}

		public string BothNames
		{
			get { return this.bothNames; }
		}
	}
}
