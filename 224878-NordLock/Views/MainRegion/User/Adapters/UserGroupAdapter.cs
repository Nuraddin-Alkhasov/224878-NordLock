using System.Collections.Generic;
using System.Linq;
using VisiWin.ApplicationFramework;
using VisiWin.UserManagement;
using System.Windows;
using System.ComponentModel;
using System.Collections.ObjectModel;
using VisiWin.Language;
using System.Windows.Input;
using VisiWin.Commands;
using HMI.Views.DialogRegion;
using HMI.Views.MessageBoxRegion;


namespace HMI.Views.MainRegion.User
{
	[ExportAdapter("UserGroupAdapter")]
	public class UserGroupAdapter : AdapterBase, INotifyPropertyChanged
	{
		#region Felder

		private readonly IUserManagementService userService;
		private ILanguageService textService;
	

		#endregion Felder

		#region Initialisierung

		public UserGroupAdapter()
		{
			if (ApplicationService.IsInDesignMode)
				return;

			this.userService = ApplicationService.GetService<IUserManagementService>();
			this.textService = ApplicationService.GetService<ILanguageService>();

			if (this.userService == null)
			{
				if (this.textService == null)
					this.Status = "Initialisierungsfehler Benutzerverwaltung";
				else
					this.Status = this.textService.GetText("@UserManagement.InitError");
				return;
			}

			//	Kommandos anbinden
			this.AddGroupCommand = new ActionCommand(OnAddGroupCommandExecuted);
			this.ChangeGroupCommand = new ActionCommand(OnChangeGroupCommandExecuted);
			this.RemoveGroupCommand = new ActionCommand(OnRemoveGroupCommandExecuted);

			this.GroupList = new ObservableCollection<GroupData>();
			this.RightList = new ObservableCollection<UserRightDataClass>();
		}

		protected override void OnViewLoaded(object sender, ViewLoadedEventArg e)
		{
			RefreshGroups();

			base.OnViewLoaded(sender, e);
		}

		protected override void OnViewUnloaded(object sender, ViewUnloadedEventArg e)
		{
			base.OnViewUnloaded(sender, e);
		}

		/// <summary>
		/// bringt die Gruppenliste auf den neuesten Stand
		/// </summary>
		private void RefreshGroups()
		{
			//	aktuelle Liste merken
			List<GroupData> oldGroupList = this.GroupList.ToList<GroupData>();

			//	die Neuen dazu
			foreach (string groupName in this.userService.UserGroupNames)
			{
				if (this.GroupList.Any(rd => rd.Name == groupName))
				{
					oldGroupList.RemoveAll(rd => rd.Name == groupName);
				}
				else
				{
					IUserGroupDefinition iugd = this.userService.GetUserGroupDefinition(groupName);
					if (iugd != null)
						GroupList.Add(new GroupData(iugd.Name, iugd.LocalizableText, iugd.Comment));
				}
			}

			//	die nicht mehr Vorhandenen entfernen
			foreach (GroupData group in oldGroupList)
				this.GroupList.Remove(group);
		}

		/// <summary>
		/// füllt die Rechteliste
		/// </summary>
		private void FillRights()
		{
			this.RightList.Clear();
			foreach (string right in this.userService.RightNames)
				this.RightList.Add(new UserRightDataClass(false, right, userService.GetRight(right).Text));
		}

		#endregion Initialisierung

		#region Ereignisse

		private void GroupChange()
		{
			if (this.SelectedGroup == null)
				return;

			IUserGroupDefinition ugd = this.userService.GetUserGroupDefinition(this.SelectedGroup.Name);

			//I
			//IRigtsService userService
			this.GroupName = ugd.Name;
			this.Comment = ugd.Comment;
			this.TimeToLogOff = ugd.AutoLogOffTime;
			this.MaxLogIns = ugd.MaxFailedLogOns;
			this.DaysToNewPassword = ugd.RenewPasswordInterval;
			this.UserRemovable = ugd.UsersRemovable;

			this.RightList.Clear();
			//List< IRightDefinition> aa = new List<IRightDefinition>();
			foreach (string right in this.userService.RightNames)
			{
				this.RightList.Add(new UserRightDataClass(ugd.RightNames.Contains(right), right, userService.GetRight(right).Text));
				//   aa.Add(userService.GetRightDefinition(right));
			}

		}

		#endregion Ereignisse

		#region Verwaltung

		/// <summary>
		/// Testen auf gültige Eingaben
		/// </summary>
		/// <returns></returns>
		void CheckGroupToAdd(object sender, DialogResultEventArgs e)
		{
			//	Überprüfung nur, wenn OK gedrückt
			if (e.Result != DialogResult.OK)
				return;

			//	Dialog muss bleiben
			e.CancelDialogClosing = true;

			if (!CheckGroupDefinition(false))
				return;

			IUserGroupDefinition ugd = this.userService.CreateUserGroupDefinition();
			if (ugd == null)
				return;

			ugd.UsersRemovable = this.UserRemovable;
			ugd.Name = this.GroupName;
			ugd.Comment = this.Comment;
			ugd.AutoLogOffTime = this.TimeToLogOff;
			ugd.MaxFailedLogOns = this.MaxLogIns;
			ugd.RenewPasswordInterval = this.DaysToNewPassword;
			ugd.RightNames.Clear();
			foreach (UserRightDataClass rc in this.RightList)
			{
				if (rc.On)
					ugd.RightNames.Add(rc.Right);
			}

			AddGroupSuccess success = this.userService.AddUserGroup(ugd);
			if (success != AddGroupSuccess.Succeeded)
			{
				string errorText = GetErrorText("AddGroupError", "AddGroupSuccess", success.ToString());
				new MessageBoxTask(errorText, "@UserManagement.View.AddGroup", MessageBoxIcon.Exclamation);
				return;
			}

			//	Gruppenliste aktualisieren
			this.GroupList.Add(new GroupData(ugd.Name, ugd.LocalizableText, ugd.Comment));
			SetError("AddGroupOK");

			//	Dialog muss doch nicht bleiben
			e.CancelDialogClosing = false;
		}

		void CheckGroupToChange(object sender, DialogResultEventArgs e)
		{
			//	Überprüfung nur, wenn OK gedrückt
			if (e.Result != DialogResult.OK)
				return;

			//	Dialog muss bleiben
			e.CancelDialogClosing = true;

			if (!CheckGroupDefinition(true))
				return;

			IUserGroupDefinition ugd = this.userService.GetUserGroupDefinition(GroupName);
			if (ugd == null)
				return;

			ugd.UsersRemovable = this.UserRemovable;
			ugd.Name = this.GroupName;
			ugd.Comment = this.Comment;
			ugd.AutoLogOffTime = this.TimeToLogOff;
			ugd.MaxFailedLogOns = this.MaxLogIns;
			ugd.RenewPasswordInterval = this.DaysToNewPassword;
			ugd.RightNames.Clear();
			foreach (UserRightDataClass rc in this.RightList)
			{
				if (rc.On)
					ugd.RightNames.Add(rc.Right);
			}

			ChangeGroupSuccess success = this.userService.ChangeUserGroup(ugd);
			if (success != ChangeGroupSuccess.Succeeded)
			{
				string errorText = GetErrorText("ChangeGroupError", "ChangeGroupSuccess", success.ToString());
				new MessageBoxTask(errorText, "@UserManagement.View.ChangeGroup", MessageBoxIcon.Exclamation);
				return;
			}

			//	Gruppenliste aktualisieren
			GroupData gd = this.GroupList.FirstOrDefault(rd => rd.Name == this.GroupName);
			if (gd != null)
				gd.Comment = ugd.Comment;

			SetError("ChangeGroupOK");

			//	Dialog muss doch nicht bleiben
			e.CancelDialogClosing = false;
		}

		private bool CheckGroupDefinition(bool changeGroup)
		{
			string header = (changeGroup) ? "@UserManagement.View.ChangeGroup" : "@UserManagement.View.AddGroup";

			if (string.IsNullOrEmpty(this.GroupName))
			{
				new MessageBoxTask("@UserManagement.Results.EnterGroupName", header, MessageBoxIcon.Information);
				return false;
			}

			//	Ist der Benutzer schon vorhanden?
			if (!changeGroup && this.GroupList.Any(rd => rd.Name == this.GroupName))
			{
				new MessageBoxTask("@UserManagement.Results.GroupExistError", header, MessageBoxIcon.Exclamation);
				return false;
			}

			return true;
		}

		/// <summary>
		/// Löscht die Eingaben
		/// </summary>
		private void ClearInputs()
		{
			this.GroupName = "";
			this.UserRemovable = true;
			this.TimeToLogOff = 0;
			this.DaysToNewPassword = 0;
			this.MaxLogIns = 0;
			this.Comment = "";
		}

		#endregion Verwaltung

		#region Befehle

		/// <summary>
		/// Neue Gruppe, wird.zB aus einer Action aus dem XAML File aufgerufen
		/// </summary>
		public ICommand AddGroupCommand { get; set; }
		private void OnAddGroupCommandExecuted(object parameter)
		{
			ClearError();
			ClearInputs();
			FillRights();

			//	Eingabemöglichkeiten für AddUser-Dialog initialisieren
			this.IsNameEnabled = true;
			this.UserRemovable = true;

			DialogView.Show("User_AddChangeGroup", "@UserManagement.View.AddGroup", VerifyDialogResultFunction: this.CheckGroupToAdd);
		}


		/// <summary>
		/// Gruppe  ändern, wird.zB aus einer Action aus dem XAML File aufgerufen
		/// </summary>
		public ICommand ChangeGroupCommand { get; set; }
		private void OnChangeGroupCommandExecuted(object parameter)
		{
			ClearError();
			GroupChange();

			//	Gruppe ausgewählt?
			if (string.IsNullOrEmpty(this.GroupName))
			{
				new MessageBoxTask("@UserManagement.Results.ChooseGroup", "@UserManagement.View.ChangeGroup", MessageBoxIcon.Information);
				return;
			}

			//	Eingabemöglichkeiten für ChangeUser-Dialog initialisieren
			this.IsNameEnabled = false;

			DialogView.Show("User_AddChangeGroup", "@UserManagement.View.ChangeGroup", VerifyDialogResultFunction: this.CheckGroupToChange);
		}

		/// <summary>
		/// Gruppe entfernen, wird.zB aus einer Action aus dem XAML File aufgerufen
		/// </summary>
		public ICommand RemoveGroupCommand { get; set; }
		private void OnRemoveGroupCommandExecuted(object parameter)
		{
			ClearError();

			//	Gruppe ausgewählt?
			if (this.SelectedGroup == null)
			{
				new MessageBoxTask("@UserManagement.Results.ChooseGroup", "@UserManagement.View.DeleteGroup", MessageBoxIcon.Information);
				return;
			}

			//	Sicherheitsabfrage
			if (MessageBoxView.Show("@UserManagement.Results.DeleteGroupQuery", "@UserManagement.View.DeleteGroup", MessageBoxButton.YesNo, MessageBoxResult.No, MessageBoxIcon.Question) != MessageBoxResult.Yes)
				return;

			//	dann löschen
			ChangeGroupSuccess success = this.userService.RemoveUserGroup(this.SelectedGroup.Name);
			if (success == ChangeGroupSuccess.Succeeded)
			{
				//	Gruppenliste aktualisieren
				this.GroupList.Remove(this.SelectedGroup);
				SetError("DeleteGroupOK");
			}
			else
			{
				SetError("DeleteGroupError", success.ToString());
			}
		}

		#endregion

		#region Bindungsmöglichkeiten

		private ObservableCollection<GroupData> grouplist;
		public ObservableCollection<GroupData> GroupList
		{
			get { return this.grouplist; }
			set { this.grouplist = value; OnPropertyChanged("GroupList"); }
		}

		private ObservableCollection<UserRightDataClass> rightlist;
		public ObservableCollection<UserRightDataClass> RightList
		{
			get { return this.rightlist; }
			set { this.rightlist = value; OnPropertyChanged("RightList"); }
		}

		private string groupname = "";
		public string GroupName
		{
			get { return this.groupname; }
			set { this.groupname = value; OnPropertyChanged("GroupName"); }
		}

		private bool isNameEnabled = false;
		public bool IsNameEnabled
		{
			get { return this.isNameEnabled; }
			set { this.isNameEnabled = value; OnPropertyChanged("IsNameEnabled"); }
		}

		private GroupData selectedgroup = null;
		public GroupData SelectedGroup
		{
			get { return this.selectedgroup; }
			set { this.selectedgroup = value; OnPropertyChanged("SelectedGroup"); }
		}

		private bool removable = false;
		public bool UserRemovable
		{
			get { return this.removable; }
			set { this.removable = value; OnPropertyChanged("UserRemovable"); }
		}

		private int timelogoff = 0;
		public int TimeToLogOff
		{
			get { return this.timelogoff; }
			set { this.timelogoff = value; OnPropertyChanged("TimeToLogOff"); }
		}

		private int daystonewpassword = 0;
		public int DaysToNewPassword
		{
			get { return this.daystonewpassword; }
			set { this.daystonewpassword = value; OnPropertyChanged("DaysToNewPassword"); }
		}

		private int maxlogins = 0;
		public int MaxLogIns
		{
			get { return this.maxlogins; }
			set { this.maxlogins = value; OnPropertyChanged("MaxLogIns"); }
		}

		private string comment = "";
		public string Comment
		{
			get { return this.comment; }
			set { this.comment = value; OnPropertyChanged("Comment"); }
		}

		private string status = "";
		public string Status
		{
			get { return this.status; }
			set { this.status = value; OnPropertyChanged("Status"); }
		}

		#endregion Bindungsmöglichkeiten

		#region Dialogtextverwaltung

		private void ClearError()
		{
			this.Status = "";
		}

		private void SetError(string szErrorCode)
		{
			if (this.textService == null)
				this.Status = "@UserManagement.Results." + szErrorCode;
			else
				this.Status = this.textService.GetText("@UserManagement.Results." + szErrorCode);
		}

		private void SetError(string szErrorCode, object param)
		{
			if (this.textService == null)
				this.Status = "@UserManagement.Results." + szErrorCode + ": " + param.ToString();
			else
				this.Status = this.textService.GetText("@UserManagement.Results." + szErrorCode, param);
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
	}

}
