using System;
using System.Threading.Tasks;
using Votyra.Core.Logging;

namespace Votyra.Core.Queueing
{
    public class ParalelTaskQueue<T> : IWorkQueue<T> where T : IDisposable
    {
        private readonly string _name;
        private readonly Action<T> _updateFunction;
        private readonly TaskFactory _taskFactory;

        public ParalelTaskQueue(string name, Action<T> updateFunction)
        {
            _name = name;
            _updateFunction = updateFunction;
            _taskFactory = new TaskFactory();
        }

        public void QueueNew(T context)
        {
            _taskFactory.StartNew(TaskUpdate, context);
        }

        private void TaskUpdate(object arg)
        {
            var context = (T) arg;
            try
            {
                _updateFunction(context);
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