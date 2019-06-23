using System;

namespace Votyra.Core.Queueing
{
    public interface IWorkQueue<T> 
    {
        void QueueNew(T context);
    }

    public interface IWorkQueue<TKey, TValue>
    {
        void QueueNew(TKey key, TValue context);
    }
}