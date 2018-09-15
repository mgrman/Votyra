using System;
using Votyra.Core.Pooling;

namespace Votyra.Core.GroupSelectors
{
    public class GroupActions<TKey> : IDisposable
    {
        public GroupActions(IReadOnlyPooledSet<TKey> toRecompute, IReadOnlyPooledSet<TKey> toKeep)
        {
            ToRecompute = toRecompute;
            ToKeep = toKeep;
        }

        public IReadOnlyPooledSet<TKey> ToRecompute { get; }
        public IReadOnlyPooledSet<TKey> ToKeep { get; }

        public void Dispose()
        {
            ToRecompute.Dispose();
            ToKeep.Dispose();
        }
    }
}