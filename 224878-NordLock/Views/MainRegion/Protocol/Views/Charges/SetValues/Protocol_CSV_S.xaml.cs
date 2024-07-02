using System;
using System.Windows;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;

namespace HMI.Views.MainRegion.Protocol
{
	[ExportView("Protocol_CSV_S")]
	public partial class Protocol_CSV_S : VisiWin.Controls.View
	{

        public Protocol_CSV_S()
		{
			this.InitializeComponent();
        }

		public double SpinSpeed1 { set { v1.Value = value; } }
		public int SpinTime1 { set { v2.Value = value; } }
		public double SpinSpeed2 { set { v3.Value = value; } }
		public int PlanetTime { set { v4.Value = value; } }
		public double PlanetSpeed { set { v5.Value = value; } }
		public double SpinSpeed3 { set { v6.Value = value; } }
		public int SpinTime3 { set { v7.Value = value; } }

        private void View_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                //Task obTask = Task.Run(async () =>
                //{
                //    await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                //    {
                A.BeginAnimation(UIElement.OpacityProperty, SetOpacity(1, 1));
                //    });
                //});
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