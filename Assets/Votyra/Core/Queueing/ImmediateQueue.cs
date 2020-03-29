using System;

namespace Votyra.Core.Queueing
{
    public class ImmediateQueue<T> : IWorkQueue<T>
    {
        public event Action<T> DoWork;

        public void QueueNew(T context)
        {
            try
            {
                DoWork?.Invoke(context);
            }
            finally
            {
                (context as IDisposable)?.Dispose();
            }
        }
    }
}
