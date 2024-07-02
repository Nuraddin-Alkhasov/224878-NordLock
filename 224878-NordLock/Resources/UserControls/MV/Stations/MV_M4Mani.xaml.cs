using HMI.Views.MainRegion.MachineOverview;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;

namespace HMI.UserControls
{
    public partial class MV_M4Mani : UserControl
    {
        public MV_M4Mani()
        {
            InitializeComponent();
        }
        IVariableService VS = ApplicationService.GetService<IVariableService>();

        IVariable maniStatus;
        public string ManipulatorStatus
        {
            set
            {
                maniStatus = VS.GetVariable(value);
                maniStatus.Change += maniStatus_Change;
            }
        }

        private void maniStatus_Change(object sender, VariableEventArgs e)
        {

            GridClear();
            switch ((short)e.Value) 
            {
                case 0:
                    ManiPosition.SymbolResourceKey = "M4Mani1";
                    B.Children.Add(GetVTray(new Thickness(205, 86, 0, 0)));
                    break;
                case 1:
                    ManiPosition.SymbolResourceKey = "M4Mani2";
                    B.Children.Add(GetVTray(new Thickness(158, 86, 0, 0)));
                    break;
                case 2:
                    ManiPosition.SymbolResourceKey = "M4Mani3";
                    B.Children.Add(GetHTray(new Thickness(82, 141, 0, 0)));
                    break;
                case 3:
                    ManiPosition.SymbolResourceKey = "M4Mani4";
                    B.Children.Add(GetHTray(new Thickness(82, 189, 0, 0)));
                    break; 
                case 4:
                    ManiPosition.SymbolResourceKey = "M4Mani5";
                    B.Children.Add(GetHTray(new Thickness(81, 54, 0, 0)));                  
                    break;
                case 5:
                    ManiPosition.SymbolResourceKey = "M4Mani6";
                    B.Children.Add(GetVTray(new Thickness(54, 74, 0, 0)));
                    break;
                case 6:
                    ManiPosition.SymbolResourceKey = "M4Mani7";
                    B.Children.Add(GetVTray(new Thickness(0, 74, 0, 0)));                  
                    break;

            }
        }

        IVariable maniPosition;
        public string ManipulatorPosition
        {
            set
            {
                maniPosition = VS.GetVariable(value);
                maniPosition.Change += maniPosition_Change;
            }
        }
        double Oldpos = 0;
        private void maniPosition_Change(object sender, VariableEventArgs e)
        {

            double pos = Math.Round((((float)e.Value) + 134) / 18.1958);

            if (Oldpos != pos)
            {
                  Mani.Margin = new Thickness(3, 3, 10 + pos, 3);
                 Oldpos = pos;
             
            }
        
        }

        private bool loaded=false;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!loaded)
            {
                Task obTask = Task.Run(async () =>
                {
                    await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                     {
                         A.BeginAnimation(UIElement.OpacityProperty, SetOpacity(1, 1));
                     });
                });

                loaded = true;
            }
        }
        private DoubleAnimation SetOpacity(Double _O, int _T)
        {
            return new DoubleAnimation
            {
                To = _O,
                Duration = TimeSpan.FromSeconds(_T),
            };
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            (new SP
            {
                Module = 4,
                M1_Station = 0,
                M2_Station = 0,
                M3_Station = 0,
                M4_Station = 7,
                OvenTray = 0,
                CZTray = 0,
                TB_Shelve = 0,
                TB_Level = 0,
                Header = "@Status.Text105",
                Type = "Tray"
            }).Execute();
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("DialogRegion", "LD View");
        }
        private void GridClear() 
        {
            for (int i = B.Children.Count-1; i >=0; i--) 
            {
                B.Children.RemoveAt(i);
            }
        }
        private MVTrayVertical GetVTray(Thickness margin) 
        {
            MVTrayVertical temp = new MVTrayVertical() 
            {
                IsTray = "NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.00 Allgemein.DB Mani04 PD.Status.Tablett.Belegt",
                IsMaterial = "NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.00 Allgemein.DB Mani04 PD.Status.Charge.Material vorhanden",
                ActualCoatingLayer = "NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.00 Allgemein.DB Mani04 PD.Status.Charge.Beschichtungen.Ist",
                SetCoatingLayer = "NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.00 Allgemein.DB Mani04 PD.Status.Charge.Beschichtungen.Soll",
                IsDischarge = "NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.00 Allgemein.DB Mani04 PD.Status.Tablett.Function.Discharge",
                IsQuality = "NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.00 Allgemein.DB Mani04 PD.Status.Tablett.Function.Manuall QS",
                IsTilted = "NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.05 Mani Kippantrieb.DB Mani04 Palette Kippantrieb HMI.Actual value.Status Bildanzeige Gekippt"
            };
           
            temp.VerticalAlignment = VerticalAlignment.Top;
            temp.HorizontalAlignment = HorizontalAlignment.Left;
            temp.Margin = margin;
            return temp;
        }
        private MVTrayHorizontal GetHTray(Thickness margin)
        {
            MVTrayHorizontal temp = new MVTrayHorizontal()
            {
                IsTray = "NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.00 Allgemein.DB Mani04 PD.Status.Tablett.Belegt",
                IsMaterial = "NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.00 Allgemein.DB Mani04 PD.Status.Charge.Material vorhanden",
                ActualCoatingLayer = "NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.00 Allgemein.DB Mani04 PD.Status.Charge.Beschichtungen.Ist",
                SetCoatingLayer = "NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.00 Allgemein.DB Mani04 PD.Status.Charge.Beschichtungen.Soll",
                IsDischarge = "NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.00 Allgemein.DB Mani04 PD.Status.Tablett.Function.Discharge",
                IsQuality = "NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.00 Allgemein.DB Mani04 PD.Status.Tablett.Function.Manuall QS",
                IsTilted = "NLM4.PLC.Blocks.4 Modul 4.07 Manipulator.05 Mani Kippantrieb.DB Mani04 Palette Kippantrieb HMI.Actual value.Status Bildanzeige Gekippt"
               
            };

            temp.VerticalAlignment = VerticalAlignment.Top;
            temp.HorizontalAlignment = HorizontalAlignment.Left;
            temp.Margin = margin;
            return temp;
        }
    }
}
