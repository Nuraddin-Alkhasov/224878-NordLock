namespace HMI.Reporting
{
    public class ParameterInfo
    {
        /// <summary>
        /// Initialisiert ein Parameterobjekt mit den angegbenen Daten.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="localiableString"></param>
        public ParameterInfo(string name, string localiableString)
        {
            this.Name = name;
            this.LocaliableString = localiableString;
        }

        /// <summary>
        /// Name des im Report definierten Parameters
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Lokalisierbare Zeichenkette für den Wert des Parameters.
        /// </summary>
        public string LocaliableString { get; private set; }
    }
}
