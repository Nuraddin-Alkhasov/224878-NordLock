using HMI.Module;
using HMI.UserControls;
using HMI.Views.MainRegion.Recipe;
using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Views.MainRegion.MachineOverview
{

    [ExportView("MO_TrayBuffer")]
    public partial class MO_TrayBuffer : VisiWin.Controls.View
    {

        public MO_TrayBuffer()
        {
            InitializeComponent();
        }

        public void Trays_Loaded(object sender, RoutedEventArgs e)
        {
            BackgroundWorker BGW = new BackgroundWorker();
            BGW.DoWork += BGW_DoWorkAsync;
            BGW.RunWorkerCompleted += BGW_RunWorkerCompleted;
            BGW.RunWorkerAsync(new BufferTrayPosition());
        }

      
        BufferTrayPosition BTP;
        private void BGW_DoWorkAsync(object sender, DoWorkEventArgs e)
        {
            BTP = (BufferTrayPosition)e.Argument;
            
            Dispatcher.InvokeAsync(delegate
            {
                if (this.IsVisible) 
                {
                    if (BTP.Shelve == null)
                    {
                        BTP = new BufferTrayPosition("A", 0);
                    }
                    BTP.NextPosition = BTP.GetNext();
                    BTP.TBT.Margin = BTP.Margin;
                    BTP.TBT.HorizontalAlignment = HorizontalAlignment.Left;
                    BTP.TBT.VerticalAlignment = VerticalAlignment.Top;
                }
            });
        }

        private void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
         //   Dispatcher.InvokeAsync((Action)delegate
          //  {
                Trays.Children.Add(BTP.TBT);
          //  });
            

            if (BTP.NextPosition != null) 
            {
                BackgroundWorker BGW = new BackgroundWorker();
                BGW.DoWork += BGW_DoWorkAsync;
                BGW.RunWorkerCompleted += BGW_RunWorkerCompleted;

                BGW.RunWorkerAsync(BTP.NextPosition);
            }
           

        }

       

        public void Trays_Unloaded(object sender, RoutedEventArgs e)
        {
            Task obTask = Task.Run(async () =>
            {
                await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                {
                    Trays.Children.Clear();
                });
            });

        }

        private void Priority_Click(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("DialogRegion", "MO_SetOrderPriority");
        }

        public TBTray GetTBTray(string _shelve, string _level) 
        {
            foreach (TBTray temp in Trays.Children) 
            {
                if (temp.TB_Shelve.ToString() == _shelve && temp.TB_Level.ToString() == _level) 
                {
                    return temp;
                }
            }
            return new TBTray();
        }
        public TBTray GetTBTray(string _coord)
        {
            foreach (TBTray temp in Trays.Children)
            {
                if (temp.Coord == _coord)
                {
                    return temp;
                }
            }
            return new TBTray();
        }

    }
    public class BufferTrayPosition
    {
        public BufferTrayPosition() { }
        public BufferTrayPosition(string _shelve, int _i)
        {
            Shelve = _shelve;
            Level = _i;
            Margin = GetShelveMargin(_shelve);
            Margin = new Thickness(Margin.Left, Margin.Top - 35 * Level, 0, 0);
            ShelveId = GetShelveId(_shelve);
            TBT = GetTray();
        }
        public string Shelve { set; get; }
        public int ShelveId { set; get; }
        public int Level { set; get; }
        public Thickness Margin { set; get; }
        public TBTray TBT { set; get; }
        public int GetShelveId(string s)
        {
            switch (s)
            {
                case "A": return 0;
                case "B": return 1;
                case "C": return 2;
                case "D": return 3;
                case "E": return 4;
                case "F": return 5;
                case "G": return 6;
                case "H": return 7;
                case "I": return 8;
                case "J": return 9;
                case "K": return 10;
                case "L": return 11;
                default: return 0;
            }
        }

        private Thickness GetShelveMargin(string s)
        {
            switch (s)
            {
                case "A": return new Thickness(145, 293, 0, 0);
                case "B": return new Thickness(145, 706, 0, 0);
                case "C": return new Thickness(415, 293, 0, 0);
                case "D": return new Thickness(415, 706, 0, 0);
                case "E": return new Thickness(685, 293, 0, 0);
                case "F": return new Thickness(685, 706, 0, 0);
                case "G": return new Thickness(955, 293, 0, 0);
                case "H": return new Thickness(955, 706, 0, 0);
                case "I": return new Thickness(1224, 293, 0, 0);
                case "J": return new Thickness(1224, 706, 0, 0);
                case "K": return new Thickness(1494, 293, 0, 0);
                case "L": return new Thickness(1494, 706, 0, 0);
                default: return new Thickness(0, 0, 0, 0);
            }
        }

        public TBTray GetTray()
        {

            DataTable DT = (new LocalDBAdapter("SELECT * " +
                                               "FROM Tray_Buffer " +
                                               "WHERE Coord = '" + Shelve + Level.ToString() + "'; ")).DB_Output();
            TBTray temp = new TBTray();
            if (DT.Rows.Count > 0)
            {
                VWRecipe VWR = new VWRecipe("TBStatus", DT.Rows[0]["Status"].ToString());

                temp = new TBTray()
                {
                    IsTray = VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Tablett.Belegt").ToArray()[0].Value.ToString(),
                    IsMaterial = VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Charge.Material vorhanden").ToArray()[0].Value.ToString(),
                    ActualCoatingLayer = VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Charge.Beschichtungen.Ist").ToArray()[0].Value.ToString(),
                    SetCoatingLayer = VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Charge.Beschichtungen.Soll").ToArray()[0].Value.ToString(),
                    IsDischarge= VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Tablett.Function.Discharge").ToArray()[0].Value.ToString(),
                    IsQuality= VWR.VWVariables.Where(x => (string)x.Item == "NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.TB.Status.Tablett.Function.Manuall QS").ToArray()[0].Value.ToString(),
                    Module = 4,
                    M4_Station = 6,
                    TB_Shelve = ShelveId,
                    TB_Level = Level,
                    Coord = Shelve+ Level.ToString(),
                    Type = "TrayTB",
                };
                temp.Name = Shelve + Level;
            }

            return temp;
        }

        public BufferTrayPosition NextPosition { get; set; }
        public BufferTrayPosition GetNext() 
        {
            char _Shelve = Convert.ToChar(Shelve);
            int _Level = Level;

            if (_Level < 6)
            {
                if (_Shelve == 'F')
                {
                    _Level = 6;
                    return new BufferTrayPosition(_Shelve.ToString(), _Level);
                }
                _Level++;
            }
            else 
            {
                _Shelve++;

                if (_Shelve == 'B') 
                {
                    _Level = 5;
                    return new BufferTrayPosition(_Shelve.ToString(), _Level);
                }
               

                if (_Shelve == 'M')
                {
                    return null;
                }

                _Level = 0;
            }


            return new BufferTrayPosition(_Shelve.ToString(), _Level);


        }
       

    }
}



