using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace HMI.Views.MessageBoxRegion
{
    class MessageBoxTask
    {
       public MessageBoxTask(string Message, string Header, MessageBoxIcon Icon)
       {
            Task obTask = Task.Run(() => {

                Application.Current.Dispatcher.InvokeAsync((Action)delegate
                {
                    MessageBoxView.Show(Message, Header, MessageBoxButton.OK, MessageBoxResult.OK, Icon);
                });

            });
       }
  	public MessageBoxTask(Exception ex, string param, MessageBoxIcon Icon)
        {
            Task obTask = Task.Run(() => {

                Application.Current.Dispatcher.InvokeAsync((Action)delegate
                {
                    string Message = "Error at line - " + (new StackTrace(ex, true)).GetFrame((new StackTrace(ex, true)).FrameCount - 1).GetFileLineNumber().ToString() + " - " + Environment.NewLine;
                    Message += "Message : " + ex.Message + Environment.NewLine;
                    Message += param == "" ? "" : param + Environment.NewLine;
                    Message += "Stacktrace : " + ex.StackTrace + Environment.NewLine;
                    MessageBoxView.Show(Message, " - Exception - " + DateTime.Now.ToString()+" -", MessageBoxButton.OK, MessageBoxResult.OK, Icon);
                });

            });
        }
    }
}
