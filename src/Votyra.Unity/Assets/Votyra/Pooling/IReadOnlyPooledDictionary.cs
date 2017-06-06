using System;
using System.Collections.Generic;

namespace Votyra.Unity.Assets.Votyra.Pooling
{
    public interface IReadOnlyPooledDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>, IReadOnlyPooledCollection<KeyValuePair<TKey, TValue>>, IDisposable
    {

    }
}