using System;
using System.Windows;
using System.Windows.Controls;
using VisiWin.Controls;
using VisiWin.ApplicationFramework;
using System.Windows.Input;
using VisiWin.Commands;
using System.ComponentModel;
using System.Threading;

namespace HMI
{
    /// <summary>
    /// Interaction logic for PasswordTouchpadView.xaml
    /// </summary>
    [ExportView("PasswordTouchpadView")]
    public partial class PasswordTouchpadView : VisiWin.Controls.View
    {
        internal const string CHANNELNAME_TOUCHTARGET = "vw:TouchTarget";

        public PasswordTouchpadView()
        {
            this.InitializeComponent();
            this.Loaded += new RoutedEventHandler(AlphaTouchpadView_Loaded);
        }

        private System.Windows.Controls.TextBlock descriptionLabel = null;
        void AlphaTouchpadView_Loaded(object sender, RoutedEventArgs e)
        {
            descriptionLabel = this.FindName("lblAlphaPadDescription") as System.Windows.Controls.TextBlock;
            if (descriptionLabel == null)
                return;
            
            descriptionLabel.Text = "AlphaPad";

            if (ApplicationService.ObjectStore.ContainsKey(CHANNELNAME_TOUCHTARGET))
            {
                var varIn = ApplicationService.ObjectStore.GetValue(CHANNELNAME_TOUCHTARGET);
                if (varIn is TextVarIn)
                    descriptionLabel.Text = ((TextVarIn)varIn).LabelText;
                else if (varIn is PasswordVarIn)
                    descriptionLabel.Text = ((PasswordVarIn)varIn).LabelText;
            }
        }

        private void TouchKeyboard_EscapeKeyPressed(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("TouchpadRegion", "EmptyView");
        }

        private void passwordVarIn1_WriteValueCompleted(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("TouchpadRegion", "EmptyView");
        }

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			ApplicationService.SetView("TouchpadRegion", "EmptyView");
        }
    }
}