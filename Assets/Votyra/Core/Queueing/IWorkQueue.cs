using System;

namespace Votyra.Core.Queueing
{
    public interface IWorkQueue<T>
    {
        event Action<T> DoWork;

        void QueueNew(T context);
    }
}
