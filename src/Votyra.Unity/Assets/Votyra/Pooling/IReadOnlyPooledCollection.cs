using System;
using System.Collections.Generic;

namespace Votyra.Unity.Assets.Votyra.Pooling
{
    public interface IReadOnlyPooledCollection<T> : IReadOnlyCollection<T>, IDisposable
    {

    }
}