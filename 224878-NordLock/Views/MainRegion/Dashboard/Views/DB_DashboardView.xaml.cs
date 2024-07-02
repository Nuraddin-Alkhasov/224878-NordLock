using System;
using System.Windows;
using System.Windows.Threading;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using VisiWin.UserManagement;
using HMI.Views.MessageBoxRegion;

namespace HMI.Dashboard
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    [ExportView("DB_DashboardView")]
    public partial class DB_DashboardView : View
    {
        private IUserManagementService userManagementService;

        public DB_DashboardView()
        {
            this.InitializeComponent();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.userManagementService = ApplicationService.GetService<IUserManagementService>();

            if (this.userManagementService == null)
            {
                return;
            }

            this.userManagementService.UserLoggedOn += this.userManagementService_UserLoggedOn;
            this.userManagementService.UserLoggedOff += this.userManagementService_UserLoggedOff;
        }

        protected override void OnAttach()
        {
            base.OnAttach();
            this.LoadDashboardConfiguration();
        }

        private void dashboard_IsInEditModeChanged(object sender, EventArgs e)
        {
            if (!this.dashboard.IsInEditMode)
            {
                if (MessageBoxView.Show("@Dashboard.SaveConfig", "@Dashboard.Text24", MessageBoxButton.YesNo, icon: MessageBoxIcon.Question) == MessageBoxResult.Yes)
                {
                    if (this.userManagementService != null)
                    {
                        if (!string.IsNullOrEmpty(this.userManagementService.CurrentUserName))
                        {
                            //ist ein Benutzer angemeldet, so wird die Konfiguration unter seinem Namen abgespeichert.
                            this.dashboard.SaveConfiguration(configurationName: this.userManagementService.CurrentUserName);
                            return;
                        }
                    }

                    //ist kein Benutzer angemeldet bzw der Userservice nicht verfügbar, so wird die Default-Konfiguration gespeichert.
                    this.dashboard.SaveConfiguration();
                }
                else
                {
                    //vorherigen Stand zurücksetzen
                    this.LoadDashboardConfiguration();
                }
            }
        }

        /// <summary>
        /// Lädt die Konfiguration für das Dashboard
        /// </summary>
        private void LoadDashboardConfiguration()
        {
            if ((this.userManagementService != null) && !string.IsNullOrEmpty(this.userManagementService.CurrentUserName) &&
                this.dashboard.ConfigurationExists(this.userManagementService.CurrentUserName))
            {
                this.Dispatcher.BeginInvoke((Action)(() => this.dashboard.LoadConfiguration(configurationName: this.userManagementService.CurrentUserName)), DispatcherPriority.Loaded);
            }
            else
            {
                this.Dispatcher.BeginInvoke((Action)(() => this.dashboard.LoadConfiguration()), DispatcherPriority.Loaded);
            }
        }

        private void userManagementService_UserLoggedOff(object sender, LogOffEventArgs e)
        {
            this.LoadDashboardConfiguration();
        }

        private void userManagementService_UserLoggedOn(object sender, LogOnEventArgs e)
        {
            this.LoadDashboardConfiguration();
        }
    }
}