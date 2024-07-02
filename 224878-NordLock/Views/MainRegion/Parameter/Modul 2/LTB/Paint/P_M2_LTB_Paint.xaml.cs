using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Parameter
{
    /// <summary>
    /// Interaction logic for DigitalIOView.xaml
    /// </summary>
    [ExportView("P_M2_LTB_Paint")]
    public partial class P_M2_LTB_Paint
    {
		
        public P_M2_LTB_Paint()
        {
            this.InitializeComponent();
            
        }

        private void Grid_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                StateCollection Temp_SC = new StateCollection();
                for (int i = 1; i <= 10; i++)
                {
                    string temp = ApplicationService.GetVariableValue("NL.PLC.Blocks.50 HMI.00 Allgemein.Lacktyp Namen.Lacktyp Name[" + i.ToString() + "]").ToString();
                    if (temp != "")
                    {
                        Temp_SC.Add(new State()
                        {
                            Text = temp,
                            Value = i.ToString()
                        });
                    }
                }

                LT1.StateList = Temp_SC;
                LT2.StateList = Temp_SC;
                LT3.StateList = Temp_SC;
                LT4.StateList = Temp_SC;
                LT5.StateList = Temp_SC;
                LT6.StateList = Temp_SC;
                LT7.StateList = Temp_SC;
                LT8.StateList = Temp_SC;
                LT9.StateList = Temp_SC;
                LT10.StateList = Temp_SC;
                LT11.StateList = Temp_SC;
                LT12.StateList = Temp_SC;
                LT13.StateList = Temp_SC;
                LT14.StateList = Temp_SC;
                LT15.StateList = Temp_SC;
            }
        }
        private void A_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Task obTask = Task.Run(async () =>
            {
                await Application.Current.Dispatcher.InvokeAsync((System.Action)delegate
                {
                    A.BeginAnimation(UIElement.OpacityProperty, SetOpacity(1, 1));
                });
            });
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