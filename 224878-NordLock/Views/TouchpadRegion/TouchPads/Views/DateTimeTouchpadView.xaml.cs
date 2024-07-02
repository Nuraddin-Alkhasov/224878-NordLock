using System;
using System.Windows;
using System.Windows.Controls;
using VisiWin.Controls;
using VisiWin.ApplicationFramework;
using System.Windows.Input;
using VisiWin.Commands;
using System.ComponentModel;
using System.Threading;
using VisiWin.Language;

namespace HMI
{
    /// <summary>
    /// Interaction logic for DateTimeTouchpadView.xaml
    /// </summary>
    [ExportView("DateTimeTouchpadView")]
    public partial class DateTimeTouchpadView : VisiWin.Controls.View
    {
        #region private fields

        internal const string CHANNELNAME_TOUCHTARGET = "vw:TouchTarget";
        private DateTimeVarIn selectedDateTimeVarIn;
        private readonly ILanguageService languageService;
        private bool twelveHourMode;
        private bool dayHalfSelctionVisible;
        private bool hourInputVisible;
        private bool minuteInputVisible;
        private bool secondInputVisible;
       

        #endregion

        #region constructor

        public DateTimeTouchpadView()
        {
            this.InitializeComponent();

            if (!ApplicationService.IsInDesignMode)
            {
                languageService = ApplicationService.GetService<ILanguageService>();
            }
        }

        #endregion

        #region overrides

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

        #endregion

        #region private methods

        /// <summary>
        /// Initialisiert das Pad anhand der Einstellungen des ausgewählten DateTimeVarIns
        /// </summary>
        private void InitializePad()
        {            
            lblPadDescription.Text = "Date and Time Pad";

            if (selectedDateTimeVarIn == null)
                return;
            
            // Label-Text übernehmen
            if (!String.IsNullOrEmpty(selectedDateTimeVarIn.LabelText))
                lblPadDescription.Text = selectedDateTimeVarIn.LabelText;

            // Sichtbarkeit von Kalender und Zeiteingabe bestimmen
            if (selectedDateTimeVarIn.DateTimeMode == VisiWin.Language.DateTimeMode.DateOnly)
                TimePanel.Visibility = System.Windows.Visibility.Collapsed;
            else
                TimePanel.Visibility = System.Windows.Visibility.Visible;

            if (selectedDateTimeVarIn.DateTimeMode == VisiWin.Language.DateTimeMode.TimeOnly)
                calendar.Visibility = System.Windows.Visibility.Collapsed;
            else
                calendar.Visibility = System.Windows.Visibility.Visible;

            // Zeitformat-Einstellungen ermitteln
            string timeFormatStringID = String.Format("@[ClientSystem].Components.Time.TimeFormats.{0}", selectedDateTimeVarIn.FormatTime.Substring(1));
            string timeFormatString = languageService.GetText(timeFormatStringID);

            twelveHourMode = timeFormatString.Contains("h");
            dayHalfSelctionVisible = timeFormatString.Contains("t");
            hourInputVisible = timeFormatString.Contains("h") || timeFormatString.Contains("H");
            minuteInputVisible = timeFormatString.Contains("m");
            secondInputVisible = timeFormatString.Contains("s");
           

            // Aktuellen Wert ermitteln
            DateTime currentValue = selectedDateTimeVarIn.Value;

            // Aktuellem Wert in Kalender und Zeiteingabe übernehmen
            calendar.SelectedDate = currentValue.Date;
            calendar.DisplayDate = currentValue.Date;
            hourInput.Value = currentValue.Hour;
            minuteInput.Value = currentValue.Minute;
            secondInput.Value = currentValue.Second;
          

            // Sichtbarkeit Zeiteingaben
            hourInput.Visibility = hourInputVisible ? Visibility.Visible : Visibility.Collapsed;
            minuteInput.Visibility = minuteInputVisible ? Visibility.Visible : Visibility.Collapsed;
            secondInput.Visibility = secondInputVisible ? Visibility.Visible : Visibility.Collapsed;
           

            // AM/PM Auswahl initialisieren
            cboDayHalfSelection.Visibility = dayHalfSelctionVisible ? Visibility.Visible : Visibility.Collapsed;

            // Anpassungen an 12 und 24 Stunden Zeit-Eingabemethode
            if (twelveHourMode)
            {
                if (currentValue.Hour == 0)
                {
                    // Sonderfall 0 Uhr Mitternacht
                    hourInput.Value = 12;
                    cboDayHalfSelection.SelectedIndex = 0;
                }
                else if (currentValue.TimeOfDay.CompareTo(new TimeSpan(12, 0, 0)) < 0)
                    // vor 12 Uhr Mittag -> AM
                    cboDayHalfSelection.SelectedIndex = 0;
                else
                    // nach 12 Uhr Mittag -> PM
                    cboDayHalfSelection.SelectedIndex = 1;

                // Minimum für Stundeneingabe
                hourInput.RawLimitMin = 1;

                // Maximum für Stundeneingabe
                hourInput.RawLimitMax = 12;

                // Der Stundenwert wird im 24 Stunden-Format ausgelesen und muss eventuell angepasst werden
                if (hourInput.Value > 12)
                    hourInput.Value -= 12;
            }
            else
            {
                // Minimum für Stundeneingabe
                hourInput.RawLimitMin = 0;

                // Maximum für Stundeneingabe
                hourInput.RawLimitMax = 23;
            }
        }

        /// <summary>
        /// Übernimmt den eingestellten Wert in das selektierte DatTimeVarIn Control
        /// </summary>
        private void WriteInputValue()
        {
            if (selectedDateTimeVarIn != null)
            {
                // Eingestelltes Datum übernehmen
                DateTime dt = calendar.SelectedDate.Value;

                // Eingestellte Stunde übernehmen
                if (twelveHourMode)
                {
                    // Da der Stundenwert intern im 24 Stundenformat abgelegt ist,
                    // müssen die Eingaben im 12 Stundenformat angepasst werden.
                    if (hourInput.Value == 12)
                    {
                        if (cboDayHalfSelection.SelectedIndex == 0)
                        {
                            // 12 Uhr AM --> 0 Uhr
                            dt = dt.AddHours(0);
                        }
                        else
                        {
                            // 12 Uhr PM --> 12 Uhr
                            dt = dt.AddHours(12);
                        }
                    }
                    else
                    {
                        if (cboDayHalfSelection.SelectedIndex == 0)
                        {
                            // 1-11 Uhr AM --> 1-11 Uhr
                            dt = dt.AddHours(hourInput.Value);
                        }
                        else
                        {
                            // 1-11 Uhr PM --> 13-23 Uhr
                            dt = dt.AddHours(hourInput.Value + 12);
                        }
                    }
                }
                else
                {
                    // Im 24 Stundenformat kann die Eingabe der Stunde direkt übernommen werden
                    dt = dt.AddHours(hourInput.Value);
                }

                // Eingestellte Minute, Sekunde und Millisekunde übernehmen
                dt = dt.AddMinutes(minuteInput.Value);
                dt = dt.AddSeconds(secondInput.Value);
              

				// Eingestelltes Datum und eingestellte Uhrzeit in selektiertes DateTimVarin übernehmen
				selectedDateTimeVarIn.StartEdit();
				selectedDateTimeVarIn.Text = dt.ToString(System.Globalization.CultureInfo.CurrentCulture);
				selectedDateTimeVarIn.StopEditAsync(true);
            }
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

        #endregion
    }
}