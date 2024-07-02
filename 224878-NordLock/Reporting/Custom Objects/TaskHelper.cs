using System;
using System.Threading;
using System.Threading.Tasks;

namespace HMI.Reporting
{
    /// <summary>
    /// Diese Klasse bietet Methoden an um Operationen explizit auf dem UI-Thread auszuführen.
    /// Dies ist bei der Verwendung von Task.Run(...) in Verbindung mit der VisiWin-API nötig da
    /// die Operation in der API nicht Thread-Safe sind und da Task.Run(...) auf irgendeinem Thread
    /// ausgeführt werden kann müssen innerhalb der Task alle aufrufe an die VisiWin-API in den
    /// UI-Thread zurückgeführt werden um die Aufrufe zu synchronisieren.
    /// </summary>
    public class TaskHelper
    {
        public static bool Initilized { get; private set; }

        private static TaskScheduler _UiTaskScheduler;
        /// <summary>
        /// Der SynchronizationContext des UI-Threads.
        /// Bevor diese Eigenschaft das erste mal verwendet wird muss die Init-Methode aufgerufen werden,
        /// sonst wird eine MemberAccessException geworfen.
        /// </summary>
        public static TaskScheduler UiTaskScheduler
        {
            get
            {
                if (Initilized)
                    return _UiTaskScheduler;
                else
                    throw new MemberAccessException("This member has not been initialized.");
            }
        }

        /// <summary>
        /// Diese Methode muss aus dem UI-Thread aufgerufen werden bevor das erste mal auf die Eigenschaft UiTaskScheduler zugegriffen wird
        /// </summary>
        public static void Init()
        {
            if (Initilized)
            {
                throw new InvalidOperationException("Has already been initialized.");
            }
            else
            {
                _UiTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                Initilized = true;
            }
        }

        /// <summary>
        /// Lässt die gegebene Action auf dem UI-Thread ausführen.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="taskCreationOptions"></param>
        /// <returns></returns>
        public static async Task RunOnUIThread(Action action, CancellationToken cancellationToken = default(CancellationToken), TaskCreationOptions taskCreationOptions = TaskCreationOptions.PreferFairness)
        {
            await Task.Factory.StartNew(action, cancellationToken, taskCreationOptions, UiTaskScheduler);
        }

        /// <summary>
        /// Lässt die gegebene Func auf dem UI-Thread ausführen.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="taskCreationOptions"></param>
        /// <returns></returns>
        public static async Task<TResult> RunOnUIThread<TResult>(Func<TResult> func, CancellationToken cancellationToken = default(CancellationToken), TaskCreationOptions taskCreationOptions = TaskCreationOptions.PreferFairness)
        {
            return await Task.Factory.StartNew(func, cancellationToken, taskCreationOptions, UiTaskScheduler);
        }
    }
}
