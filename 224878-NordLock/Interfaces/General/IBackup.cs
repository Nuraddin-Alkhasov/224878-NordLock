using System;

namespace HMI.Interfaces
{
    interface IBackup
    {
        /// <summary>
        /// Dies ist unser Funktion im Service. Sie schreibt in die Console
        /// </summary>
        void Start();
        void Stop();
        TimeSpan Time { set; get; }
        bool isRunning { set; get; }
    }
}
