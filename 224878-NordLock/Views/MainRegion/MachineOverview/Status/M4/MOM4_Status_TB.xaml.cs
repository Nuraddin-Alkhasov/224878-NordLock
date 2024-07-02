using HMI.Module;
using HMI.Views.MainRegion.Recipe;
using HMI.Views.MessageBoxRegion;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using VisiWin.DataAccess;

namespace HMI.Views.MainRegion.MachineOverview
{

    [ExportView("MOM4_Status_TB")]
    public partial class MOM4_Status_TB : VisiWin.Controls.View
    {
        

        public MOM4_Status_TB()
        {
            this.InitializeComponent();
            
        }

        private void Type_Loaded(object sender, RoutedEventArgs e)
        {
        
        }


       
        private void View_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!this.IsVisible)
            {
                (new SP()).Reset();
            }
            else 
            {
                string StoredParameters = ApplicationService.ObjectStore.GetValue("MOM4_Status_TB" + "_KEY").ToString();
                string[] Parameters = StoredParameters.Split('*');
                ((StatusTBAdapter)this.DataContext).Update(Parameters);
                ApplicationService.ObjectStore.Remove("MOM4_Status_TB" + "_KEY");
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
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
            }
        }

       

     
    }
}