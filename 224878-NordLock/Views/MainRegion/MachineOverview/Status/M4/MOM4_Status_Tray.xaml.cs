using HMI.Views.MessageBoxRegion;
using System;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using VisiWin.Recipe;

namespace HMI.Views.MainRegion.MachineOverview
{

	[ExportView("MOM4_Status_Tray")]
	public partial class MOM4_Status_Tray : VisiWin.Controls.View
	{
      
        public MOM4_Status_Tray()
		{
			this.InitializeComponent();
          
        }

    }
}