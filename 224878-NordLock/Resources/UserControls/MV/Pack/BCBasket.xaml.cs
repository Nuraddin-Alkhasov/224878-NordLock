﻿using HMI.Views.MainRegion.MachineOverview;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;

namespace HMI.UserControls
{
    public partial class BCBasket : UserControl
    {
        public BCBasket()
        {
            InitializeComponent();
        }

        IVariableService VS = ApplicationService.GetService<IVariableService>();

        IVariable isBasket;
        public string IsBasket
        {
            set
            {
                isBasket = VS.GetVariable(value);
                isBasket.Change += isBasket_ValueChanged;
            }
        }

        private void isBasket_ValueChanged(object sender, VariableEventArgs e)
        {
            if ((bool)e.Value)
            {
                Task obTask = Task.Run(async () =>
                {
                    await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                    {
                        A.BeginAnimation(UIElement.OpacityProperty, SetOpacity(1, 1));
                    });
                });
             
            }
            else 
            {
                Task obTask = Task.Run(async () =>
                {
                    await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                    {
                        A.BeginAnimation(UIElement.OpacityProperty, SetOpacity(0, 1));
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
                Task obTask = Task.Run(async () =>
                {
                    await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                    {
                        ismaterial.BeginAnimation(UIElement.OpacityProperty, SetOpacity(1, 1));
                    });
                });
            }
            else
            {
                Task obTask = Task.Run(async () =>
                {
                    await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                    {
                        ismaterial.BeginAnimation(UIElement.OpacityProperty, SetOpacity(0, 1));
                    });
                });
            }
        }

        IVariable isDischarge;
        public string IsDischarge
        {
            set
            {
                isDischarge = VS.GetVariable(value);
                isDischarge.Change += isDischarge_ValueChanged;
            }
        }

        private void isDischarge_ValueChanged(object sender, VariableEventArgs e)
        {
            if ((bool)e.Value)
            {
                discharge.Visibility = Visibility.Visible;
               
            }
            else
            {
                discharge.Visibility = Visibility.Collapsed;
            }
        }

        IVariable isClean;
        public string IsClean
        {
            set
            {
                isClean = VS.GetVariable(value);
                isClean.Change += isClean_ValueChanged;
            }
        }

        private void isClean_ValueChanged(object sender, VariableEventArgs e)
        {
            if ((bool)e.Value)
            {
                clean.Visibility = Visibility.Collapsed;
            }
            else
            {
                clean.Visibility = Visibility.Visible;
               
            }
        }

        IVariable actualCL;
        public string ActualCoatingLayer
        {
            set
            {
                Value.Visibility = Visibility.Visible;
                Value.VariableName = value;
                actualCL = VS.GetVariable(value);
                actualCL.Change += ActualCL_Change;
            }
        }

        private void ActualCL_Change(object sender, VariableEventArgs e)
        {
            if ((short)e.Value == 0)
            {
                cl.Background = new SolidColorBrush(Colors.White);
            }
            else 
            {
                cl.Background = (System.Windows.Media.Brush)Application.Current.FindResource("FP_Yellow_Gradient");
            }
               
        }

        public string SetCoatingLayer
        {
            set
            {
                Value.Visibility = Visibility.Visible;
                Value.VariableName = value;
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

        public override string ToString() { return "Basket"; }

        #region - - - Status - - -

        public string Header { set; get; }
        public string Type { set; get; }
        public int Module { set; get; }
        public int M1_Station { set; get; }
        public int M2_Station { set; get; }
        public int M3_Station { set; get; }
        public int M4_Station { set; get; }
        public int OvenTray { set; get; }
        public int CZTray { set; get; }
        public int TB_Shelve { set; get; }
        public int TB_Level { set; get; }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (new SP
            {
                Module = Module,
                M1_Station = M1_Station,
                M2_Station = M2_Station,
                M3_Station = M3_Station,
                M4_Station = M4_Station,
                OvenTray = OvenTray,
                CZTray = CZTray,
                TB_Shelve = TB_Shelve,
                TB_Level = TB_Level,
                Header = Header,
                Type = Type
            }).Execute();
        }
        #endregion
    }
}
