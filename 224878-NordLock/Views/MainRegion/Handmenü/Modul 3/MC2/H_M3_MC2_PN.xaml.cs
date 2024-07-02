using VisiWin.ApplicationFramework;
using VisiWin.Controls;
namespace HMI.Handmenu
{
    /// <summary>
    /// Interaction logic for DigitalIOView.xaml
    /// </summary>
    [ExportView("H_M3_MC2_PN")]
    public partial class H_M3_MC2_PN : View
    {
		
        public H_M3_MC2_PN()
        {
            this.InitializeComponent();
            
        }

        private void View_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible && H_M3_MC2_pn.SelectedPanoramaRegionIndex!=0)
            {
                H_M3_MC2_pn.ScrollPrevious();
            }
        }
    }
}