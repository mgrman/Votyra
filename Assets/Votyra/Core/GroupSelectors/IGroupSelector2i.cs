using Votyra.Core.Models;

namespace Votyra.Core.GroupSelectors
{
    public interface IGroupSelector2i
    {
        GroupActions<Vector2i> GetGroupsToUpdate(IFrameData2i options);
    }
}