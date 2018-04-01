using Votyra.Core.Models;

namespace Votyra.Core.GroupSelectors
{
    public interface IGroupSelector<TFrameData, TKey>
    where TFrameData : IFrameData
    {
        GroupActions<TKey> GetGroupsToUpdate(TFrameData options);
    }
}