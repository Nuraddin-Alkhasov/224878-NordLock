using VisiWin.ApplicationFramework;
using System.ComponentModel.Composition;
using VisiWin.Alarm;
using System.Linq;
using System.Collections.Generic;

namespace HMI.Diagnose
{
    [ExportAdapter("AlarmAdapter")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class AlarmAdapter : AdapterBase
    {
 
        private List<IAlarmItem> alarms;
        ICurrentAlarms2 CurrentAlarmList;
        private IAlarmItem alarm;

        public AlarmAdapter()
        {
            if (ApplicationService.IsInDesignMode)
                return;

            CurrentAlarmList = ApplicationService.GetService<IAlarmService>().GetCurrentAlarms2();
            Alarms = CurrentAlarmList.Alarms.Where(x => (x.Group.Name == "Errors" || x.Group.Name == "Warnings") && x.AlarmState == AlarmState.Active).ToList();

            CurrentAlarmList.ChangeAlarm += SetAlarmData;
            CurrentAlarmList.NewAlarm += SetAlarmData;
            CurrentAlarmList.ClearAlarm += SetAlarmData;
        }

        public List<IAlarmItem> Alarms
        {
            get 
            { 
                return this.alarms; 
            }
            set
            {
                if (this.alarms != value)
                {
                    this.alarms = value;
                    this.OnPropertyChanged("Alarms");
                }
            }
        }

        public IAlarmItem Alarm
        {
            get
            {
                return this.alarm;
            }
            set
            {
                if (this.alarm != value)
                {
                    this.alarm = value;
                    this.OnPropertyChanged("Alarm");
                }
            }
        }

        void SetAlarmData(object sender, AlarmEventArgs e)
        {
            Alarms = CurrentAlarmList.Alarms.Where(x => (x.Group.Name == "Errors" || x.Group.Name == "Warnings") && x.AlarmState == AlarmState.Active).ToList();
        }
    }

}
