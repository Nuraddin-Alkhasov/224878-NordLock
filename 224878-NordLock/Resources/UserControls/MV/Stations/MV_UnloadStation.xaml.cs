using HMI.Views.MainRegion.MachineOverview;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;

namespace HMI.UserControls
{
    public partial class MV_UnloadStation : UserControl
    {
        public MV_UnloadStation()
        {
            InitializeComponent();
        }
        IVariableService VS = ApplicationService.GetService<IVariableService>();
        
        IVariable isBox;
        public string IsBox
        {
            set
            {
                isBox = VS.GetVariable(value);
                isBox.Change += isBox_ValueChanged;
            }
        }

        private void isBox_ValueChanged(object sender, VariableEventArgs e)
        {
            if ((bool)e.Value)
            {
                Task obTask = Task.Run(async () =>
                {
                    await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                    {
                        Box.BeginAnimation(UIElement.OpacityProperty, SetOpacity(1, 1));
                    });
                });
            }
            else
            {
                Task obTask = Task.Run(async () =>
                {
                    await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                    {
                        Box.BeginAnimation(UIElement.OpacityProperty, SetOpacity(0, 1));
                    });
                });
            }
        }
        IVariable isMaterial;
        public string IsMaterial
        {
            set
            {
                isMaterial = VS.GetVariable(value);
                isMaterial.Change += isMaterial_ValueChanged;
            }
        }

        private void isMaterial_ValueChanged(object sender, VariableEventArgs e)
        {
            if ((bool)e.Value)
            {
                isMat.Visibility= Visibility.Visible;
            }
            else
            {
                isMat.Visibility = Visibility.Hidden;
            }
        }
        public string Weight
        {
            set
            {
                weight.VariableName = value;
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            (new SP
            {
                Module = 4,
                M1_Station = 0,
                M2_Station = 0,
                M3_Station = 0,
                M4_Station = 8,
                OvenTray = 0,
                CZTray = 0,
                TB_Shelve = 0,
                TB_Level = 0,
                Header = "@Status.Text104",
                Type = "Box"
            }).Execute();
        }
    }
}
