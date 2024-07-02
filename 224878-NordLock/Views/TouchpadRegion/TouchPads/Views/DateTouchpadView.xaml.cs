using System;
using System.Windows;
using System.Windows.Controls;
using VisiWin.Controls;
using VisiWin.ApplicationFramework;
using System.Windows.Input;
using VisiWin.Language;

namespace HMI
{
    /// <summary>
    /// Interaction logic for DateTimeTouchpadView.xaml
    /// </summary>
    [ExportView("DateTouchpadView")]
    public partial class DateTouchpadView : VisiWin.Controls.View
    {
        #region private fields

        internal const string CHANNELNAME_TOUCHTARGET = "vw:TouchTarget";
        private DateTimeVarIn selectedDateTimeVarIn;
       
       

        #endregion

        #region constructor

        public DateTouchpadView()
        {
            this.InitializeComponent();

         
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

            if (selectedDateTimeVarIn.DateTimeMode == VisiWin.Language.DateTimeMode.TimeOnly)
                calendar.Visibility = System.Windows.Visibility.Collapsed;
            else
                calendar.Visibility = System.Windows.Visibility.Visible;

            // Zeitformat-Einstellungen ermitteln
        
          

            // Aktuellen Wert ermitteln
            DateTime currentValue = selectedDateTimeVarIn.Value;

            // Aktuellem Wert in Kalender und Zeiteingabe übernehmen
            calendar.SelectedDate = currentValue.Date;
            calendar.DisplayDate = currentValue.Date;

            // Anpassungen an 12 und 24 Stunden Zeit-Eingabemethode
         
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