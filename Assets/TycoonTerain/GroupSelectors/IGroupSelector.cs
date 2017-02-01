using System.Collections.Generic;

public interface IGroupSelector
{
    IList<Vector2i> GetGroupsToUpdate(GroupVisibilityOptions options);
}