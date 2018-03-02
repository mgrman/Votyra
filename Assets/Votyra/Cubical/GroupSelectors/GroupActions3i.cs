using System;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Cubical.GroupSelectors
{
    public class GroupActions3i : IDisposable
    {
        public IReadOnlyPooledCollection<Vector3i> ToRecompute { get; }
        public IReadOnlyPooledCollection<Vector3i> ToKeep { get; }

        public GroupActions3i(IReadOnlyPooledCollection<Vector3i> toRecompute, IReadOnlyPooledCollection<Vector3i> toKeep)
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