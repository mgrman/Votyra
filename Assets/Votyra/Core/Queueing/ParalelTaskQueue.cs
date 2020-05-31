using System;
using System.Threading.Tasks;
using Votyra.Core.Logging;

namespace Votyra.Core.Queueing
{
    public class ParalelTaskQueue<T> : IWorkQueue<T>
        where T : IDisposable
    {
        private readonly TaskFactory taskFactory;

        public ParalelTaskQueue()
        {
            this.taskFactory = new TaskFactory();
        }

        public event Action<T> DoWork;

        public void QueueNew(T context)
        {
            this.taskFactory.StartNew(this.TaskUpdate, context);
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
