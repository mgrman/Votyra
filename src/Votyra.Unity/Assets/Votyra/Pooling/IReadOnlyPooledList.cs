using System;
using System.Collections.Generic;

namespace Votyra.Unity.Assets.Votyra.Pooling
{
    public interface IReadOnlyPooledList<T> : IReadOnlyList<T>, IDisposable
    {

    }
}