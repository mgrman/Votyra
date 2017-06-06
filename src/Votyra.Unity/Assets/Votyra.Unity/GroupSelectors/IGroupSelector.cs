using System.Collections.Generic;
using Votyra.Common.Models;
using Votyra.Unity.Assets.Votyra.Pooling;

namespace Votyra.Unity.GroupSelectors
{
    public interface IGroupSelector
    {
        GroupActions GetGroupsToUpdate(IGroupVisibilityContext options);
    }
}