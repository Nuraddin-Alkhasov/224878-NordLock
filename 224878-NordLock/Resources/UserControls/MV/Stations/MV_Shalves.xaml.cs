using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;

namespace HMI.UserControls
{
    public partial class MV_Shalves : UserControl
    {
        public MV_Shalves()
        {
            InitializeComponent();
        }
        IVariableService VS = ApplicationService.GetService<IVariableService>();

        IVariable qsdoor1Status;
        public string QSDoor1Status
        {
            set
            {
                qsdoor1Status = VS.GetVariable(value);
                qsdoor1Status.Change += qsdoor1Status_ValueChanged;
            }
        }

        private void qsdoor1Status_ValueChanged(object sender, VariableEventArgs e)
        {
            if ((short)e.Value == 1 || (short)e.Value == 2)
            {
                QSDoor1.SymbolResourceKey = "QSDoorClosed";
            }
            else
            {
                QSDoor1.SymbolResourceKey = "QSDoorOpen";
            }
        }


        IVariable qsdoor2Status;
        public string QSDoor2Status
        {
            set
            {
                qsdoor2Status = VS.GetVariable(value);
                qsdoor2Status.Change += qsdoor2Status_ValueChanged;
            }
        }

        private void qsdoor2Status_ValueChanged(object sender, VariableEventArgs e)
        {
            if ((short)e.Value==1|| (short)e.Value == 2)
            {
                QSDoor2.SymbolResourceKey = "QSDoorClosed";
            }
            else
            {
                QSDoor2.SymbolResourceKey = "QSDoorOpen";
            }
        }

        IVariable QSStatus;
        public string QualityStatus
        {
            set
            {
                QSStatus = VS.GetVariable(value);
                QSStatus.Change += QualityStatus_ValueChanged;
            }
        }

        private void QualityStatus_ValueChanged(object sender, VariableEventArgs e)
        {
            switch ((short)e.Value)
            {
                case 1 : qs.Visibility = Visibility.Visible; qs.IsBlinkEnabled = false; break;
                case 2 : qs.Visibility = Visibility.Visible; qs.IsBlinkEnabled = true; break;
                default: qs.Visibility = Visibility.Hidden; break;

            }
        }

        IVariable LRStatus;
        public string TBReturnStatus
        {
            set
            {
                LRStatus = VS.GetVariable(value);
                LRStatus.Change += TBReturnStatus_ValueChanged;
            }
        }

        private void TBReturnStatus_ValueChanged(object sender, VariableEventArgs e)
        {
            switch ((short)e.Value)
            {
                case 1: lr.Visibility = Visibility.Visible; break;
                default: lr.Visibility = Visibility.Hidden; break;

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

 
    }
}
