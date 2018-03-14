using System;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Core.GroupSelectors
{
    public class GroupActions3i : IDisposable
    {
        public IReadOnlyPooledSet<Vector3i> ToRecompute { get; }
        public IReadOnlyPooledSet<Vector3i> ToKeep { get; }

        public GroupActions3i(IReadOnlyPooledSet<Vector3i> toRecompute, IReadOnlyPooledSet<Vector3i> toKeep)
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