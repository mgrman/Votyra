using System;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Core.GroupSelectors
{
    public class GroupActions2i : IDisposable
    {
        public IReadOnlyPooledSet<Vector2i> ToRecompute { get; }
        public IReadOnlyPooledSet<Vector2i> ToKeep { get; }

        public GroupActions2i(IReadOnlyPooledSet<Vector2i> toRecompute, IReadOnlyPooledSet<Vector2i> toKeep)
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