using Votyra.Core.Models;

namespace Votyra.Core.GroupSelectors
{
    public interface IGroupSelector3b
    {
        GroupActions<Vector3i> GetGroupsToUpdate(IFrameData3b options);
    }
}