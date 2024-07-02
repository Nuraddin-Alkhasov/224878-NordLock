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
    public partial class MV_KBDMani : UserControl
    {
        public MV_KBDMani()
        {
            InitializeComponent();
        }
        IVariableService VS = ApplicationService.GetService<IVariableService>();

        IVariable maniPos;
        public string ManipulatorPosition
        {
            set
            {
                maniPos = VS.GetVariable(value);
                maniPos.Change += maniPos_Change;
            }
        }

        private void maniPos_Change(object sender, VariableEventArgs e)
        {

            GridClear();
            switch ((short)e.Value) 
            {
                case 0:
                    ManiPosition.SymbolResourceKey = "M3Mani8";
                    B.Children.Add(GetBasket("L", new Thickness(71, 8, 0, 0),0));
                    B.Children.Add(GetBasket("R", new Thickness(105, 8, 0, 0),0));
                    break;
                case 1:
                    ManiPosition.SymbolResourceKey = "M3Mani7";
                    B.Children.Add(GetBasket("L", new Thickness(73, 60, 0, 0),1));
                    B.Children.Add(GetBasket("R", new Thickness(105, 60, 0, 0),1));
                    break;
                case 2:
                    ManiPosition.SymbolResourceKey = "M3Mani6";
                    B.Children.Add(GetBasket("L", new Thickness(26, 81, 0, 0),2));
                    B.Children.Add(GetBasket("R", new Thickness(56, 66, 0, 0),2));
                    break;
                case 3:
                    ManiPosition.SymbolResourceKey = "M3Mani5";
                    B.Children.Add(GetBasket("L", new Thickness(20, 92, 0, 0),3));
                    B.Children.Add(GetBasket("R", new Thickness(3, 116, 0, 0),3));
                    break; 
                case 4:
                    ManiPosition.SymbolResourceKey = "M3Mani4";
                    B.Children.Add(GetBasket("R", new Thickness(3, 146, 0, 0),4));
                    B.Children.Add(GetBasket("L", new Thickness(14, 174, 0, 0),4));
                   
                    break;
                case 5:
                    ManiPosition.SymbolResourceKey = "M3Mani3";
                    B.Children.Add(GetBasket("L", new Thickness(71, 211, 0, 0),5));
                    B.Children.Add(GetBasket("R", new Thickness(105, 211, 0, 0),5)); 
                    break;
                case 6:
                    ManiPosition.SymbolResourceKey = "M3Mani2";
                    B.Children.Add(GetBasket("L", new Thickness(173, 123, 0, 0),6));
                    B.Children.Add(GetBasket("R", new Thickness(173, 152, 0, 0),6));
                   
                    break;
                case 7:
                    ManiPosition.SymbolResourceKey = "M3Mani1";
                    B.Children.Add(GetBasket("L", new Thickness(235, 56, -2, 0),7));
                    B.Children.Add(GetBasket("R", new Thickness(235, 86, -2, 0),7)); break; 
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


        #region - - - Status - - -
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            (new SP
            {
                Module = 3,
                M1_Station = 0,
                M2_Station = 0,
                M3_Station = 1,
                M4_Station = 0,
                OvenTray = 0,
                CZTray = 0,
                TB_Shelve = 0,
                TB_Level = 0,
                Header = "@Status.Text26",
                Type = "Basket"
            }).Execute();

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
                CZTray = 0,
                TB_Shelve = 0,
                TB_Level = 0,
                Header = "@Status.Text25",
                Type = "BasketF"
            }).Execute();
        }

    

    #endregion
    private void GridClear() 
        {
            for (int i = B.Children.Count-1; i >=0; i--) 
            {
                B.Children.RemoveAt(i);
            }
        }
        private MVBasket GetBasket(string B, Thickness margin, int ManiPos) 
        {
        MVBasket temp = new MVBasket();
            temp.Module = 3;
            temp.M1_Station = 0;
            temp.M2_Station = 0;
            temp.M3_Station = 1;
            temp.M4_Station = 0;
            temp.OvenTray = 0;
            temp.CZTray = 0;
            temp.TB_Shelve = 0;
            temp.TB_Level = 0;
            temp.Header = "@Status.Text26";
            temp.Type = "Basket";
            switch (B) 
            {
                case "L":
                    temp.IsBasket = "NL.PLC.Blocks.3 Modul 3.08 Manipulator.00 Allgemein.DB Manipulator PD.Status.Korb.Belegt links";
                    temp.IsMaterial = "NL.PLC.Blocks.3 Modul 3.08 Manipulator.00 Allgemein.DB Manipulator PD.Status.Charge.Material vorhanden";
                    temp.ActualCoatingLayer = "NL.PLC.Blocks.3 Modul 3.08 Manipulator.00 Allgemein.DB Manipulator PD.Status.Charge.Beschichtungen.Ist";
                    if (ManiPos >= 0 && ManiPos <= 2 || ManiPos == 4)
                    {
                        temp.IsClean = "NL.PLC.Blocks.3 Modul 3.08 Manipulator.00 Allgemein.DB Manipulator PD.Status.Korb.Korb Reinigung iO";
                        temp.IsDischarge = "NL.PLC.Blocks.3 Modul 3.08 Manipulator.00 Allgemein.DB Manipulator PD.Status.Korb.Ausschleusen";

                    }
                    break;
                case "R":
                    temp.IsBasket = "NL.PLC.Blocks.3 Modul 3.08 Manipulator.00 Allgemein.DB Manipulator PD.Status.Korb.Belegt rechts";
                    temp.IsMaterial = "NL.PLC.Blocks.3 Modul 3.08 Manipulator.00 Allgemein.DB Manipulator PD.Status.Charge.Material vorhanden";
                    temp.ActualCoatingLayer = "NL.PLC.Blocks.3 Modul 3.08 Manipulator.00 Allgemein.DB Manipulator PD.Status.Charge.Beschichtungen.Ist";
                    temp.SetCoatingLayer = "NL.PLC.Blocks.3 Modul 3.08 Manipulator.00 Allgemein.DB Manipulator PD.Status.Charge.Beschichtungen.Soll";
                    if (ManiPos >= 3 && ManiPos <= 7 && ManiPos != 4)
                    {
                        temp.IsClean = "NL.PLC.Blocks.3 Modul 3.08 Manipulator.00 Allgemein.DB Manipulator PD.Status.Korb.Korb Reinigung iO";
                        temp.IsDischarge = "NL.PLC.Blocks.3 Modul 3.08 Manipulator.00 Allgemein.DB Manipulator PD.Status.Korb.Ausschleusen";

                    }
                    break;
            }

            temp.VerticalAlignment = VerticalAlignment.Top;
            temp.HorizontalAlignment = HorizontalAlignment.Left;
            temp.Margin = margin;
            return temp;
        }
    }
}
