using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VisiWin.ApplicationFramework;

namespace HMI
{
	/// <summary>
	/// Interaction logic for LoggingFilterView.xaml
	/// </summary>
    [ExportView("LoggingFilterView")]
	public partial class LoggingFilterView : VisiWin.Controls.View
	{
		public LoggingFilterView()
		{
			this.InitializeComponent();
		}

        private void Btn1_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            btn1.IsSelected = true;
        }

        private void Btn2_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            btn2.IsSelected = true;
        }

        private void Btn3_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            btn3.IsSelected = true;
        }

        private void View_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
		cmb.SelectedIndex = 0;
                LoggingFilterAdapter a = (LoggingFilterAdapter)this.DataContext;
                a.SetTimeSpan(a.TimeSpanFilterTypes[a.SelectedTimeSpanFilterTypeIndex].FilterType);
            }

        }
    }
}