using HMI.Views.MainRegion.Recipe;
using System;
using System.Collections.ObjectModel;

namespace HMI.Views.MainRegion.Protocol
{
    public class SetValues
    {

        public SetValues()
        {
            Id = -1;
            PaintTemp = 0;
            PHZTemp = 0;
            DryerTemp = 0;
            CZTemp = 0;
            Values = new ObservableCollection<VWVariable>();
        }

        public long Id { set; get; }
        public ObservableCollection<VWVariable> Values { set; get; }
        public string PaintType { set; get; }
        public double PaintTemp { set; get; }
        public double PHZTemp { set; get; }
        public double DryerTemp { set; get; }
        public double CZTemp { set; get; }
    }
}
