using HMI.Views.MessageBoxRegion;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;
using VisiWin.Recipe;

namespace HMI.Views.MainRegion.MachineOverview
{

	[ExportView("MO_DO_PStatus")]
	public partial class MO_DO_PStatus : VisiWin.Controls.View
	{

        public MO_DO_PStatus()
		{
			this.InitializeComponent();
        }
        
    }
}