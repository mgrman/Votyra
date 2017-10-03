using System.Collections.Generic;



namespace Votyra.Plannar.GroupSelectors
{
    public interface IGroupSelector2i
    {
        GroupActions2i GetGroupsToUpdate(IGroupVisibilityContext2i options);
    }
}
