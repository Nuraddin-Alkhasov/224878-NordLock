using System;
using System.Windows;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;

namespace HMI.Views.MainRegion.Protocol
{
	[ExportView("Protocol_CSV_T")]
	public partial class Protocol_CSV_T : VisiWin.Controls.View
	{

        public Protocol_CSV_T()
		{
			this.InitializeComponent();
        }

		public double SpinSpeed { set { v1.Value = value; } }
		public int TiltTime { set { v2.Value = value; } }
		public int ReversalTime { set { v3.Value = value; } }
		public double TiltAngle { set { v4.Value = value; } }

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