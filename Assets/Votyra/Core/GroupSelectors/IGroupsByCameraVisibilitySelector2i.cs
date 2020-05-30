using System;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Core.GroupSelectors
{
    public interface IGroupsByCameraVisibilitySelector2I
    {
        void UpdateGroupsVisibility(ArcResource<IFrameData2I> optionsResource, Func<Vector2i, bool> wasVisible, Action<Vector2i, ArcResource<IFrameData2I>> onGroupBecameVisible, Action<Vector2i> onGroupNotVisibleAnyMore);
    }
}
