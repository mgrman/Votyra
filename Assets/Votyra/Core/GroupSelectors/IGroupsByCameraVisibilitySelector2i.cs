using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Votyra.Core.Models;

namespace Votyra.Core.GroupSelectors
{
    public interface IGroupsByCameraVisibilitySelector2i
    {
        void UpdateGroupsVisibility<T>( IFrameData2i options, Vector2i cellInGroupCount, IDictionary<Vector2i, T> existingGroups,object existingGroupsLock, Func<Vector2i,T> create, Action<T> dispose);
    }
}