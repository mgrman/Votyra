using System;

namespace Votyra.Core.Queueing
{
    public class ImmediateQueue<TKey, TValue> : ImmediateQueue<TValue>, IWorkQueue<TKey, TValue>
    {
        public ImmediateQueue(Action<TValue> updateFunction)
            : base(updateFunction)
        {
        }

        void IWorkQueue<TKey, TValue>.QueueNew(TKey key, TValue context)
        {
            QueueNew(context);
        }
    }

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