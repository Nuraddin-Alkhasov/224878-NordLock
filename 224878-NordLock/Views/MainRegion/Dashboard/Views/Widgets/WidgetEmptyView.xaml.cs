using System.ComponentModel.Composition;
using VisiWin.ApplicationFramework;

namespace HMI.Dashboard
{
	/// <summary>
	/// View, die als Platzhalter für nicht vorhandene Widgets im Dashboard angezeigt wird
	/// </summary>
	[ExportView("WidgetEmptyView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
	public partial class WidgetEmptyView : VisiWin.Controls.View
	{
		public WidgetEmptyView()
		{
			this.InitializeComponent();
		}
	}
}