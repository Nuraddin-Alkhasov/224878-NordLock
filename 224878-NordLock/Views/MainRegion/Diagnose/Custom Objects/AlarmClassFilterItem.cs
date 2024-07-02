
namespace HMI.Views.MainRegion.Diagnose
{
    public class AlarmClassFilterItem
    {
        private bool isSelected;
        private string className;
        private string localizableText;
        private string server;

        public AlarmClassFilterItem()
        {
        }

        public AlarmClassFilterItem(string className, string localizableText, string server)
        {
            this.className = className;
            this.localizableText = localizableText;
            this.server = server;
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; }
        }

        public string ClassName
        {
            get { return className; }
            set { className = value; }
        }

        public string LocalizableText
        {
            get { return localizableText; }
            set { localizableText = value; }
        }

        public string Server
        {
            get { return server; }
            set { server = value; }
        }
    }
}
