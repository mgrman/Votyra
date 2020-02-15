using System;

namespace Votyra.Core.Queueing
{
    public interface IWorkQueue<T> 
    {
        void QueueNew(T context);

        event Action<T> DoWork;
    }
}