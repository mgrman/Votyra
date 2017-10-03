using System;
using System.Collections.Generic;

namespace Votyra.Core.Pooling
{
    public interface IReadOnlyPooledList<T> : IReadOnlyList<T>, IReadOnlyPooledCollection<T>, IDisposable
    {

    }
}
