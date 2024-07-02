using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;

namespace HMI.UserControls
{
    public partial class MV_LD : UserControl
    {
        public MV_LD()
        {
            InitializeComponent();
        }

        IVariableService VS = ApplicationService.GetService<IVariableService>();

        IVariable mod1Status;

        public string Mod1Status
        {
            set
            {
                mod1Status = VS.GetVariable(value);
                mod1Status.Change += mod1Status_ValueChanged;
            }
        }

        private void mod1Status_ValueChanged(object sender, VariableEventArgs e)
        {
            switch ((short)e.Value)
            {
                case 1: Mod1.Visibility = Visibility.Visible; Mod1.LocalizableText = "@MainView.Text65"; break;
                case 2: Mod1.Visibility = Visibility.Visible; Mod1.LocalizableText = "@MainView.Text76"; break;
                case 3: Mod1.Visibility = Visibility.Visible; Mod1.LocalizableText = "@MainView.Text77"; break;
                default: Mod1.Visibility = Visibility.Hidden; break;
            }
        }

   

        IVariable mod2Status;

        public string Mod2Status
        {
            set
            {
                mod2Status = VS.GetVariable(value);
                mod2Status.Change += mod2Status_ValueChanged;
            }
        }

        private void mod2Status_ValueChanged(object sender, VariableEventArgs e)
        {
            switch ((short)e.Value)
            {
                case 1: Mod2.Visibility = Visibility.Visible; break;
                default: Mod2.Visibility = Visibility.Hidden; break;
            }
        }
        private bool loaded=false;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!loaded)
            {
                Task obTask = Task.Run(async () =>
                {
                    await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                     {
                         A.BeginAnimation(UIElement.OpacityProperty, SetOpacity(1, 1));
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

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("DialogRegion", "MO_DataPicker");
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("MainRegion", "MO_LiftandTilter");
        }
    }
}
