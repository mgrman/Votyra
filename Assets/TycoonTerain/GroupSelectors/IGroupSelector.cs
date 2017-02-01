using System.Collections.Generic;

public interface IGroupSelector
{
    IEnumerable<Vector2i> GetGroupsToUpdate(GroupVisibilityOptions options);
}