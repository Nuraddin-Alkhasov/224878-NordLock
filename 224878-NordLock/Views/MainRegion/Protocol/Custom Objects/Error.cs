using System;

namespace HMI.Views.MainRegion.Protocol
{
    public class Error
    {

        public Error()
        {
            Id = -1;
            Charge_Id = -1;
            TimeStamp = "-";
            Text = "";
            Comment = "";
            User = "";
        }

        public long Id { set; get; }
        public long Charge_Id { set; get; }
        public string TimeStamp { set; get; }
       
        public string Text { set; get; }

        public string Comment { set; get; }
        
        public string User { set; get; }
    }
}
