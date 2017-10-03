using System;
using System.Collections.Generic;

namespace Votyra.Core.Pooling
{
    public interface IReadOnlyPooledCollection<T> : IReadOnlyCollection<T>, IDisposable
    {

    }
}
