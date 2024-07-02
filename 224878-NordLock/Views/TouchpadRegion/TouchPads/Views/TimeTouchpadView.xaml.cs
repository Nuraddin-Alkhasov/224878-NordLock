using System;
using System.Windows;
using System.Windows.Controls;
using VisiWin.Controls;
using VisiWin.ApplicationFramework;
using System.Windows.Input;
using VisiWin.Language;
using HMI.Views.MessageBoxRegion;

namespace HMI
{
    /// <summary>
    /// Interaction logic for DateTimeTouchpadView.xaml
    /// </summary>
    [ExportView("TimeTouchpadView")]
    public partial class TimeTouchpadView : VisiWin.Controls.View
    {


        internal const string CHANNELNAME_TOUCHTARGET = "vw:TouchTarget";
        private DateTimeVarIn selectedDateTimeVarIn;

        public TimeTouchpadView()
        {
            this.InitializeComponent();
        }


        protected override void OnLoaded()
        {
            if (ApplicationService.ObjectStore.ContainsKey(CHANNELNAME_TOUCHTARGET))
            {
                var varIn = ApplicationService.ObjectStore.GetValue(CHANNELNAME_TOUCHTARGET);
                if (varIn is DateTimeVarIn)
                {
                    selectedDateTimeVarIn = varIn as DateTimeVarIn;
                    InitializePad();
                }
            }

            this.btnEnter.Focus();

            base.OnLoaded();
        }

        /// <summary>
        /// After a click in the calendar, the control has captured the mouse.
        /// We have to release the capture to make the buttons on the DateTimePad work again.
        /// </summary>
        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);
            if (Mouse.Captured is Calendar || Mouse.Captured is System.Windows.Controls.Primitives.CalendarItem)
            {
                Mouse.Capture(null);
            }
        }

        private void InitializePad()
        {
            lblPadDescription.Text = "Time Pad";
            hourInput.Value = selectedDateTimeVarIn.Value.Hour;
            minuteInput.Value = selectedDateTimeVarIn.Value.Minute;
            secondInput.Value = selectedDateTimeVarIn.Value.Second;
        }
    

        private void WriteInputValue()
        {
            DateTime dt = new DateTime();
            dt = dt.AddHours(hourInput.Value);
            dt = dt.AddMinutes(minuteInput.Value);
            dt = dt.AddSeconds(secondInput.Value);

            if (dt <= new DateTime(1, 1, 1, 0, 1, 0))
            {
                dt = new DateTime(1, 1, 1, 0, 1, 0);
            }

            selectedDateTimeVarIn.StartEdit();
            selectedDateTimeVarIn.Text = dt.ToString(System.Globalization.CultureInfo.CurrentCulture);
            selectedDateTimeVarIn.StopEditAsync(true);

        }

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
            selectedDateTimeVarIn = null;
            ApplicationService.SetView("TouchpadRegion", "EmptyView");
        }

        private void BtnEnter_Click(object sender, RoutedEventArgs e)
        {

            WriteInputValue();
            selectedDateTimeVarIn = null;
            ApplicationService.SetView("TouchpadRegion", "EmptyView");
            
        }
    }
}