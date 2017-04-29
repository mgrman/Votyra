using System.Collections.Generic;

namespace Votyra.Unity.GroupSelectors
{
    public interface IGroupSelector
    {
        IList<Common.Models.Vector2i> GetGroupsToUpdate(GroupVisibilityOptions options);
    }
}