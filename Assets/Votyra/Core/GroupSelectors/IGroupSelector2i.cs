namespace Votyra.Core.GroupSelectors
{
    public interface IGroupSelector2i
    {
        GroupActions2i GetGroupsToUpdate(IGroupVisibilityContext2i options);
    }
}