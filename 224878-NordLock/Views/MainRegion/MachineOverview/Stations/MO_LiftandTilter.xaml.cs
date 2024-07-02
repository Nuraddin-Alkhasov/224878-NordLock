using HMI.Views.DialogRegion;
using System.Windows;
using System.Windows.Media;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;
using VisiWin.Language;

namespace HMI.Views.MainRegion.MachineOverview
{

    [ExportView("MO_LiftandTilter")]
    public partial class MO_LiftandTilter : VisiWin.Controls.View
    {

        public MO_LiftandTilter()
        {
            InitializeComponent();
            ReturnStatus = "NLM4.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Produktionsmodus.Neu befüllen.Gesperrt wegen Rückführung";
            OptimizedWeight_D = "NL.PLC.Blocks.1 Modul 1.03 Dossing conveyor.DB DC PD.Station.Feeding.BS.Weight per basket";
        }
       
        IVariableService VS = ApplicationService.GetService<IVariableService>();
        ILanguageService textService = ApplicationService.GetService<ILanguageService>();

        IVariable VWV_ReturnStatus;
        public string ReturnStatus
        {
            set
            {
                VWV_ReturnStatus = VS.GetVariable(value);
                VWV_ReturnStatus.Change += TBReturnStatus_ValueChanged;
            }
        }
        private void TBReturnStatus_ValueChanged(object sender, VariableEventArgs e)
        {
            switch ((short)e.Value)
            {
                case 1: lr.Visibility = Visibility.Visible; lr.LocalizableText = "@MainView.Text65"; break;
                case 2: lr.Visibility = Visibility.Visible; lr.LocalizableText = "@MainView.Text76"; break;
                case 3: lr.Visibility = Visibility.Visible; lr.LocalizableText = "@MainView.Text77"; break;
                default: lr.Visibility = Visibility.Hidden; break;
            }
        }
        IVariable VWV_OptimizedWeight_D;
        public string OptimizedWeight_D
        {
            set
            {
                VWV_OptimizedWeight_D = VS.GetVariable(value);
                VWV_OptimizedWeight_D.Change += VWV_OptimizedWeight_D_ValueChanged;
            }
        }
        private void VWV_OptimizedWeight_D_ValueChanged(object sender, VariableEventArgs e)
        {
            float wpb_min =  (float)e.Value;
            float wpb_max = (float)(wpb_min + (float)ApplicationService.GetVariableValue("NL.PLC.Blocks.1 Modul 1.04 Basket filling station.DB KBD HMI.Parameter.DIff Maximal Gewicht"));
            sweight_d.Value = textService.GetText("@MainView.Text80") +" : "+ wpb_min.ToString("0.0")+ " - " + wpb_max.ToString("0.0") + " " + textService.GetText("@Units.kg");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            (new SP
            {
                Module = 1,
                M1_Station = 4,
                M2_Station = 0,
                M3_Station = 0,
                M4_Station = 0,
                OvenTray = 0,
                TB_Shelve = 0,
                TB_Level = 0,
                Header = "@Status.Text25",
                Type = "BasketF"
            }).Execute();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            DialogView.Show("MO_LD_Lift", "@HandMenu.Text9", DialogButton.Close);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            DialogView.Show("MO_LD_HC", "@HandMenu.Text23", DialogButton.Close);
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            DialogView.Show("MO_LD_DC", "@HandMenu.Text21", DialogButton.Close);
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            DialogView.Show("MO_LD_BS", "@HandMenu.Text89", DialogButton.Close);
        }

    }
}



