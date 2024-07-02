using System;

namespace HMI.Views.DialogRegion
{
    public enum DialogButton
    {
        OK = 0,
        OKCancel = 1,
        Custom = 7,
        Close = 8
    }

    public enum DialogResult
    {
        None = 0,
        OK = 1,
        Cancel = 2,
        Left = 3,
        Middle = 4,
        Right = 5
    }

    public class DialogResultEventArgs : EventArgs
    {
        private string view;
        private DialogResult result;

        public DialogResultEventArgs(string view, DialogResult res)
        {
            this.view = view;
            this.result = res;
            CancelDialogClosing = false;
        }

        public string View
        {
            get { return this.view; }
        }

        public DialogResult Result
        {
            get { return this.result; }
        }

        public bool CancelDialogClosing { get; set; }
    }
}
