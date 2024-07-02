using HMI.Views.DialogRegion;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using VisiWin.DataAccess;

namespace HMI.Views.MainRegion.MachineOverview
{

    [ExportView("MO_QS")]
    public partial class MO_QS : VisiWin.Controls.View
    {

        public MO_QS()
        {

            InitializeComponent();
        }

        IVariableService VS = ApplicationService.GetService<IVariableService>();

        IVariable qsdoor1Status;
        public string QSDoor1Status
        {
            set
            {
                qsdoor1Status = VS.GetVariable(value);
                qsdoor1Status.Change += qsdoor1Status_ValueChanged;
            }
        }

        private void qsdoor1Status_ValueChanged(object sender, VariableEventArgs e)
        {
            if ((short)e.Value == 1 || (short)e.Value == 2)
            {
                QSDoor1.BeginAnimation(ContentControl.MarginProperty, SetMargin(new Thickness(QSDoor1.Margin.Left, 366, 0, 0), new Thickness(529, 366, 0, 0), 1));

            }
            else
            {
                QSDoor1.BeginAnimation(ContentControl.MarginProperty, SetMargin(new Thickness(QSDoor1.Margin.Left, 366, 0, 0), new Thickness(311, 366, 0, 0), 1));
            }
        }


        IVariable qsdoor2Status;
        public string QSDoor2Status
        {
            set
            {
                qsdoor2Status = VS.GetVariable(value);
                qsdoor2Status.Change += qsdoor2Status_ValueChanged;
            }
        }

        private void qsdoor2Status_ValueChanged(object sender, VariableEventArgs e)
        {
            if ((short)e.Value==1 || (short)e.Value==2)
            {
                QSDoor2.BeginAnimation(ContentControl.MarginProperty, SetMargin(new Thickness(QSDoor2.Margin.Left, 500, 0, 0), new Thickness(529, 500, 0, 0), 1));

            }
            else
            {
                QSDoor2.BeginAnimation(ContentControl.MarginProperty, SetMargin(new Thickness(QSDoor2.Margin.Left, 500, 0, 0), new Thickness(311, 500, 0, 0), 1));
            }
        }

        private ThicknessAnimation SetMargin(Thickness _From, Thickness _To, int _T)
        {
            return new ThicknessAnimation
            {
                From = _From,
                To = _To,
                Duration = TimeSpan.FromSeconds(_T),
            };
        }

        private void QSDoor1_Loaded(object sender, RoutedEventArgs e)
        {
            QSDoor1Status = "NLM4.PLC.Blocks.4 Modul 4.10 Qualität.00 Allgemein.DB Qualität Allgemein HMI.Actual value.Status Türe 1 oben";

        }

        private void QSDoor2_Loaded(object sender, RoutedEventArgs e)
        {
            QSDoor2Status = "NLM4.PLC.Blocks.4 Modul 4.10 Qualität.00 Allgemein.DB Qualität Allgemein HMI.Actual value.Status Türe 2 unten";

        }
    }
}



