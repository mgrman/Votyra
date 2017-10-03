using System.Collections.Generic;
using Votyra.Models;
using Votyra.Unity.Assets.Votyra.Pooling;

namespace Votyra.Unity.GroupSelectors
{
    public interface IGroupSelector2i
    {
        GroupActions2i GetGroupsToUpdate(IGroupVisibilityContext2i options);
    }
}