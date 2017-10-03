using System;
using System.Collections.Generic;

namespace Votyra.Core.Models
{
    public class SetDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IReadOnlySet<TKey>
    {
        bool IReadOnlySet<TKey>.Contains(TKey value) => this.ContainsKey(value);

        IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() => Keys.GetEnumerator();
    }
}
