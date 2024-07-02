using HMI.Views.MessageBoxRegion;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;
using VisiWin.Recipe;
using WpfAnimatedGif;

namespace HMI.Views.MainRegion.MachineOverview
{

	[ExportView("MO_DataOverwrite")]
	public partial class MO_DataOverwrite
	{
       // IVariableService VS;
      //  private readonly IVariable inPause;

        public MO_DataOverwrite()
		{
			this.InitializeComponent();
            //VS = ApplicationService.GetService<IVariableService>();
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri((new Resources.LocalResources()).Paths.LoadingGif);
            image.EndInit();
            ImageBehavior.SetAnimatedSource(gif, image);

            // inPause = VS.GetVariable("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data from PC.Fahre Anlage in Pause");
            // inPause.Change += InPause_Change;

        }

        private void InPause_Change(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue)
            {
                if ((bool)e.Value)
                {
                    pn_dataoverwrite.ScrollNext();
                }
                else
                {
                    pn_dataoverwrite.ScrollPrevious();
                }
                
            }
        }

        private void Pn_dataoverwrite_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (pn_dataoverwrite.SelectedPanoramaRegionIndex == 0)
            {
                pn_dataoverwrite.ScrollNext();
            }
            else
            {
                pn_dataoverwrite.ScrollPrevious();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("DialogRegion", "EmptyView");
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxView.Show("@StatusView.Text28", "@Tasten.Text12", MessageBoxButton.YesNo, MessageBoxResult.No, MessageBoxIcon.Question) == MessageBoxResult.Yes)
            {
                IRegionService iRS = ApplicationService.GetService<IRegionService>();
                MO_DO_Data MODOData = (MO_DO_Data)iRS.GetView("MO_DO_Data");
                ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.MR.Header.Maschinenprogramm", MODOData.mr.Value);
                ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.MR.Header.User", MODOData.user.Value);
                ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data from PC.Fahre Anlage in Pause", true);
            }
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxView.Show("@StatusView.Text37", "@Tasten.Text13", MessageBoxButton.YesNo, MessageBoxResult.No, MessageBoxIcon.Question) == MessageBoxResult.Yes)
            {
                ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data from PC.Fahre Anlage in Pause abbruch", true);
            }
        }

        private void View_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                //if (ApplicationService.GetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data from PC.Fahre Anlage in Pause").ToString() == "true")
                //{
                //    pn_dataoverwrite.SelectedPanoramaRegionIndex = 1;
                //}
            }           
        }
    }
}