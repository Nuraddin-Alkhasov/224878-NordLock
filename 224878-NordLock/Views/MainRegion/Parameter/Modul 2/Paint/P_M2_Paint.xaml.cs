using System;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
namespace HMI.Parameter
{
    /// <summary>
    /// Interaction logic for DigitalIOView.xaml
    /// </summary>
    [ExportView("P_M2_Paint")]
    public partial class P_M2_Paint : View
    {
		
        public P_M2_Paint()
        {
            this.InitializeComponent();
            
        }

        private void P_M2_Paint_pn_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(this.IsVisible && P_M2_Paint_pn.SelectedPanoramaRegionIndex==1) 
            {
                P_M2_Paint_pn.ScrollPrevious();
            }
        }

      
    }
}