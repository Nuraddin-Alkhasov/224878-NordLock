using VisiWin.ApplicationFramework;
using VisiWin.Controls;
namespace HMI.Handmenu
{
    /// <summary>
    /// Interaction logic for DigitalIOView.xaml
    /// </summary>
    [ExportView("H_M3_Mani_PN")]
    public partial class H_M3_Mani_PN : View
    {
		
        public H_M3_Mani_PN()
        {
            this.InitializeComponent();
            
        }

        private void View_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible && H_M3_Mani_pn.SelectedPanoramaRegionIndex!=0)
            {
                H_M3_Mani_pn.SelectedPanoramaRegionIndex = 0;
            }
        }

        private void _SelectedPanoramaRegionChanged(object sender, SelectedPanoramaRegionChangedEventArgs e)
        {
            switch (H_M3_Mani_pn.SelectedPanoramaRegionIndex) 
            {
                case 0: header.LocalizableText = "@HandMenu.Text49"; break;
                case 1: header.LocalizableText = "@HandMenu.Text47"; break;
                case 2: header.LocalizableText = "@HandMenu.Text48"; break;
            }
        }
    }
}