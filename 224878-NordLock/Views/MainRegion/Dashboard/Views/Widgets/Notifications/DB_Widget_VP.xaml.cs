using HMI.Views.MessageBoxRegion;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;


namespace HMI.Dashboard
{
    [ExportDashboardWidget("DB_Widget_VP", "Dashboard.Text20", "@Dashboard.Text19", 1, 1)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class DB_Widget_VP : View
    {
        DateTime StartTime;
        BackgroundWorker counter;
        public DB_Widget_VP()
        {
            this.InitializeComponent();
            tbTime.Value = new DateTime();
            counter = new BackgroundWorker();
            counter.DoWork += A_DoWork;
            if ((bool)ApplicationService.GetVariableValue("Dashboard.counter1ON"))
            {
                CounterON();
                counter.RunWorkerAsync();
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (tbTime.Value.TimeOfDay.TotalSeconds >= 10)
            {
                CounterON();
                counter.RunWorkerAsync();
            }
            
           
        }

        private void A_DoWork(object sender, DoWorkEventArgs e)
        {
            StartTime = (DateTime)ApplicationService.GetVariableValue("Dashboard.counter1");

            try
            {
                while ((bool)ApplicationService.GetVariableValue("Dashboard.counter1ON"))
                {
                    DateTime temp = (DateTime)ApplicationService.GetVariableValue("Dashboard.counter1");
                    TimeSpan _time = new TimeSpan();
                    _time = _time.Add(TimeSpan.FromHours(temp.Hour));
                    _time = _time.Add(TimeSpan.FromMinutes(temp.Minute));
                    _time = _time.Add(TimeSpan.FromSeconds(temp.Second));

                    if (_time == TimeSpan.Zero)
                    {
                        Application.Current.Dispatcher.InvokeAsync((Action)delegate
                        {
                            if (MessageBoxView.Show("@Dashboard.Text22", "@Dashboard.Text20", MessageBoxButton.OK, MessageBoxResult.OK, MessageBoxIcon.Exclamation) == MessageBoxResult.OK)
                            {
                                ApplicationService.SetVariableValue("Dashboard.counter1", StartTime);
                                Start_Click(this, null);
                            }
                        });
                        return;
                    }
                    else
                    {
                        Thread.Sleep(1000);
                        _time = _time.Add(TimeSpan.FromSeconds(-1));
                        temp = new DateTime();
                        temp = temp + _time;
                        ApplicationService.SetVariableValue("Dashboard.counter1", temp);
                    }
                }
            }
            catch
            {

            }
              
            ApplicationService.SetVariableValue("Dashboard.counter1", StartTime);
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            start.IsEnabled = true;
            Enable.Visibility = Visibility.Collapsed;
            stop.IsEnabled = false;
            ApplicationService.SetVariableValue("Dashboard.counter1ON", false);
        }

        private void CounterON()
        {
            start.IsEnabled = false;
            Enable.Visibility = Visibility.Visible;
            stop.IsEnabled = true;
            ApplicationService.SetVariableValue("Dashboard.counter1ON", true);
        }

    }
}