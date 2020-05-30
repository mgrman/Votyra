using Votyra.Core.Models;

namespace Votyra.Core.GroupSelectors
{
    public interface IGroupSelector2I
    {
        GroupActions<Vector2i> GetGroupsToUpdate(IFrameData2I options);
    }
}
