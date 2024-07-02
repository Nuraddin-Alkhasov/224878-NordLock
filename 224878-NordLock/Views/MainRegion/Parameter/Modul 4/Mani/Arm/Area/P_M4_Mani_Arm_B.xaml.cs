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
	[ExportView("P_M4_Mani_Arm_B")]
	public partial class P_M4_Mani_Arm_B : VisiWin.Controls.View
	{

        public P_M4_Mani_Arm_B()
		{
			this.InitializeComponent();
        }

		private void A_Loaded(object sender, System.Windows.RoutedEventArgs e)
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

        private bool loaded = false;
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