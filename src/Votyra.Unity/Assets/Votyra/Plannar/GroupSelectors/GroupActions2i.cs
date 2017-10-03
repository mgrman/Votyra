using System;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Plannar.GroupSelectors
{
    public class GroupActions2i : IDisposable
    {
        public IReadOnlyPooledCollection<Vector2i> ToRecompute { get; }
        public IReadOnlyPooledCollection<Vector2i> ToKeep { get; }

        public GroupActions2i(IReadOnlyPooledCollection<Vector2i> toRecompute, IReadOnlyPooledCollection<Vector2i> toKeep)
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
