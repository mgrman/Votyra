namespace Votyra.Cubical.GroupSelectors
{
    public interface IGroupSelector3i
    {
        GroupActions3i GetGroupsToUpdate(IGroupVisibilityContext3i options);
    }
}