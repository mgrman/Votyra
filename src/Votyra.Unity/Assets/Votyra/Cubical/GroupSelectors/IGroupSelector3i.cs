using System.Collections.Generic;
using Votyra.Models;
using Votyra.Unity.Assets.Votyra.Pooling;

namespace Votyra.Unity.GroupSelectors
{
    public interface IGroupSelector3i
    {
        GroupActions3i GetGroupsToUpdate(IGroupVisibilityContext3i options);
    }
}