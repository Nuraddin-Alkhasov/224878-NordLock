using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Reporting
{
    /// <summary>
    /// Interaction logic for ReportView.xaml
    /// </summary>
    [ExportView("ReportView")]
    public partial class ReportView : View
    {
        private static bool isOpen;

        public ReportView()
        {
            this.InitializeComponent();
        }

        public static void Show(ReportConfiguration reportConfiguration)
        {
            if (isOpen)
            {
                return;
            }

            isOpen = true;

            ReportViewAdapter adapter = (ReportViewAdapter)ApplicationService.GetAdapter("ReportViewAdapter");
            adapter.ReportConfiguration = reportConfiguration;
           

            isOpen = false;
        }
    }
}