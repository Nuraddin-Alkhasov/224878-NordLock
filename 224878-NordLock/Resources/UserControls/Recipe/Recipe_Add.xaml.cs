using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace HMI.UserControls
{
    public partial class Recipe_Add : UserControl
    {
        public Recipe_Add()
        {
            InitializeComponent();
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
                      Layout.BeginAnimation(UIElement.OpacityProperty, SetOpacity(1,1));
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

        public override string ToString() { return "Recipe_Add"; }

    }
}
