using System;
using System.ComponentModel;


namespace HMI.Views.MainRegion.User
{
    public class GroupData : INotifyPropertyChanged
    {
        public GroupData(string name, string text, string comment)
        {
            this.Name = name;
            this.Text = text;
            this.Comment = comment;
        }

        private string name;
        public string Name
        {
            get { return this.name; }
            set { this.name = value; OnPropertyChanged("Name"); }
        }

        private string text;
        public string Text
        {
            get { return this.text; }
            set { this.text = value; OnPropertyChanged("Text"); }
        }

        private string comment;
        public string Comment
        {
            get { return this.comment; }
            set { this.comment = value; OnPropertyChanged("Comment"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(String info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
