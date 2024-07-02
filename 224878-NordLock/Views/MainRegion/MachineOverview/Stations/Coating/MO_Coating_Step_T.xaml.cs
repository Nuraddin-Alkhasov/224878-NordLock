using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;

namespace HMI.Views.MainRegion.MachineOverview
{

    [ExportView("MO_Coating_Step_T")]
	public partial class MO_Coating_Step_T : VisiWin.Controls.View
	{
 

        public MO_Coating_Step_T()
		{
			this.InitializeComponent();

          
        }


        private void _Loaded(object sender, RoutedEventArgs e)
        {
          
            gb.BeginAnimation(UIElement.OpacityProperty, SetOpacity(1, 1));
               
        }
        private void _Unloaded(object sender, RoutedEventArgs e)
        {

            gb.BeginAnimation(UIElement.OpacityProperty, SetOpacity(0, 1));

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