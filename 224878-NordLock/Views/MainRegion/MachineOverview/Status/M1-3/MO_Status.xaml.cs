using HMI.Views.MessageBoxRegion;
using System;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using VisiWin.DataAccess;

namespace HMI.Views.MainRegion.MachineOverview
{

    [ExportView("MO_Status")]
    public partial class MO_Status : VisiWin.Controls.View
    {
        IVariableService VS = ApplicationService.GetService<IVariableService>();
        IVariable actualPaint;

        public MO_Status()
        {
            this.InitializeComponent();
            actualPaint = VS.GetVariable("NL.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Charge.Aktueller Durchlauf");
            actualPaint.Change += actualPaint_ValueChanged;
        }

        private void Type_Loaded(object sender, RoutedEventArgs e)
        {
            string StoredParameters = ApplicationService.ObjectStore.GetValue("MO_Status" + "_KEY").ToString();
            string[] Parameters = StoredParameters.Split('*');
            switch (Parameters[0])
            {
                case "Basket": 
                    Type.Content = new MO_Status_Basket(); 
                    Type.Visibility = Visibility.Visible;
                    run.Visibility = Visibility.Visible;
                    charge.Visibility = Visibility.Visible;
                    weight.Visibility = Visibility.Visible;
                    break;
                case "BasketF": 
                    Type.Content = new MO_Status_BasketF(); 
                    Type.Visibility = Visibility.Visible;
                    run.Visibility = Visibility.Visible;
                    charge.Visibility = Visibility.Visible;
                    weight.Visibility = Visibility.Visible;
                    break;
                case "Box": 
                    Type.Content = new MO_Status_Box(); 
                    Type.Visibility = Visibility.Visible;
                    run.Visibility = Visibility.Hidden;
                    charge.Visibility = Visibility.Hidden;
                    weight.Visibility = Visibility.Hidden;
                    break;
                case "HCDB":
                    Type.Content = null;
                    Type.Visibility = Visibility.Collapsed;
                    run.Visibility = Visibility.Hidden;
                    charge.Visibility = Visibility.Hidden;
                    weight.Visibility = Visibility.Hidden;
                    break;
                default: 
                    Type.Content = null; 
                    Type.Visibility = Visibility.Collapsed;
                    run.Visibility = Visibility.Visible;
                    charge.Visibility = Visibility.Visible;
                    weight.Visibility = Visibility.Visible;
                    break;
            }
            ApplicationService.ObjectStore.Remove("MO_Status" + "_KEY");
            HeaderText.LocalizableText = Parameters[1];
        }

        private void actualPaint_ValueChanged(object sender, VariableEventArgs e)
        {
            if ((short)e.Value != 0)
            {
                short Paint_Id = (short)ApplicationService.GetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Header.MR.CoatingLayer " + e.Value + ".Paint Id");
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
                    ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.Data.Delete", true);
                });
                taskA.ContinueWith(async x =>
                {
                    await Task.Delay(800);
                    ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.Data.Delete", false);
                    
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
                    ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.Data.Write", true);
                });
                taskA.ContinueWith(async x =>
                {
                    await Task.Delay(800);
                    ApplicationService.SetVariableValue("NL.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Handshake.from PC.Data.Write", false);
                    await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                    {
                        ApplicationService.SetView("DialogRegion", "EmptyView");
                    });
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
            }
        }
    }
}