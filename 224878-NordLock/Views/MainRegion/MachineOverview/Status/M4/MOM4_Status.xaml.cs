using HMI.Views.MessageBoxRegion;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using VisiWin.DataAccess;

namespace HMI.Views.MainRegion.MachineOverview
{

    [ExportView("MOM4_Status")]
    public partial class MOM4_Status : VisiWin.Controls.View
    {
        IVariableService VS = ApplicationService.GetService<IVariableService>();
        IVariable actualPaint;

        public MOM4_Status()
        {
            this.InitializeComponent();
            actualPaint = VS.GetVariable("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Charge.Aktueller Durchlauf");
            actualPaint.Change += actualPaint_ValueChanged;
        }

        private void Type_Loaded(object sender, RoutedEventArgs e)
        {
            string StoredParameters = ApplicationService.ObjectStore.GetValue("MOM4_Status" + "_KEY").ToString();
            string[] Parameters = StoredParameters.Split('*');
            switch (Parameters[0])
            {
                case "Basket": 
                    Type.Content = new MOM4_Status_Basket(); 
                    Type.Visibility = Visibility.Visible;
                    charge.Visibility = Visibility.Visible;
                    weight.Visibility = Visibility.Visible;
                    break;
                case "Box": 
                    Type.Content = new MOM4_Status_Box(); 
                    Type.Visibility = Visibility.Visible;
                    charge.Visibility = Visibility.Hidden;
                    weight.Visibility = Visibility.Hidden;
                    break;
                case "Tray": 
                    Type.Content = new MOM4_Status_Tray(); 
                    Type.Visibility = Visibility.Visible;
                    charge.Visibility = Visibility.Visible;
                    weight.Visibility = Visibility.Visible;
                    break;
                default: 
                    Type.Content = null; 
                    Type.Visibility = Visibility.Collapsed;
                    charge.Visibility = Visibility.Visible;
                    weight.Visibility = Visibility.Visible;
                    break;
            }
            ApplicationService.ObjectStore.Remove("MOM4_Status" + "_KEY");
            HeaderText.LocalizableText = Parameters[1];
        }

        private void actualPaint_ValueChanged(object sender, VariableEventArgs e)
        {
            if ((short)e.Value != 0)
            {
                short Paint_Id = (short)ApplicationService.GetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Header.MR.CoatingLayer " + e.Value + ".Paint Id");
                if (Paint_Id >= 1 && Paint_Id <= 10)
                {
                    Paint.Value = ApplicationService.GetVariableValue("NL.PLC.Blocks.50 HMI.00 Allgemein.Lacktyp Namen.Lacktyp Name[" + Paint_Id + "]").ToString();
                }
                else { Paint.Value = ""; }
            }
            else { Paint.Value = ""; }
        }

        private void View_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!this.IsVisible)
            {
                (new SP()).Reset();
            }
          
        }

        private void Key_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxView.Show("@MessageBox.Text3", "@Buttons.Text9", MessageBoxButton.YesNo, MessageBoxResult.No, MessageBoxIcon.Question) == MessageBoxResult.Yes)
            {
                Task taskA = Task.Run(() =>
                {
                    ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.Data.Delete", true);
                });
                taskA.ContinueWith(async x =>
                {
                    await Task.Delay(800);
                    ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.Data.Delete", false);
                    await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                    {
                        ApplicationService.SetView("DialogRegion", "EmptyView");
                    });
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
            }
        }

        private void Key_Click_1(object sender, RoutedEventArgs e)
        {
            if (MessageBoxView.Show("@MessageBox.Text3", "@Buttons.Text8", MessageBoxButton.YesNo, MessageBoxResult.No, MessageBoxIcon.Question) == MessageBoxResult.Yes)
            {
                Task taskA = Task.Run(() =>
                {
                    ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.Data.Write", true);
                });
                taskA.ContinueWith(async x =>
                {
                    await Task.Delay(800);
                    ApplicationService.SetVariableValue("NLM4.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.Data.Write", false);
                    await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                    {
                        ApplicationService.SetView("DialogRegion", "EmptyView");
                    });
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (admin.Opacity == 0)
            {
                admin.BeginAnimation(UIElement.OpacityProperty, SetOpacity(1, 1));
            }
            else
            {
                admin.BeginAnimation(UIElement.OpacityProperty, SetOpacity(0, 1));
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
    }
}