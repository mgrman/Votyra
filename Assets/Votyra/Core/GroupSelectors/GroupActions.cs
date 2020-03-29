using System;
using System.Collections.Generic;
using Votyra.Core.Utils;

namespace Votyra.Core.GroupSelectors
{
    public class GroupActions<TKey> : IDisposable
    {
        public GroupActions(IReadOnlyCollection<TKey> toRecompute, IReadOnlyCollection<TKey> toKeep)
        {
            ToRecompute = toRecompute;
            ToKeep = toKeep;
        }

        public IReadOnlyCollection<TKey> ToRecompute { get; }

        public IReadOnlyCollection<TKey> ToKeep { get; }

        public void Dispose()
        {
            ToRecompute.TryDispose();
            ToKeep.TryDispose();
        }
    }
}
