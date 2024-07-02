namespace HMI.Reporting
{
    /// <summary>
    /// Basisklasse für ein Objekt, welches dem VisiWinReportViewer signalisiert, wann es den ReportViewer zu verstecken hat.
    /// </summary>
    public abstract class ContentUnderlayNotifer
    {
        /// <summary>
        /// EventHandler für das ContentIsCoveredEvent.
        /// </summary>
        /// <param name="ShowDummy"></param>
        public delegate void ContentIsCoveredEventHandler(bool showDummy);

        /// <summary>
        /// Wird ausgelöst, wenn eine View (die keine EmptyView ist), in eine Region geladen wird, die über dem VisiWinReportViewer
        /// liegt.
        /// </summary>
        public event ContentIsCoveredEventHandler ContentIsCovered;

        /// <summary>
        /// Löst das ContentIsCovered Event aus.
        /// </summary>
        /// <param name="showDummy"></param>
        public void OnContentIsCovered(bool showDummy)
        {
            if (this.ContentIsCovered != null)
            {
                this.ContentIsCovered(showDummy);
            }
        }
    }
}