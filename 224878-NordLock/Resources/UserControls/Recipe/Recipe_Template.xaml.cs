using HMI.Views.MainRegion.Recipe;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace HMI.UserControls
{
    public partial class Recipe_Template : UserControl
    {
        public Recipe_Template()
        {
            InitializeComponent();
        }
        public string LocalizableLabelText
        {
            set
            {
                A.LocalizableLabelText = value;
            }
        }

        private RecipeTemplateData rtd;
        public RecipeTemplateData RTD
        {
            set
            {
                rtd = value;
                if (rtd != null) 
                {
                    _name.Value = RTD.Name;
                    _descr.Value = RTD.Description;
                    A.SymbolResourceKey = RTD.Symbol;
                }
            
            }
            get { return rtd; }
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

        public override string ToString() { return "Recipe_Template"; }
    }
}
