using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;

namespace HMI.Parameter
{
	/// <summary>
	/// Interaction logic for BSStepEdit.xaml
	/// </summary>
	[ExportView("P_M4_Mani_Lift_B")]
	public partial class P_M4_Mani_Lift_B : VisiWin.Controls.View
	{

        public P_M4_Mani_Lift_B()
		{
			this.InitializeComponent();
        }
        private void A_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Task obTask = Task.Run(async () =>
            {
                await Application.Current.Dispatcher.InvokeAsync((Action)delegate
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