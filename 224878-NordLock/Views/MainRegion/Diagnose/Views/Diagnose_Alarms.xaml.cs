using HMI.Views.DialogRegion;
using HMI.Views.MessageBoxRegion;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VisiWin.ApplicationFramework;
using VisiWin.UserManagement;

namespace HMI.Diagnose
{
	/// <summary>
	/// Interaction logic for View1.xaml
	/// </summary>
	[ExportView("Diagnose_Alarms")]
	public partial class Diagnose_Alarms : VisiWin.Controls.View
	{

        private readonly Stopwatch _doubleTapStopwatch = new Stopwatch();
        private Point _lastTapLocation;

        public Diagnose_Alarms()
		{
			this.InitializeComponent();
		}

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Visible;
            ButtonOpenMenu.Visibility = Visibility.Collapsed;

        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            ButtonOpenMenu.Visibility = Visibility.Visible;

        }

        private bool IsDoubleTap(TouchEventArgs e)
        {
            Point currentTapPosition = e.GetTouchPoint(this).Position;
            bool tapsAreCloseInDistance = currentTapPosition.X - _lastTapLocation.X < 40;
            _lastTapLocation = currentTapPosition;

            TimeSpan elapsed = _doubleTapStopwatch.Elapsed;
            _doubleTapStopwatch.Restart();
            bool tapsAreCloseInTime = (elapsed != TimeSpan.Zero && elapsed < TimeSpan.FromSeconds(0.2));

            return tapsAreCloseInDistance && tapsAreCloseInTime;
        }

        private void OnPreviewTouchDown(object sender, TouchEventArgs e)
        {
            if (IsDoubleTap(e))
            {
                IUserManagementService userService = ApplicationService.GetService<IUserManagementService>();
                if (userService.CurrentUser != null && userService.CurrentUser.RightNames.Contains("Diagnose"))
                {
                    Task taskA = Task.Run(() => 
                    {
                        ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Störung Quittieren", true);
                        ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Störung Quittieren", true);
                    });
                    taskA.ContinueWith(async x =>
                    {
                        await Task.Delay(800);
                        ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Störung Quittieren", false);
                        ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Störung Quittieren", false);

                    }, TaskContinuationOptions.OnlyOnRanToCompletion);
                }
            }
        }

        private void Button_Click_TO(object sender, RoutedEventArgs e)
        {
            if (MessageBoxView.Show("@HistoricalAlarms.Text6", "@HistoricalAlarms.Text5", MessageBoxButton.YesNo, MessageBoxResult.No, MessageBoxIcon.Question) == MessageBoxResult.Yes)
            {
                ProcessStartInfo proc = new ProcessStartInfo();
                proc.WindowStyle = ProcessWindowStyle.Hidden;
                proc.FileName = "cmd";
                proc.Arguments = "/C shutdown -f -r -t 0";
                Process.Start(proc);   
            }
        }


        private void Button_Click_B(object sender, RoutedEventArgs e)
        {
            DialogView.Show("Backup", "@Backup.Text3", DialogButton.Close, DialogResult.OK);
        }

        private void Button_Click_EKS(object sender, RoutedEventArgs e)
        {
            DialogView.Show("EKS", "@EKS.Text15", DialogButton.Close, DialogResult.OK);
        }

        private void ButtonQuittieren_KeyDown(object sender, RoutedEventArgs e)
        {
            Task taskA = Task.Run(() =>
            {
                ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Störung Quittieren", true);
                ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Störung Quittieren", true);
            });
            taskA.ContinueWith(async x =>
            {
                await Task.Delay(800);
                ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Störung Quittieren", false);
                ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Störung Quittieren", false);

            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        private void ButtonHupe_KeyDown(object sender, RoutedEventArgs e)
        {
            Task taskA = Task.Run(() =>
            {
                ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Hupe aus", true);
                ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Hupe aus", true);
                Task.Delay(800);
            });
            taskA.ContinueWith(async x =>
            {
                await Task.Delay(800);
                ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Hupe aus", false);
                ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Hupe aus", false);

            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }



        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Lampentest", true);
            ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Lampentest", true);      
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Lampentest", false);
            ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Lampentest", false);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogView.Show("TransportReset", "@Buttons.Text58", DialogButton.Close, DialogResult.OK);
        }
    }
}