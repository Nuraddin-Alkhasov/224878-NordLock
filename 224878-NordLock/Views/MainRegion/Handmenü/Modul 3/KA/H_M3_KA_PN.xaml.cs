using VisiWin.ApplicationFramework;
using VisiWin.Controls;
namespace HMI.Handmenu
{
    /// <summary>
    /// Interaction logic for DigitalIOView.xaml
    /// </summary>
    [ExportView("H_M3_KA_PN")]
    public partial class H_M3_KA_PN : View
    {
		
        public H_M3_KA_PN()
        {
            this.InitializeComponent();
            
        }

        private void View_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible && H_M3_KA_pn.SelectedPanoramaRegionIndex!=0)
            {
                H_M3_KA_pn.ScrollPrevious();
            }
        }

        private void _SelectedPanoramaRegionChanged(object sender, SelectedPanoramaRegionChangedEventArgs e)
        {
            switch (H_M3_KA_pn.SelectedPanoramaRegionIndex) 
            {
                case 0: header.LocalizableText = "@HandMenu.Text58"; break;
                case 1: header.LocalizableText = "@HandMenu.Text59"; break;
            }
        }
    }
}