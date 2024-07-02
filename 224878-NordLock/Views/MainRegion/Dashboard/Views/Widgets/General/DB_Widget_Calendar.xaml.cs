﻿using System.ComponentModel.Composition;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;


namespace HMI.Dashboard
{
    /// <summary>
    /// Interaction logic for DashboardWidgetBar.xaml
    /// </summary>
    [ExportDashboardWidget("DB_Widget_Calendar", "Dashboard.Text2", "@Dashboard.Text12", 1, 1)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class DB_Widget_Calendar : View
    {

        public DB_Widget_Calendar()
        {
            this.InitializeComponent();
        }


    }
}