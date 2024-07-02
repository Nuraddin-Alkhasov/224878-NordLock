using HMI.Module;
using HMI.Views.MainRegion.MachineOverview;
using HMI.Views.MainRegion.Recipe;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;

namespace HMI.UserControls
{
    public partial class TBTray : UserControl
    {
        public TBTray()
        {
            InitializeComponent();
        }

        public string IsTray
        {
            set
            {
                if (value == "1" || value == "-1")
                {
                    Task obTask = Task.Run(async () =>
                    {
                        await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                        {
                            A.BeginAnimation(UIElement.OpacityProperty, SetOpacity(1, 1));
                        });
                    });
                }
                else 
                {
                    Task obTask = Task.Run(async () =>
                    {
                        await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                        {
                            A.BeginAnimation(UIElement.OpacityProperty, SetOpacity(0, 1));
                        });
                    });
                }
            }
        }

        public string IsMaterial
        {
            set
            {
                if (value == "1" || value == "-1")
                {
                    Task obTask = Task.Run(async () =>
                    {
                        await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                        {
                            ismaterial.BeginAnimation(UIElement.OpacityProperty, SetOpacity(1, 1));
                        });
                    });
                }
                else
                {
                    Task obTask = Task.Run(async () =>
                    {
                        await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                        {
                            ismaterial.BeginAnimation(UIElement.OpacityProperty, SetOpacity(0, 1));
                        });
                    });
                }
            }
        }

        public string IsDischarge
        {
            set
            {
                if (value == "1" || value == "-1")
                {
                    discharge.Visibility = Visibility.Visible;
                }
                else
                {
                    discharge.Visibility = Visibility.Collapsed;
                }
            }
        }

        public string IsQuality
        {
            set
            {
                if (value == "1" || value == "-1")
                {
                    quality.Visibility = Visibility.Visible;
                }
                else
                {
                    quality.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void isMaterial_ValueChanged(object sender, VariableEventArgs e)
        {
            if ((bool)e.Value)
            {
                Task obTask = Task.Run(async () =>
                {
                    await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                    {
                        ismaterial.BeginAnimation(UIElement.OpacityProperty, SetOpacity(1, 1));
                    });
                });
            }
            else
            {
                Task obTask = Task.Run(async () =>
                {
                    await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                    {
                        ismaterial.BeginAnimation(UIElement.OpacityProperty, SetOpacity(0, 1));
                    });
                });
            }
        }

        public string ActualCoatingLayer
        {
            set
            {
                if (value == "0")
                {
                    tl.Background = new SolidColorBrush(Colors.White);
                    tr.Background = new SolidColorBrush(Colors.White);
                }
                else
                {
                    tl.Background = (System.Windows.Media.Brush)Application.Current.FindResource("FP_Yellow_Gradient");
                    tr.Background = (System.Windows.Media.Brush)Application.Current.FindResource("FP_Yellow_Gradient");
                }
                aCL.Value = Convert.ToDouble(value);
            }
        }

        public string SetCoatingLayer
        {
            set
            {
                sCL.Value = Convert.ToDouble(value);
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

        
        public void UpdateTrayStatus() 
        {

            DataTable DT = (new LocalDBAdapter("SELECT * " +
                                                  "FROM Tray_Buffer " +
                                                  "WHERE Coord = '" + Coord + "'; ")).DB_Output();
            if (DT.Rows.Count > 0)
            {
                VWRecipe VWR = new VWRecipe("TBStatus", DT.Rows[0]["Status"].ToString());

                IsTray = VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Tablett.Belegt").ToArray()[0].Value.ToString();
                IsMaterial = VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Charge.Material vorhanden").ToArray()[0].Value.ToString();
                ActualCoatingLayer = VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Charge.Beschichtungen.Ist").ToArray()[0].Value.ToString();
                SetCoatingLayer = VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Charge.Beschichtungen.Soll").ToArray()[0].Value.ToString();
                IsDischarge = VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Tablett.Function.Discharge").ToArray()[0].Value.ToString();
                IsQuality = VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Tablett.Function.Manuall QS").ToArray()[0].Value.ToString();
            }
        }

        public override string ToString() { return "TBTray"; }

        #region - - - Status - - -
        public string Header { set; get; }
        public string Type { set; get; }
        public int Module { set; get; }
        public int M1_Station { set; get; }
        public int M2_Station { set; get; }
        public int M3_Station { set; get; }
        public int M4_Station { set; get; }
        public int OvenTray { set; get; }
        public int CZTray { set; get; }
        public int TB_Shelve { set; get; }
        public int TB_Level { set; get; }

        public string Coord { set; get; }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (new SP
            {
                Module = Module,
                M1_Station = M1_Station,
                M2_Station = M2_Station,
                M3_Station = M3_Station,
                M4_Station = M4_Station,
                OvenTray = OvenTray,
                CZTray = CZTray,
                TB_Shelve = TB_Shelve,
                TB_Level = TB_Level,
                Header = Header,
                Type = Type
            }).Execute();
        }
        #endregion

    }
}
