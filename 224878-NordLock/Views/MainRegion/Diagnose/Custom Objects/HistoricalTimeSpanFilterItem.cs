using VisiWin.ApplicationFramework;
using VisiWin.Language;

namespace HMI
{
    public enum HistoricalTimeSpanFilterType
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
    public class HistoricalTimeSpanFilterItem
    {
        private HistoricalTimeSpanFilterType filterType;

        public HistoricalTimeSpanFilterItem()
        {

        }

        public HistoricalTimeSpanFilterItem(HistoricalTimeSpanFilterType filterType)
        {
            this.filterType = filterType;
        }

        public HistoricalTimeSpanFilterType FilterType
        {
            get { return filterType; }
            set { filterType = value; }
        }

        public override string ToString()
        {
            string retVal;
            switch (filterType)
            {
                case HistoricalTimeSpanFilterType.Custom:
                    retVal = ApplicationService.GetText(TextType.ClientApplication, "HistoricalAlarms.HistoricalAlarmFilterView.TimespanFilter.Custom");
                    break;
                case HistoricalTimeSpanFilterType.Today:
                    retVal = ApplicationService.GetText(TextType.ClientApplication, "HistoricalAlarms.HistoricalAlarmFilterView.TimespanFilter.Today");
                    break;
                case HistoricalTimeSpanFilterType.Yesterday:
                    retVal = ApplicationService.GetText(TextType.ClientApplication, "HistoricalAlarms.HistoricalAlarmFilterView.TimespanFilter.Yesterday");
                    break;
                case HistoricalTimeSpanFilterType.ThisWeek:
                    retVal = ApplicationService.GetText(TextType.ClientApplication, "HistoricalAlarms.HistoricalAlarmFilterView.TimespanFilter.ThisWeek");
                    break;
                case HistoricalTimeSpanFilterType.LastWeek:
                    retVal = ApplicationService.GetText(TextType.ClientApplication, "HistoricalAlarms.HistoricalAlarmFilterView.TimespanFilter.LastWeek");
                    break;
                default:
                    retVal = filterType.ToString();
                    break;
            }
            return retVal;
        }
    }
}
