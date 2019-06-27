using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Core.GroupSelectors
{
    public interface IGroupsByCameraVisibilitySelector2i
    {
        void UpdateGroupsVisibility(ArcResource<IFrameData2i> optionsResource, Vector2i cellInGroupCount, Func<Vector2i, bool> wasVisible, Action<Vector2i, ArcResource<IFrameData2i>> onGroupBecameVisible, Action<Vector2i> onGroupNotVisibleAnyMore);
    }
}