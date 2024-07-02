using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace HMI.UserControls
{
    public partial class PaintType : UserControl
    {

        public PaintType()
        {
            InitializeComponent();
        }
        public string Header
        {
            set
            {
                A.LocalizableHeaderText = value;
            }
        }
        public string Name
        {
            set
            {
                name.VariableName = value;
            }
        }
        public string paintType
        {
            set
            {
                painttype.VariableName = value;
            }
        }

        public string IsSolvent
        {
            set
            {
                solvent.VariableName = value;
            }
        }
        public string WatchDog
        {
            set
            {
                watchdog.VariableName = value;
            }
        }
        public string MaxCoating
        {
            set
            {
                setCL.VariableName = value;
            }
        }
        public string Pump
        {
            set
            {
                pump.VariableName = value;
            }
        }
        public string PumpOn
        {
            set
            {
                pump_on.VariableName = value;
            }
        }
        public string PumpOff
        {
            set
            {
                pump_off.VariableName = value;
            }
        }
        public string OfenUL
        {
            set
            {
               o_UL.VariableName = value;
            }
        }
        public string OfenProcess
        {
            set
            {
                o_process.VariableName = value;
            }
        }
        public string OfenLL
        {
            set
            {
                o_LL.VariableName = value;
            }
        }
        public string CoolingZoneUL
        {
            set
            {
                c_UL.VariableName = value;
            }
        }
        public string CoolingZoneProcess
        {
            set
            {
                c_process.VariableName = value;
            }
        }
        public string CoolingZoneLL
        {
            set
            {
                c_LL.VariableName = value;
            }
        }
        private bool loaded=false;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!loaded)
            {
                Task obTask = Task.Run(async () =>
                {
                    await Task.Delay(100);
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
    }
}
