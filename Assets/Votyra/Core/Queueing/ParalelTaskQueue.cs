using System;
using System.Threading.Tasks;
using Votyra.Core.Logging;

namespace Votyra.Core.Queueing
{
    public class ParalelTaskQueue<T> : IWorkQueue<T> where T : IDisposable
    {
        private readonly TaskFactory _taskFactory;

        public ParalelTaskQueue()
        {
            this._taskFactory = new TaskFactory();
        }

        public event Action<T> DoWork;

        public void QueueNew(T context)
        {
            this._taskFactory.StartNew(this.TaskUpdate, context);
        }

        private void TaskUpdate(object arg)
        {
            var context = (T)arg;
            try
            {
                this.DoWork?.Invoke(context);
            }
            catch (Exception ex)
            {
                StaticLogger.LogException(ex);
            }
            finally
            {
                (context as IDisposable)?.Dispose();
            }
        }
    }
}
