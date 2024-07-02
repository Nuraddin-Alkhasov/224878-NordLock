using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using VisiWin.ApplicationFramework;
using System.Windows;
using VisiWin.UserManagement;
using VisiWin.Language;
using System.Windows.Input;
using VisiWin.Commands;
using HMI.Views.DialogRegion;
using HMI.Views.MessageBoxRegion;

namespace HMI.Views.MainRegion.User
{
	[ExportAdapter("UserAdapter")]
	public class UserAdapter : AdapterBase, INotifyPropertyChanged
	{
		#region Felder

		private IUserManagementService userService;
		private ILanguageService textService;

		#endregion Felder

		#region Initialisierung

		public UserAdapter()
		{
			if (ApplicationService.IsInDesignMode)
				return;

			this.userService = ApplicationService.GetService<IUserManagementService>();
			this.textService = ApplicationService.GetService<ILanguageService>();

			//	Kommandos anbinden
			this.AddUserCommand = new ActionCommand(OnAddUserCommandExecuted);
			this.ChangeUserCommand = new ActionCommand(OnChangeUserCommandExecuted);
			this.RemoveUserCommand = new ActionCommand(OnRemoveUserCommandExecuted);
			this.ChangePasswordCommand = new ActionCommand(OnChangePasswordCommandExecuted);

			this.UserList = new ObservableCollection<UserData>();

			this.GroupList = new ObservableCollection<string>();


			this.StateList = new ObservableCollection<UserState>() { UserState.Active, UserState.Deactivated, UserState.Invalidated, UserState.PasswordExpired };


		}

		protected override void OnViewLoaded(object sender, ViewLoadedEventArg e)
		{
			RefreshUserNames();
			base.OnViewLoaded(sender, e);
		}


		private void RefreshUserNames()
		{
			UserList.Clear();

			foreach (string userName in this.userService.UserNames)
			{
				IUserDefinition iud = this.userService.GetUserDefinition(userName);
				UserList.Add(new UserData(iud.Name, iud.FullName, iud.UserState, iud.GroupName, iud.Comment, iud.DeactivationTime));
			}
		}


		private void RefreshUserGroups()
		{
			GroupList.Clear();

			foreach (string group in this.userService.UserGroupNames)
			{
				GroupList.Add(group);
			}
		}

		#endregion Initialisierung

		#region Dialogfunktionen

		void CheckUserToAdd(object sender, DialogResultEventArgs e)
		{
			//	Überprüfung nur, wenn OK gedrückt
			if (e.Result != DialogResult.OK)
				return;

			//	Dialog muss bleiben
			e.CancelDialogClosing = true;

			if (!CheckUserDefinition(false))
				return;

			//	Benutzer neu anlegen
			IUserDefinition ud = this.userService.CreateUserDefinition();
			ud.Name = this.UserName;
			ud.FullName = this.UserFullName;
			ud.Comment = this.Comment;
			ud.GroupName = this.SelectedGroup;
			ud.InitialPassword = this.Password;
			ud.Code = this.MachineCode;
			ud.UserState = this.SelectedState;

			if (this.DeackNever)
			{
				ud.DeactivationTime = DateTime.FromOADate(0);
			}
			else if (this.DeackDate)
			{
				ud.DeactivationTime = Date;
			}
			else if (this.DeackTime)
			{
				TimeSpan ts = new TimeSpan(Convert.ToInt32(this.Days), Convert.ToInt32(this.Hours), Convert.ToInt32(this.Minutes), 0);
				ud.DeactivationTime = DateTime.FromOADate(ts.TotalDays);
			}

			AddUserSuccess success = this.userService.AddUser(ud);
			if (success != AddUserSuccess.Succeeded)
			{
				string errorText = GetErrorText("AddUserError", "AddUserSuccess", success.ToString());
				new MessageBoxTask(errorText, "@UserManagement.View.AddUser", MessageBoxIcon.Exclamation);
				return;
			}

			//	Anlegen erfolgreich
			this.UserList.Add(new UserData(ud.Name, ud.FullName, ud.UserState, ud.GroupName, ud.Comment, ud.DeactivationTime));

			SetError("AddUserOK");

			//	Dialog muss doch nicht bleiben
			e.CancelDialogClosing = false;
		}

		void CheckUserToChange(object sender, DialogResultEventArgs e)
		{
			//	Überprüfung nur, wenn OK gedrückt
			if (e.Result != DialogResult.OK)
				return;

			//	Dialog muss bleiben
			e.CancelDialogClosing = true;

			if (!CheckUserDefinition(true))
				return;

			IUserDefinition ud = this.userService.GetUserDefinition(this.UserName);
			ud.GroupName = this.SelectedGroup;
			if (this.IsPasswordChecked)
			{
				ud.InitialPassword = this.Password;
			}
			else
			{
				ud.InitialPassword = null;
			}
			if (this.IsCodeChecked)
				ud.Code = this.MachineCode;
			else
				ud.Code = null;
			ud.UserState = this.SelectedState;

			if (this.DeackNever)
			{
				ud.DeactivationTime = DateTime.FromOADate(0);
			}
			else if (this.DeackDate)
			{
				ud.DeactivationTime = Date;
			}
			else if (this.DeackTime)
			{
				TimeSpan ts = new TimeSpan(Convert.ToInt32(this.Days), Convert.ToInt32(this.Hours), Convert.ToInt32(this.Minutes), 0);
				ud.DeactivationTime = DateTime.FromOADate(ts.TotalDays);
			}

			ChangeUserSuccess success = this.userService.ChangeUser(ud);
			if (success != ChangeUserSuccess.Succeeded)
			{
				string errorText = GetErrorText("ChangeUserError", "ChangeUserSuccess", success.ToString());
				new MessageBoxTask(errorText, "@UserManagement.View.ChangeUser", MessageBoxIcon.Exclamation);
				return;
			}

			//	Benutzerliste aktualisieren
			UserData uda = this.UserList.FirstOrDefault(rd => rd.Name == this.UserName);
			if (uda != null)
			{
				uda.Group = ud.GroupName;
				uda.State = ud.UserState;
				uda.Comment = ud.Comment;
			}

			SetError("ChangeUserOK");

			//	Dialog muss doch nicht bleiben
			e.CancelDialogClosing = false;
		}

		bool CheckUserDefinition(bool changeUser)
		{
			string header = (changeUser) ? "@UserManagement.View.ChangeUser" : "@UserManagement.View.AddUser";

			if (string.IsNullOrEmpty(this.UserName))
			{
				new MessageBoxTask("@UserManagement.Results.EnterUserName", header, MessageBoxIcon.Information);
				return false;
			}

			//	Ist der Benutzer schon vorhanden?
			if (!changeUser && this.UserList.Any(rd => rd.Name == this.UserName))
			{
				new MessageBoxTask("@UserManagement.Results.UserExistError", header, MessageBoxIcon.Exclamation);
				return false;
			}

			//	Maschinencode schon vorhanden ?
			if (this.IsCodeChecked && !CheckCode(this.MachineCode))
			{
				new MessageBoxTask("@UserManagement.Results.CodeExistError", header, MessageBoxIcon.Exclamation);
				return false;
			}

			//	Gruppe gewählt?
			if (string.IsNullOrEmpty(this.SelectedGroup))
			{
				new MessageBoxTask("@UserManagement.Results.EnterGroupName", header, MessageBoxIcon.Information);
				return false;
			}

			//	Password vorhanden?
			if (this.IsPasswordChecked)
			{
				if (string.IsNullOrEmpty(this.Password))
				{
					new MessageBoxTask("@UserManagement.Results.EnterOldPassword", header, MessageBoxIcon.Information);
					return false;
				}

				//	Passwordwiederholung vorhanden?
				if (string.IsNullOrEmpty(this.Password2))
				{
					new MessageBoxTask("@UserManagement.Results.EnterPassword", header, MessageBoxIcon.Information);
					return false;
				}

				//	Passwörter ungleich?
				if (this.Password != this.Password2)
				{
					new MessageBoxTask("@UserManagement.Results.EnterSamePassword", header, MessageBoxIcon.Information);
					return false;
				}
			}

			//	Status gewählt?
			if (this.SelectedState == UserState.Unknown)
			{
				new MessageBoxTask("@UserManagement.Results.ChooseState", header, MessageBoxIcon.Information);
				return false;
			}

			//	Datum eingeben?
			if (this.DeackDate && string.IsNullOrEmpty(this.Date.ToString()))
			{
				new MessageBoxTask("@UserManagement.Results.EnterDate", header, MessageBoxIcon.Information);
				return false;
			}

			return true;
		}

		void CheckPasswords(object sender, DialogResultEventArgs e)
		{
			//	Anmelden oder Abmelden?
			if (e.Result != DialogResult.OK)
				return;

			//	Dialog muss bleiben
			e.CancelDialogClosing = true;

			//	kein altes Passwort eingegeben?
			if (string.IsNullOrEmpty(this.OldPassword))
			{
				new MessageBoxTask("@UserManagement.Results.EnterOldPassword", "@UserManagement.View.ChangePassword", MessageBoxIcon.Information);
				return;
			}

			//	kein neues Passwort eingegeben?
			if (string.IsNullOrEmpty(this.Password))
			{
				new MessageBoxTask("@UserManagement.Results.EnterPassword", "@UserManagement.View.ChangePassword", MessageBoxIcon.Information);
				return;
			}

			//	Passwordwiederholung vorhanden?
			if (string.IsNullOrEmpty(this.Password2))
			{
				new MessageBoxTask("@UserManagement.Results.EnterRepeatedPassword", "@UserManagement.View.ChangePassword", MessageBoxIcon.Information);
				return;
			}

			//	Passwörter ungleich?
			if (this.Password != this.Password2)
			{
				new MessageBoxTask("@UserManagement.Results.EnterSamePassword", "@UserManagement.View.ChangePassword", MessageBoxIcon.Information);
				return;
			}

			//	Passwort ändern
			ChangePasswordSuccess success = this.userService.ChangeUserPassword(this.UserName, this.Password, this.OldPassword);
			if (success != ChangePasswordSuccess.Succeeded)
			{
				string errorText = GetErrorText("ChangePasswordError", "ChangePasswordSuccess", success.ToString());
				new MessageBoxTask(errorText, "@UserManagement.View.ChangePassword", MessageBoxIcon.Exclamation);
				return;
			}

			//	Anlegen erfolgreich
			SetError("ChangePasswordOK");

			//	fertig, Dialog muss doch nicht bleiben
			e.CancelDialogClosing = false;
		}

		private bool CheckCode(string code)
		{
			if (string.IsNullOrEmpty(code))
				return true;

			foreach (string user in this.userService.UserNames)
			{
				IUserDefinition ud = this.userService.GetUserDefinition(user);
				if (ud.Code == code)
					return false;
			}

			return true;
		}

		#endregion Dialogfunktionen

		#region Verwaltung

		/// <summary>
		/// Belegt die Eingabefelder nach Auswahl des Benutzers
		/// </summary>
		private void GetUser(string user)
		{
			IUserDefinition ud = this.userService.GetUserDefinition(user);
			if (ud == null)
				return;

			this.UserName = ud.Name;
			this.UserFullName = ud.FullName;
			this.Comment = ud.Comment;
			this.MachineCode = ud.Code;
			this.SelectedGroup = ud.GroupName;

			//	Passwörter ermitteln
			this.OldPassword = "";
			this.Password = "";
			this.Password2 = "";

			//	Status setzen
			this.SelectedState = ud.UserState;

			//	Deaktivierung setzen
			this.DeackNever = false;
			this.DeackDate = false;
			this.DeackTime = false;
			this.Date = DateTime.Now;
			this.Days = "0";
			this.Hours = "0";
			this.Minutes = "0";

			double deactivationTime = ud.DeactivationTime.ToOADate();
			if (deactivationTime == 0)
			{
				this.DeackNever = true;
			}
			else if (deactivationTime > 25000.0)
			{
				this.Date = ud.DeactivationTime;
				this.DeackDate = true;
			}
			else
			{
				TimeSpan ts = TimeSpan.FromDays(deactivationTime);
				this.Days = ts.Days.ToString();
				this.Hours = ts.Hours.ToString();
				this.Minutes = ts.Minutes.ToString();
				this.DeackTime = true;
			}
		}

		/// <summary>
		/// Löscht die Eingaben
		/// </summary>
		private void ClearInputs()
		{
			this.UserName = "";
			this.UserFullName = "";
			this.Comment = "";
			this.MachineCode = "";
			this.OldPassword = "";
			this.Password = "";
			this.Password2 = "";
			this.DeackNever = true;
			this.DeackDate = false;
			this.DeackTime = false;
			this.Date = DateTime.Now;
			this.Hours = "0";
			this.Minutes = "0";
			this.Days = "0";
		}

		#endregion Verwaltung

		#region Dialogtextverwaltung

		private void ClearError()
		{
			this.Status = "";
		}

		private void SetError(string error)
		{
			if (this.textService == null)
				this.Status = "@UserManagement.Results." + error;
			else
				this.Status = this.textService.GetText("@UserManagement.Results." + error);
		}

		private void SetError(string error, object param)
		{
			if (this.textService == null)
				this.Status = string.Format("@UserManagement.Results.{0}: {1}", error, param);
			else
				this.Status = this.textService.GetText("@UserManagement.Results." + error, param);
		}

		private string GetErrorText(string error, string errorCode, string errorResult)
		{
			string errorText;
			if (textService == null)
				errorText = string.Format("@UserManagement.Results.{0}: {1}.{2}", error, errorCode, errorResult);
			else
				errorText = this.textService.GetText("@UserManagement.Results." + error) + ": " + textService.GetText(string.Format("@UserManagement.Results.{0}.{1}", errorCode, errorResult));
			return errorText;
		}

		#endregion Dialogtextverwaltung

		#region Befehle

		/// <summary>
		/// Benutzer hinzufügen
		/// </summary>
		public ICommand AddUserCommand { get; set; }
		private void OnAddUserCommandExecuted(object parameter)
		{
			ClearError();
			RefreshUserGroups();
			RefreshUserNames();
			ClearInputs();

			this.SelectedGroup = this.GroupList[0];
			this.SelectedState = this.StateList[0];
			//	Eingabemöglichkeiten für AddUser-Dialog initialisieren
			this.IsNameEnabled = true;
			this.IsCodeChecked = true;
			this.IsPasswordChecked = true;
			this.UserChangeVisibility = Visibility.Hidden;

			DialogView.Show("User_AddChangeUser", "@UserManagement.View.AddUser", VerifyDialogResultFunction: this.CheckUserToAdd);
		}

		/// <summary>
		/// Benutzer ändern
		/// </summary>
		public ICommand ChangeUserCommand { get; set; }
		private void OnChangeUserCommandExecuted(object parameter)
		{
			ClearError();
			RefreshUserGroups();
			this.SelectedGroup = this.GroupList[0];
			this.SelectedState = this.StateList[0];

			//	Benutzer einstellen
			if (this.SelectedUser != null)
				GetUser(this.SelectedUser.Name);

			//	Benutzer ausgewählt?
			if (string.IsNullOrEmpty(this.UserName))
			{
				new MessageBoxTask("@UserManagement.Results.ChooseUser", "@UserManagement.View.ChangeUser", MessageBoxIcon.Information);
				return;
			}

			//	Eingabemöglichkeiten für AddUser-Dialog initialisieren
			this.IsNameEnabled = false;
			this.IsCodeChecked = false;
			this.IsPasswordChecked = false;
			this.UserChangeVisibility = Visibility.Visible;

			DialogView.Show("User_AddChangeUser", "@UserManagement.View.ChangeUser", VerifyDialogResultFunction: this.CheckUserToChange);
		}

		/// <summary>
		/// Benutzer entfernen
		/// </summary>
		public ICommand RemoveUserCommand { get; set; }
		private void OnRemoveUserCommandExecuted(object parameter)
		{
			ClearError();

			//	Benutzer ausgewählt?
			if (this.SelectedUser == null)
			{
				new MessageBoxTask("@UserManagement.Results.ChooseUser", "@UserManagement.View.DeleteUser", MessageBoxIcon.Information);
				return;
			}

			//	dürfen Benutzer gelöscht werden?
			IUserDefinition ud = this.userService.GetUserDefinition(this.SelectedUser.Name);
			if (ud == null)
				return;

			IUserGroupDefinition ugd = this.userService.GetUserGroupDefinition(ud.GroupName);
			if (ugd == null)
				return;
			if (!ugd.UsersRemovable)
			{
				new MessageBoxTask("@UserManagement.Results.DeleteUserProhibited", "@UserManagement.View.DeleteUser", MessageBoxIcon.Exclamation);
				return;
			}

			//	Sicherheitsabfrage
			if (MessageBoxView.Show("@UserManagement.Results.DeleteUserQuery", "@UserManagement.View.DeleteUser", MessageBoxButton.YesNo, MessageBoxResult.No, MessageBoxIcon.Question) != MessageBoxResult.Yes)
				return;

			//	dann löschen
			bool success = this.userService.RemoveUser(this.SelectedUser.Name);
			if (success)
			{
				//	Benutzerliste aktualisieren
				UserList.Remove(this.SelectedUser);
				SetError("DeleteUserOK");
			}
			else
			{
				SetError("DeleteUserError", success.ToString());
			}
		}

		/// <summary>
		/// Passwort ändern
		/// </summary>
		public ICommand ChangePasswordCommand { get; set; }
		private void OnChangePasswordCommandExecuted(object parameter)
		{
			ClearError();

			//	Benutzer ausgewählt?
			if (this.SelectedUser == null)
			{
				new MessageBoxTask("@UserManagement.Results.ChooseUser", "@UserManagement.View.ChangePassword", MessageBoxIcon.Information);
				return;
			}

			//	Benutzername wird übergeben?
			if (parameter is string)
				ShowChangePasswordDialog((string)parameter);
			else
				ShowChangePasswordDialog();
		}

		/// <summary>
		/// Dialog zum Passwort ändern anzeigen
		/// </summary>
		/// <param name="user">Benutzer (optional)</param>
		/// <returns>erfolgreich oder nicht</returns>
		public bool ShowChangePasswordDialog(string user = null)
		{
			//	Benutzername wird übergeben?
			if (string.IsNullOrEmpty(user))
			{
				if (this.SelectedUser == null)
					return false;
				user = this.SelectedUser.Name;
			}

			//	Benutzerinformationen auffüllen
			GetUser(user);

			//	Benutzer ausgewählt?
			if (string.IsNullOrEmpty(this.UserName))
			{
				new MessageBoxTask("@UserManagement.Results.ChooseUser", "@UserManagement.View.ChangePassword", MessageBoxIcon.Information);
				return false;
			}

			//	Eingaben vorbelegen
			this.OldPassword = "";
			this.Password = "";
			this.Password2 = "";

			DialogResult result = DialogView.Show("User_ChangePassword", "@UserManagement.View.ChangePassword", DialogButton.OKCancel, VerifyDialogResultFunction: this.CheckPasswords);
			return (result == DialogResult.OK);
		}

		#endregion

		#region Bindungsmöglichkeiten

		private string username = "";
		public string UserName
		{
			get { return this.username; }
			set { this.username = value; OnPropertyChanged("UserName"); }
		}

		private string userfullname = "";
		public string UserFullName
		{
			get { return this.userfullname; }
			set { this.userfullname = value; OnPropertyChanged("UserFullName"); }
		}

		private string comment = "";
		public string Comment
		{
			get { return this.comment; }
			set { this.comment = value; OnPropertyChanged("Comment"); }
		}

		private Visibility userChangeVisibility = Visibility.Hidden;
		public Visibility UserChangeVisibility
		{
			get { return this.userChangeVisibility; }
			set { this.userChangeVisibility = value; OnPropertyChanged("UserChangeVisibility"); }
		}

		private bool isNameEnabled = false;
		public bool IsNameEnabled
		{
			get { return this.isNameEnabled; }
			set { this.isNameEnabled = value; OnPropertyChanged("IsNameEnabled"); }
		}

		private bool isCodeChecked = false;
		public bool IsCodeChecked
		{
			get { return this.isCodeChecked; }
			set { this.isCodeChecked = value; OnPropertyChanged("IsCodeChecked"); }
		}

		private bool isPasswordChecked = false;
		public bool IsPasswordChecked
		{
			get { return this.isPasswordChecked; }
			set { this.isPasswordChecked = value; OnPropertyChanged("IsPasswordChecked"); }
		}

		private string code = "";
		public string MachineCode
		{
			get { return this.code; }
			set { this.code = value; OnPropertyChanged("MachineCode"); }
		}

		private ObservableCollection<UserData> userlist;
		public ObservableCollection<UserData> UserList
		{
			get { return this.userlist; }
			set { this.userlist = value; OnPropertyChanged("UserList"); }
		}

		private ObservableCollection<string> grouplist;
		public ObservableCollection<string> GroupList
		{
			get { return this.grouplist; }
			set { this.grouplist = value; OnPropertyChanged("GroupList"); }
		}

		private string oldPassword;
		public string OldPassword
		{
			get { return this.oldPassword; }
			set { this.oldPassword = value; OnPropertyChanged("OldPassword"); }
		}

		private string password;
		public string Password
		{
			get { return this.password; }
			set { this.password = value; OnPropertyChanged("Password"); }
		}

		private string password2;
		public string Password2
		{
			get { return this.password2; }
			set { this.password2 = value; OnPropertyChanged("Password2"); }
		}

		private ObservableCollection<UserState> stateList;
		public ObservableCollection<UserState> StateList
		{
			get { return this.stateList; }
			set { this.stateList = value; OnPropertyChanged("StateList"); }
		}

		private string status = "";
		public string Status
		{
			get { return this.status; }
			set { this.status = value; OnPropertyChanged("Status"); }
		}

		private bool deacknever = true;
		public bool DeackNever
		{
			get { return this.deacknever; }
			set { this.deacknever = value; OnPropertyChanged("DeackNever"); }
		}

		private bool deackdate = false;
		public bool DeackDate
		{
			get { return this.deackdate; }
			set { this.deackdate = value; OnPropertyChanged("DeackDate"); }
		}

		private bool deacktime = false;
		public bool DeackTime
		{
			get { return this.deacktime; }
			set { this.deacktime = value; OnPropertyChanged("DeackTime"); }
		}
		private DateTime date = DateTime.Now;
		public DateTime Date
		{
			get { return this.date; }
			set { this.date = value; OnPropertyChanged("Date"); }
		}

		private string days = "0";
		public string Days
		{
			get { return this.days; }
			set { this.days = value; OnPropertyChanged("Days"); }
		}

		private string hours = "0";
		public string Hours
		{
			get { return this.hours; }
			set { this.hours = value; OnPropertyChanged("Hours"); }
		}

		private string minutes = "0";
		public string Minutes
		{
			get { return this.minutes; }
			set { this.minutes = value; OnPropertyChanged("Minutes"); }
		}

		private UserData selectedUser = null;
		public UserData SelectedUser
		{
			get { return this.selectedUser; }
			set
			{
				this.selectedUser = value;
				OnPropertyChanged("SelectedUser");
			}
		}

		private string selectedgroup = "";
		public string SelectedGroup
		{
			get { return this.selectedgroup; }
			set { this.selectedgroup = value; OnPropertyChanged("SelectedGroup"); }
		}

		private UserState selectedstate = UserState.Unknown;
		public UserState SelectedState
		{
			get { return this.selectedstate; }
			set { this.selectedstate = value; OnPropertyChanged("SelectedState"); }
		}

		#endregion Bindungsmöglichkeiten
	}


}
