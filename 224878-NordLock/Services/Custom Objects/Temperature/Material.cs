using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace HMI.Services.Custom_Objects
{    
    public class Material
    {
        public Material(uint _OrderId, short _Charge, double _Temperature)
        {
            OrderId = _OrderId;
            Charge = _Charge;
            Temperatures.Add(_Temperature);
            Temperatures.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChangedMethod);
        }

        public uint OrderId;
        public short Charge;
        public double AverageTemperature;

        public ObservableCollection<double> Temperatures = new ObservableCollection<double>();

        private void CollectionChangedMethod(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                AverageTemperature = Temperatures.Sum() / Temperatures.Count;
            }
        }
    }
}
