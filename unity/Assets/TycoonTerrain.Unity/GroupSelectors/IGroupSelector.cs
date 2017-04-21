using System.Collections.Generic;

namespace TycoonTerrain.Unity.GroupSelectors
{
    public interface IGroupSelector
    {
        IList<Common.Models.Vector2i> GetGroupsToUpdate(GroupVisibilityOptions options);
    }
}