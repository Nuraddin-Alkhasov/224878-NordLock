using System;
using System.Windows.Media.Imaging;
using VisiWin.ApplicationFramework;
using WpfAnimatedGif;

namespace HMI.Views.DialogRegion
{

    [ExportView("ServiceScreen")]
    public partial class ServiceScreen : VisiWin.Controls.View
    {

        public ServiceScreen()
        {
            this.InitializeComponent();
         

            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri((new Resources.LocalResources()).Paths.LoadingGif);
            image.EndInit();
            ImageBehavior.SetAnimatedSource(gif, image);
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ApplicationService.SetView("DialogRegion", "EmptyView");
        }
    }
}