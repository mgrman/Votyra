using Votyra.Core.Models;

namespace Votyra.Core.GroupSelectors
{
    public interface IGroupSelector3B
    {
        GroupActions<Vector3i> GetGroupsToUpdate(IFrameData3B options);
    }
}
