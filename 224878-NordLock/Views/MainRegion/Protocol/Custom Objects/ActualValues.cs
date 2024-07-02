using HMI.Views.MainRegion.Recipe;
using System;
using System.Collections.ObjectModel;

namespace HMI.Views.MainRegion.Protocol
{
    public class ActualValues
    {

        public ActualValues()
        {
            Id = -1;

            PaintTemp = 0;
            PHZTempMin = 0;
            PHZTemp = 0;
            PHZTempMax = 0;
            DryerTempMin = 0;
            DryerTemp = 0;
            DryerTempMax = 0;
            CZTempMin = 0;
            CZTemp = 0;
            CZTempMax = 0;
        }

        public long Id { set; get; }
        public double PaintTemp { set; get; }
        public double PHZTempMin { set; get; }
        public double PHZTemp { set; get; }
        public double PHZTempMax { set; get; }
        public double DryerTempMin { set; get; }
        public double DryerTemp { set; get; }
        public double DryerTempMax { set; get; }
        public double CZTempMin { set; get; }
        public double CZTemp { set; get; }
        public double CZTempMax { set; get; }

    }
}
