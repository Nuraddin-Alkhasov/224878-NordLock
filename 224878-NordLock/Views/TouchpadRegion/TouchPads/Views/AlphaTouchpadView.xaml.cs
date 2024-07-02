using System;
using System.Windows;
using System.Windows.Controls;
using VisiWin.Controls;
using VisiWin.ApplicationFramework;
using System.Windows.Input;
using VisiWin.Commands;
using System.ComponentModel;
using System.Threading;
using System.Linq;

namespace HMI
{
    /// <summary>
    /// Interaction logic for AlphaTouchpadView.xaml
    /// </summary>
    [ExportView("AlphaTouchpadView")]
    public partial class AlphaTouchpadView : VisiWin.Controls.View
    {
        internal const string CHANNELNAME_TOUCHTARGET = "vw:TouchTarget";

        public AlphaTouchpadView()
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
                textVarIn1 = (TextVarIn)ApplicationService.ObjectStore.GetValue(CHANNELNAME_TOUCHTARGET);
                //if (textVarIn1 is TextVarIn)
                descriptionLabel.Text = textVarIn1.LabelText;
                //else if (textVarIn1 is PasswordVarIn @in)
                //{
                //    descriptionLabel.Text = @in.LabelText;
                //}
            }
        }

        private void TouchKeyboard_EscapeKeyPressed(object sender, RoutedEventArgs e)
        {
            FixValueCheck();
            ApplicationService.SetView("TouchpadRegion", "EmptyView");
        }

        private void TextVarIn1_WriteValueCompleted(object sender, RoutedEventArgs e)
        {
            FixValueCheck();
            ApplicationService.SetView("TouchpadRegion", "EmptyView");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            FixValueCheck();

            ApplicationService.SetView("TouchpadRegion", "EmptyView");
        }
        private void FixValueCheck()
        {
            if (textVarIn1.Value.Length < textVarIn1.TextLengthMin || textVarIn1.Value.Length > textVarIn1.TextLengthMax)
            {
                var rand = new Random();
                var temp = textVarIn1.Value;

                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

                textVarIn1.Value = new string(Enumerable.Repeat(chars, (int)textVarIn1.TextLengthMax).Select(s => s[rand.Next(s.Length)]).ToArray());
                textVarIn1.Value = temp;
            }

        }
    }
}