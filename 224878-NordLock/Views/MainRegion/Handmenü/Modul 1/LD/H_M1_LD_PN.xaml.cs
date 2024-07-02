using VisiWin.ApplicationFramework;
using VisiWin.Controls;
namespace HMI.Handmenu
{
    /// <summary>
    /// Interaction logic for DigitalIOView.xaml
    /// </summary>
    [ExportView("H_M1_LD_PN")]
    public partial class H_M1_LD_PN : View
    {
		
        public H_M1_LD_PN()
        {
            this.InitializeComponent();
            
        }

        private void View_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible && H_M1_LD_pn.SelectedPanoramaRegionIndex!=0)
            {
                H_M1_LD_pn.ScrollPrevious();
            }
        }

        private void Pn_H_ALO1_SelectedPanoramaRegionChanged(object sender, SelectedPanoramaRegionChangedEventArgs e)
        {
            if (H_M1_LD_pn.SelectedPanoramaRegionIndex == 1)
            {
                header.LocalizableText = "@HandMenu.Text10";
            }
            else
            {
                header.LocalizableText = "@HandMenu.Text9";
            }
        }
    }
}