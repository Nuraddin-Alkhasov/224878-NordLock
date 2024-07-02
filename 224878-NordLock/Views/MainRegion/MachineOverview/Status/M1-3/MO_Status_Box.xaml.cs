using HMI.Views.MessageBoxRegion;
using System;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using VisiWin.Recipe;

namespace HMI.Views.MainRegion.MachineOverview
{

	[ExportView("MO_Status_Box")]
	public partial class MO_Status_Box : VisiWin.Controls.View
	{
      
        public MO_Status_Box()
		{
			this.InitializeComponent();
          
        }

    }
}