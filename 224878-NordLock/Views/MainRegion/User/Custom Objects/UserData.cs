using System;
using System.ComponentModel;
using VisiWin.UserManagement;

namespace HMI.Views.MainRegion.User
{
    public class UserData : INotifyPropertyChanged
    {
        public UserData(string name, string fullname, UserState state, string group, string comment, DateTime deactivationtime)
        {
            this.Name = name;
            this.FullName = fullname;
            this.State = state;
            this.Comment = comment;
            this.Group = group;
            if (deactivationtime > DateTime.Now)
            {
                this.DeactivationTime = deactivationtime.ToString();
            }
            else
            {
                this.DeactivationTime = "- - - -";
            }
        }

        private string name;
        public string Name
        {
            get { return this.name; }
            set { this.name = value; OnPropertyChanged("Name"); }
        }

        private string fullName;
        public string FullName
        {
            get { return this.fullName; }
            set { this.fullName = value; OnPropertyChanged("FullName"); }
        }

        private UserState state;
        public UserState State
        {
            get { return this.state; }
            set { this.state = value; OnPropertyChanged("State"); }
        }

        private string group;
        public string Group
        {
            get { return this.group; }
            set { this.group = value; OnPropertyChanged("Group"); }
        }

        private string comment;
        public string Comment
        {
            get { return this.comment; }
            set { this.comment = value; OnPropertyChanged("Comment"); }
        }

        private string deactivationtime;
        public string DeactivationTime
        {
            get { return this.deactivationtime; }
            set { this.deactivationtime = value; OnPropertyChanged("DeactivationTime"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(String info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
