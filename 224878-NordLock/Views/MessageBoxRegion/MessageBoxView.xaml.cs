using HMI.Views.DialogRegion;
using System.Windows;
using VisiWin.ApplicationFramework;

namespace HMI.Views.MessageBoxRegion
{

	[ExportView("MessageBoxView")]
	public partial class MessageBoxView : VisiWin.Controls.View
	{
		public static bool messageBoxIsOpen;

		public MessageBoxView()
		{
			this.InitializeComponent();
		}

		
		public static MessageBoxResult Show(string text, string caption, MessageBoxButton type = MessageBoxButton.OK, MessageBoxResult defaultResult = MessageBoxResult.OK, MessageBoxIcon icon = MessageBoxIcon.None)
		{
            if (DialogView.dialogIsOpen) DialogView.dialogIsOpen=false;
            //	nur eine MessageBox gleichzeitig ist möglich
            if (MessageBoxView.messageBoxIsOpen) return MessageBoxResult.Cancel;
			MessageBoxView.messageBoxIsOpen = true;

			//	die Antwort wird im ObjectStore erwartet, d.h. Ergebnis vorher löschen
			ApplicationService.ObjectStore.Remove("MessageBoxView_RESULT");

			//	MessageBox zeigen, die Übergabeparameter kommen ebenfalls in den ObjectStore
			ApplicationService.SetView("MessageBoxRegion", "MessageBoxView", new DialogParams { headerText = caption, content = text, type = (InternalDialogButtons) type, defaultResult = (InternalDialogResult) defaultResult, icon = icon, modal = true });

			//	jetzt warten wir, bis das Bearbeitungsergebnis im ObjectStore gespeichert wurde
			object result = null;
			while ((result = ApplicationService.ObjectStore["MessageBoxView_RESULT"]) == null) DialogAdapter.DoEvents();

			//	ObjectStore aufräumen
			ApplicationService.ObjectStore.Remove("MessageBoxView_RESULT");
			ApplicationService.ObjectStore.Remove("MessageBoxView_KEY");

			//	MessageBox verschwinden lassen
			ApplicationService.SetView("MessageBoxRegion", "EmptyView");

			//	fertig
			MessageBoxView.messageBoxIsOpen = false;

			return (result is InternalDialogResult) ? (MessageBoxResult)result : MessageBoxResult.Cancel;
		}
	}
}