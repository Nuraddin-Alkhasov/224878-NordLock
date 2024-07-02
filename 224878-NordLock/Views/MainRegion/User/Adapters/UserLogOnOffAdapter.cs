using VisiWin.ApplicationFramework;
using System.ComponentModel;
using VisiWin.UserManagement;
using VisiWin.Language;
using VisiWin.Commands;
using System.Windows.Input;
using HMI.Views.MessageBoxRegion;

namespace HMI.Views.MainRegion.User
{
    [ExportAdapter("UserLogOnOffAdapter")]
    public class UserLogOnOffAdapter : AdapterBase, INotifyPropertyChanged
    {
        #region Felder

        private IUserManagementService userService;
        private readonly ILanguageService textService;

        #endregion Felder

        public UserLogOnOffAdapter()
        {
            if (ApplicationService.IsInDesignMode)
                return;

            this.userService = ApplicationService.GetService<IUserManagementService>();
            this.textService = ApplicationService.GetService<ILanguageService>();

            this.userService.UserLoggedOn += UserService_UserLoggedOn;
            this.userService.UserLoggedOff += UserService_UserLoggedOff;

            this.LogOnCommand = new ActionCommand(OnLogOnCommandExecuted);
            this.LogOffCommand = new ActionCommand(OnLogOffCommandExecuted);
            this.CloseDialogCommand = new ActionCommand(OnDialogClosingCommand);

            SetData();

        }

        private void UserService_UserLoggedOff(object sender, LogOffEventArgs e)
        {
            SetData();
        }

        private void UserService_UserLoggedOn(object sender, LogOnEventArgs e)
        {
            SetData();
        }

        #region Befehle

        public ICommand CloseDialogCommand { get; set; }
        private void OnDialogClosingCommand(object parameter)
        {
            SetData();
            ApplicationService.SetView("DialogRegion", "EmptyView");
        }

        public ICommand LogOnCommand { get; set; }
        private void OnLogOnCommandExecuted(object parameter)
        {


            if (string.IsNullOrEmpty(this.CurrentUserName))
            {
                new MessageBoxTask("@UserManagement.Results.ChooseUser", "@UserManagement.View.LogOnOff", MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrEmpty(this.CurrentUserName))
            {
                new MessageBoxTask("@UserManagement.Results.ChooseUser", "@UserManagement.View.LogOnOff", MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrEmpty(this.Password))
            {
                new MessageBoxTask("@UserManagement.Results.EnterPassword", "@UserManagement.View.LogOnOff", MessageBoxIcon.Information);
                return;
            }

            LogOnSuccess success = this.userService.LogOn(this.CurrentUserName, this.Password);
            if (success != LogOnSuccess.Succeeded)
            {
                string errorText = GetErrorText("LogOnError", "LogOnSuccess", success.ToString());
                new MessageBoxTask(errorText, "@UserManagement.View.LogOnOff", MessageBoxIcon.Exclamation);


                if (success == LogOnSuccess.RenewPassword)
                {

                    UserAdapter ua = (UserAdapter)ApplicationService.GetAdapter("UserAdapter");
                    ua.ShowChangePasswordDialog(this.CurrentUserName);
                    userService.LogOn(this.CurrentUserName, ua.Password);
                    SetData();
                }
            }
            ApplicationService.SetView("DialogRegion", "EmptyView");
        }

        public ICommand LogOffCommand { get; set; }
        private void OnLogOffCommandExecuted(object parameter)
        {
            this.userService.LogOff();
        }

        #endregion Befehle

        #region Bindungsmöglichkeiten

        private string currentUserName;
        public string CurrentUserName
        {
            get { return this.currentUserName; }
            set { this.currentUserName = value; OnPropertyChanged("CurrentUserName"); }
        }

        private string password;
        public string Password
        {
            get { return this.password; }
            set { this.password = value; OnPropertyChanged("Password"); }
        }

        private bool islogedin;
        public bool IsLogedIn
        {
            get { return this.islogedin; }
            set { this.islogedin = value; OnPropertyChanged("IsLogedIn"); }
        }

        private bool islogedoff;
        public bool IsLogedOff
        {
            get { return this.islogedoff; }
            set { this.islogedoff = value; OnPropertyChanged("IsLogedOff"); }
        }


        public void SetData()
        {

            Password = "";
            if (this.userService.CurrentUserName != "")
            {
                CurrentUserName = userService.CurrentUser.FullName;
                IsLogedIn = true;
                IsLogedOff = false;
            }
            else
            {
                CurrentUserName = "";
                IsLogedIn = false;
                IsLogedOff = true;
            }
        }
        #endregion

        #region Dialogtextverwaltung

        private string GetErrorText(string error, string errorCode, string errorResult)
        {
            string errorText;
            if (textService == null)
                errorText = string.Format("@UserManagement.Results.{0}: {1}.{2}", error, errorCode, errorResult);
            else
                errorText = /*this.textService.GetText("@UserManagement.Results." + error) + ": " + */this.textService.GetText(string.Format("@UserManagement.Results.{0}.{1}", errorCode, errorResult));
            return errorText;
        }

        #endregion Dialogtextverwaltung
    }
}
