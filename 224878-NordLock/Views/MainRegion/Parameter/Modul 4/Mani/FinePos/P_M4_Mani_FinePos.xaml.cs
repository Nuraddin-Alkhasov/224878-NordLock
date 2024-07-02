using System;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
namespace HMI.Parameter
{
    /// <summary>
    /// Interaction logic for DigitalIOView.xaml
    /// </summary>
    [ExportView("P_M4_Mani_FinePos")]
    public partial class P_M4_Mani_FinePos : View
    {
		
        public P_M4_Mani_FinePos()
        {
            this.InitializeComponent();
            
        }

        private void P_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Task obTask = Task.Run(() =>
            {
                Application.Current.Dispatcher.InvokeAsync((Action)delegate
                {
                    Reg.Content = new P_M4_Mani_FinePos_P();
                });
            });

        }

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible) 
            {
                StateCollection Temp_SC = new StateCollection();
                int i = 1;
                for (char c = 'A'; c <= 'L'; c++)
                {
                    Temp_SC.Add(new State()
                    {
                        Text = c.ToString(),
                        Value = i.ToString()
                    });
                    i++;
                }
                cb.StateList = Temp_SC;
            }
        }

        private void Reg_Unloaded(object sender, RoutedEventArgs e)
        {
            Reg.Content = null;
        }
    }
}