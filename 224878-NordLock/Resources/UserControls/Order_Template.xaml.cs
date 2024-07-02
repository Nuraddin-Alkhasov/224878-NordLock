using HMI.Views.MainRegion.Protocol;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;

namespace HMI.UserControls
{
    public partial class Order_Template : UserControl
    {
        public Order_Template()
        {
            InitializeComponent();
        }
        private Order o = new Order();
        public Order O
        {
            set
            {
                o = value;
                if (o != null) 
                {
                    A.LabelText = value.Data_2;
                    data1.Value = value.Data_1;
                    data3.Value = value.Data_3;
                    mr.Value = value.MR;
                }
                else
                {
                    A.LabelText = "";
                    data1.Value = "";
                    data3.Value = "";
                    mr.Value = "";
                }          
            }
            get { return o; }
        }

        private bool loaded=false;
      
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!loaded)
            {
                Task obTask = Task.Run(() =>
              {
                  Application.Current.Dispatcher.InvokeAsync((Action)delegate
                  {
                      A.BeginAnimation(UIElement.OpacityProperty, SetOpacity(1,1));
                  });
              });

                loaded = true;
            }
        }
        private DoubleAnimation SetOpacity(Double _O, int _T)
        {
            return new DoubleAnimation
            {
                To = _O,
                Duration = TimeSpan.FromSeconds(_T),
            };
        }

        public override string ToString() { return "Order_Template"; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("MessageBoxRegion", "MO_MRData", O.MR_Id);
        }
    }
}
