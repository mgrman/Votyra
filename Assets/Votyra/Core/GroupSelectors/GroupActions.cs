using System;
using Votyra.Core.Pooling;

namespace Votyra.Core.GroupSelectors
{
    public class GroupActions<TKey> : IDisposable
    {
        public IReadOnlyPooledSet<TKey> ToRecompute { get; }
        public IReadOnlyPooledSet<TKey> ToKeep { get; }

        public GroupActions(IReadOnlyPooledSet<TKey> toRecompute, IReadOnlyPooledSet<TKey> toKeep)
        {
            ToRecompute = toRecompute;
            ToKeep = toKeep;
        }

        public void Dispose()
        {
            ToRecompute.Dispose();
            ToKeep.Dispose();
        }
    }
}