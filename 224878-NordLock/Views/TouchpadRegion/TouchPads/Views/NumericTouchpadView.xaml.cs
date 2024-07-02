using System;
using System.Windows;
using System.Windows.Controls;
using VisiWin.Controls;
using VisiWin.ApplicationFramework;

namespace HMI
{
    /// <summary>
    /// Interaction logic for NumericTouchpadView.xaml
    /// </summary>
    [ExportView("NumericTouchpadView")]
    public partial class NumericTouchpadView : VisiWin.Controls.View
    {
        internal const string CHANNELNAME_TOUCHTARGET = "vw:TouchTarget";

        public NumericTouchpadView()
        {
            this.InitializeComponent();
            this.Loaded += new RoutedEventHandler(NumericTouchpadView_Loaded);
        }

        private System.Windows.Controls.TextBlock descriptionLabel = null;
        void NumericTouchpadView_Loaded(object sender, RoutedEventArgs e)
        {
            descriptionLabel = this.FindName("lblNumericPadDescription") as System.Windows.Controls.TextBlock;
            if (descriptionLabel == null)
                return;

            descriptionLabel.Text = "NumPad";

            if (ApplicationService.ObjectStore.ContainsKey(CHANNELNAME_TOUCHTARGET))
            {
                numericVarIn = ApplicationService.ObjectStore.GetValue(CHANNELNAME_TOUCHTARGET) as NumericVarIn;
                if (numericVarIn != null)
                {
                    descriptionLabel.Text = numericVarIn.LabelText;
                }
            }
        }

        private void Touchkeyboard1_EscapeKeyPressed(object sender, System.Windows.RoutedEventArgs e)
        {
            FixValueCheck();

            ApplicationService.SetView("TouchpadRegion", "EmptyView");
        }

        private void NumericVarIn_WriteValueCompleted(object sender, RoutedEventArgs e)
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

            if (numericVarIn.Value < numericVarIn.LimitMin || numericVarIn.Value > numericVarIn.LimitMax)
            {
                var rand = new Random();
                var temp = numericVarIn.Value;
                numericVarIn.Value = rand.Next((int)numericVarIn.LimitMin, (int)numericVarIn.LimitMax);
                numericVarIn.Value = temp;
            }
        }
    }
}