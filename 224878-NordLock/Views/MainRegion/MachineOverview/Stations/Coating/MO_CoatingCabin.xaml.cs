using HMI.Views.DialogRegion;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using VisiWin.DataAccess;

namespace HMI.Views.MainRegion.MachineOverview
{

    [ExportView("MO_CoatingCabin")]
    public partial class MO_CoatingCabin : VisiWin.Controls.View
    {
        IVariableService VS;
        IVariable LTBPos;
        IVariable LTBAviable;
        IVariable SDOpen;
        IVariable CoatingStep;
        public MO_CoatingCabin()
        {
            InitializeComponent();
            VS = ApplicationService.GetService<IVariableService>();
            LTBPos = VS.GetVariable("NL.PLC.Blocks.2 Modul 2.05 Tauchbecken.DB LTB HMI.Actual value.Hub.Aktuelle Position in mm");
            LTBPos.Change += LTBPos_Change;
            LTBAviable = VS.GetVariable("NL.PLC.Blocks.2 Modul 2.05 Tauchbecken.DB LTB HMI.Actual value.Dipping Vat.Vorhanden");
            LTBAviable.Change += LTBAviable_Change;

            SDOpen = VS.GetVariable("NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein HMI.Status.Schieber");
            SDOpen.Change += SDOpen_Change;

        }
        double Oldpos = 0;
        private void LTBPos_Change(object sender, VariableEventArgs e)
        {
            double pos = Math.Round(((float)e.Value) / 6.0270);

            if (Oldpos != pos)
            {
                LTB.Margin = new Thickness(744, 375, 744, pos-5);
                Oldpos = pos;
            }
        }

        private void LTBAviable_Change(object sender, VariableEventArgs e)
        {
            if ((bool)e.Value)
            {
                Task obTask = Task.Run(async () =>
                {
                    await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                    {
                        LTB.BeginAnimation(UIElement.OpacityProperty, SetOpacity(1, 1));
                    });
                });
            }
            else 
            {
                Task obTask = Task.Run(async () =>
                {
                    await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                    {
                        LTB.BeginAnimation(UIElement.OpacityProperty, SetOpacity(0, 1));
                    });
                });
            }
        }

        private void SDOpen_Change(object sender, VariableEventArgs e)
        {
            if ((short)e.Value==3)
            {
                Task obTask = Task.Run(async () =>
                {
                    await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                    {
                        SD.BeginAnimation(UIElement.OpacityProperty, SetOpacity(0, 1));
                    });
                });
            }
            else
            {
                Task obTask = Task.Run(async () =>
                {
                    await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                    {
                        SD.BeginAnimation(UIElement.OpacityProperty, SetOpacity(1, 1));
                    });
                });
            }
        }
        private void CoatingStep_Change(object sender, VariableEventArgs e)
        {
            object content;
            switch ((short)e.Value) 
            {
                case 1: content = new MO_Coating_Step_D(); break;
                case 2: content = new MO_Coating_Step_S(); break;
                case 3: content = new MO_Coating_Step_T(); break;
                default: content = null; break;
            }
            Dispatcher.InvokeAsync((Action)delegate
            {
                region.Content = content; ;
            });
           
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            (new SP
            {
                Module = 2,
                M1_Station = 0,
                M2_Station = 1,
                M3_Station = 0,
                M4_Station = 0,
                OvenTray = 0,
                TB_Shelve = 0,
                TB_Level = 0,
                Header = "@Status.Text27",
                Type = "Basket"
            }).Execute();
        }

        private void TextVarOut_ValueChanged(object sender, VisiWin.DataAccess.VariableEventArgs e)
        {
            short Paint_Id = (short)ApplicationService.GetVariableValue("NL.PLC.Blocks.2 Modul 2.05 Tauchbecken.DB LTB HMI.Actual value.Dipping Vat.Lacktyp");
            if (Paint_Id >= 1 && Paint_Id <= 10)
            {
                PaintTyp.Value = ApplicationService.GetVariableValue("NL.PLC.Blocks.50 HMI.00 Allgemein.Lacktyp Namen.Lacktyp Name[" + Paint_Id + "]").ToString();
            }
            else
            {
                PaintTyp.Value = "";
            }
        }
        
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("MainRegion", "TestTrendChartView");

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("MainRegion", "TrendChartView",
                new TrendData() 
                            {
                                ArchiveName="Paint",
                                TrendName_1="Trend1",
                                CurveTag_1= "@TrendSystem.Text2",
                                TrendName_2= "Trend2",
                                CurveTag_2= "@TrendSystem.Text3",
                                Header= "@TrendSystem.Text6",
                                Min=0,
                                Max=50,
                                BackViewName= "MO_CoatingCabin"
                            }
                );
            
        }

        private DoubleAnimation SetOpacity(Double _O, int _T)
        {
            return new DoubleAnimation
            {
                To = _O,
                Duration = TimeSpan.FromSeconds(_T),
            };
        }

        private void region_Loaded(object sender, RoutedEventArgs e)
        {
            CoatingStep = VS.GetVariable("NL.PLC.Blocks.2 Modul 2.00 Allgemein.DB Beschichtung Allgemein IW / SW / Para.Soll aktiver Schritt.Tauchen / Schleudern / Wälzen");
            CoatingStep.Change += CoatingStep_Change;
        }

    }
}



