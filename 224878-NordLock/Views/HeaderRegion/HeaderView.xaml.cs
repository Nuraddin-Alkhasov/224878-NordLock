using VisiWin.ApplicationFramework;
using VisiWin.Alarm;
using System.Linq;
using VisiWin.UserManagement;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;
using System;
using System.Windows;
using System.Windows.Media.Animation;
using VisiWin.DataAccess;
using HMI.Interfaces;
using System.Threading;

namespace HMI
{

    [ExportView("HeaderView")]
    public partial class HeaderView : VisiWin.Controls.View
    {
        ICurrentAlarms2 CurrentAlarmList;
        IAlarmItem CurrentAlarm;
        IVariableService VS;
        IVariable VW_CPU1;
        IVariable VW_CPU2;
        IVariable MM;

        public HeaderView()
        {
            this.InitializeComponent();


            VS = ApplicationService.GetService<IVariableService>();
            MM = VS.GetVariable("NL.PLC.Blocks.50 HMI.02 MOP.DB MOP Kommunikation.Kommunikation.Gebrückter Modus durch Remote.Aktiv");

            CurrentAlarmList = ApplicationService.GetService<IAlarmService>().GetCurrentAlarms2();
            CurrentAlarmList.ChangeAlarm +=  SetAlarmLineData;
            CurrentAlarmList.NewAlarm += SetAlarmLineData;
            CurrentAlarmList.ClearAlarm += SetAlarmLineData;

            SetAlarmLineData(null,null);
        }

        #region - - - Set Alarm Label - - -
        private void AlarmLabel_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            Task obTask = Task.Run(() =>
            {
                IUserManagementService userService = ApplicationService.GetService<IUserManagementService>();
                if (userService.CurrentUser != null && userService.CurrentUser.RightNames.Contains("Diagnose"))
                {
                    CurrentAlarm.Acknowledge();
                }
            });
        }
        
        void SetAlarmLineData(object sender, AlarmEventArgs e)
        {
            IAlarmItem[] TT = CurrentAlarmList.Alarms.Where(x => x.Group.Name == "Errors" && x.AlarmState == AlarmState.Active).ToArray();

            Task obTask = Task.Run(() =>
            {
                CurrentAlarm = (TT.Length > 0) ? TT[0] : null;

                Dispatcher.InvokeAsync((Action)delegate
                {
                    if (CurrentAlarm != null)
                    {
                        AlarmText.Text = CurrentAlarm.ActivationTime.ToString() + "  -  " + CurrentAlarm.Text;

                        AlarmText.BeginAnimation(UIElement.OpacityProperty, SetOpacity(1, 1));
                    }
                    else
                    {
                        AlarmText.BeginAnimation(UIElement.OpacityProperty, SetOpacity(0, 1));
                    }
                });
            });
        }

        #endregion

        #region - - - Set CPU Connection Labels - - -

        private void CPU1_Loaded(object sender, RoutedEventArgs e)
        {
            VW_CPU1 = VS.GetVariable("IsAlive.CPU1");
            VW_CPU1.Change += VW_CPU1_Change;
        }

        private void VW_CPU1_Change(object sender, VariableEventArgs e)
        {
            if ((bool)e.Value)
            {
                CPU1.BeginAnimation(UIElement.OpacityProperty, SetOpacity(1, 1));
            }
            else
            {
                CPU1.BeginAnimation(UIElement.OpacityProperty, SetOpacityForever(0, 1));
            }
        }

        private void CPU2_Loaded(object sender, RoutedEventArgs e)
        {
            VW_CPU2 = VS.GetVariable("IsAlive.CPU2");
            VW_CPU2.Change += VW_CPU2_Change;
        }
        private void VW_CPU2_Change(object sender, VariableEventArgs e)
        {
            if ((bool)e.Value)
            {
                CPU2.BeginAnimation(UIElement.OpacityProperty, SetOpacity(1, 1));
            }
            else
            {
                CPU2.BeginAnimation(UIElement.OpacityProperty, SetOpacityForever(0, 1));
            }
        }

        #endregion

        #region - - - Methods - - -

        private DoubleAnimation SetOpacityForever(Double _O, int _T)
        {
            return new DoubleAnimation
            {
                To = _O,
                Duration = TimeSpan.FromSeconds(_T),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
        }

        private DoubleAnimation SetOpacity(Double _O, int _T)
        {
            return new DoubleAnimation
            {
                To = _O,
                Duration = TimeSpan.FromSeconds(_T),
            };
        }

        #endregion

    }
}