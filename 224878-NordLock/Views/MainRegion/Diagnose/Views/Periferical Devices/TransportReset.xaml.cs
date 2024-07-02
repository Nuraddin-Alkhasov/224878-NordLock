using System;
using HMI.Interfaces;
using HMI.Module;
using HMI.Services;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;

namespace HMI.Diagnose
{
	/// <summary>
	/// Interaction logic for UserAdd.xaml
	/// </summary>
    [ExportView("TransportReset")]
	public partial class TransportReset : VisiWin.Controls.View, IView
	{
       

        public TransportReset()
		{
			this.InitializeComponent();
        }

   
      
    }
}