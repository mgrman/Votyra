using System;
using System.Collections.Generic;
using Votyra.Core.Models;

namespace Votyra.Core.GroupSelectors
{
    public interface IGroupsByCameraVisibilitySelector2i
    {
        event Action<Vector2i> OnAdd;
        event Action<Vector2i> OnRemove;
        void UpdateGroupsVisibility(IFrameData2i options);
    }
}