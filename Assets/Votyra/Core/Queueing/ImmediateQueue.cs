using System;

namespace Votyra.Core.Queueing
{
    public class ImmediateQueue<T> : IWorkQueue<T>
    {
        private readonly Action<T> _updateFunction;

        public ImmediateQueue(Action<T> updateFunction)
        {
            _updateFunction = updateFunction;
        }

        public void QueueNew(T context)
        {
            try
            {
                _updateFunction(context);
            }
            finally
            {
                (context as IDisposable)?.Dispose();
            }
        }
    }
}