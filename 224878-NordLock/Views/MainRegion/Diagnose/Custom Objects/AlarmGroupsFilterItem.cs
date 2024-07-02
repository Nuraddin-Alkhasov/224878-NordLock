namespace HMI.Views.MainRegion.Diagnose
{
    public class AlarmGroupsFilterItem
    {
        private bool isSelected;
        private string groupName;
        private string groupTree;
        private string localizableText;
        private string server;

        public AlarmGroupsFilterItem()
        {
        }

        public AlarmGroupsFilterItem(string groupName, string groupTree, string localizableText, string server)
        {
            this.groupName = groupName;
            this.groupTree = groupTree;
            this.localizableText = localizableText;
            this.server = server;
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; }
        }

        public string GroupName
        {
            get { return groupName; }
            set { groupName = value; }
        }

        public string GroupTree
        {
            get { return groupTree; }
            set { groupTree = value; }
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
