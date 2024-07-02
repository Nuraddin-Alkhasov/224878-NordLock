using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using VisiWin.DataAccess;

namespace HMI
{
    /// <summary>
    /// Interaction logic for AppbarView.xaml
    /// </summary>
    [ExportView("AppbarView")]
    public partial class AppbarView : View
    {
        IVariableService VS;
        IVariable ERS;
        IVariable BFS;
        public AppbarView()
        {
            InitializeComponent();
        }
        int OldStatus=0;
        private void _Change(object sender, VariableEventArgs e)
        {
            if (ERS != null && BFS != null)
            {
                
                
                int CurrentStatus = GetStatus();

                if (CurrentStatus != OldStatus)
                {
                    Diagnose.Background = Brushes.White;
                    if (Diagnose.Background.IsFrozen)
                        Diagnose.Background = Diagnose.Background.CloneCurrentValue();
                    switch (CurrentStatus)
                    {
                        case 0: Diagnose.Background.BeginAnimation(SolidColorBrush.ColorProperty, ReSetColorAnimation((Color)FindResource("FP_Gray_C"), 1)); break;
                        case 1: Diagnose.Background.BeginAnimation(SolidColorBrush.ColorProperty, SetColorAnimation((Color)FindResource("FP_Gray_C"), (Color)FindResource("FP_Yellow_C"), 1)); break;
                        case 2: Diagnose.Background.BeginAnimation(SolidColorBrush.ColorProperty, SetColorAnimation((Color)FindResource("FP_Gray_C"), (Color)FindResource("FP_Red_C"), 1)); break;
                        case 3: Diagnose.Background.BeginAnimation(SolidColorBrush.ColorProperty, SetColorAnimation((Color)FindResource("FP_Red_C"), (Color)FindResource("FP_Yellow_C"), 1)); break;

                    }
                    OldStatus = CurrentStatus;
                }
               
            }
               
        }

        private int GetStatus() 
        {
            if (!(bool)ERS.Value && !(bool)BFS.Value) { return 0; }
            if ((!(bool)ERS.Value && (bool)BFS.Value)) { return 1; }
            if ((bool)ERS.Value && !(bool)BFS.Value) { return 2; }
            if ((bool)ERS.Value && (bool)BFS.Value) { return 3; }
            return 0;
        }


        private ColorAnimation SetColorAnimation(Color _From, Color _To, int _T)
        {
            return new ColorAnimation
            {
                From = _From,
                To = _To,
                Duration = TimeSpan.FromSeconds(_T),
                AutoReverse= true,
                RepeatBehavior = RepeatBehavior.Forever
            };
        }

        private ColorAnimation ReSetColorAnimation(Color _To, int _T)
        {
            return new ColorAnimation
            {
                To = _To,
                Duration = TimeSpan.FromSeconds(_T)
            };
        }

        private void Diagnose_Loaded(object sender, RoutedEventArgs e)
        {
            VS = ApplicationService.GetService<IVariableService>();
            ERS = VS.GetVariable("NL.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Sammelstörung Anlage");
            ERS.Change += _Change;
            BFS = VS.GetVariable("NL.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Sammelbedinerführung Anlage");
            BFS.Change += _Change;
           
        }

       
    }
}

