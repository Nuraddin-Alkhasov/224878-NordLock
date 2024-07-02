
using VisiWin.Alarm;

namespace HMI.Views.MainRegion.Diagnose
{
    public class AlarmStateFilterItem
    {
        private bool isSelected;
        private AlarmState alarmState;
        private string localizableText;

        public AlarmStateFilterItem()
        {
        }

        public AlarmStateFilterItem(AlarmState alarmState, string localizableText)
        {
            this.alarmState = alarmState;
            this.localizableText = localizableText;
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; }
        }

        public AlarmState AlarmState
        {
            get { return alarmState; }
            set { alarmState = value; }
        }

        public string LocalizableText
        {
            get { return localizableText; }
            set { localizableText = value; }
        }
    }
}
