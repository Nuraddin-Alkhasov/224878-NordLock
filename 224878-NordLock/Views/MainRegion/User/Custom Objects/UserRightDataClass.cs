using System;
using System.ComponentModel;


namespace HMI.Views.MainRegion.User
{
    public class UserRightDataClass : INotifyPropertyChanged
    {
        public UserRightDataClass(bool on, string right, string rt)
        {
            this.on = on;
            this.Right = right;
            RightText = rt;
        }

        private string right;
        public string Right
        {
            get { return this.right; }
            set { this.right = value; OnPropertyChanged("Right"); }
        }
        private string rightText;
        public string RightText
        {
            get { return this.rightText; }
            set { this.rightText = value; OnPropertyChanged("RightText"); }
        }

        private bool on;
        public bool On
        {
            get { return this.on; }
            set { this.on = value; OnPropertyChanged("On"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(String info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
