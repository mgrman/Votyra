using System.Collections.Generic;
using Votyra.Pooling;
using UnityEngine;
using Votyra.Common.Models;
using Votyra.Unity.Assets.Votyra.Pooling;
using System.Linq;
using System;

namespace Votyra.Unity.GroupSelectors
{
    public class InvalidatedAreaVisibilitySelector : IGroupSelector
    {
        IGroupSelector _innerSelector;

        public InvalidatedAreaVisibilitySelector(IGroupSelector innerSelector)
        {
            _innerSelector = innerSelector;
        }

        public IReadOnlyPooledCollection<Group> GetGroupsToUpdate(GroupVisibilityOptions options)
        {
            var readonlyGroups = _innerSelector.GetGroupsToUpdate(options);
            var invalidatedArea = options.InvalidatedArea;
            var cellInGroupCount = options.CellInGroupCount;

            PooledList<Group> groups;
            if (readonlyGroups is PooledList<Group>)
            {
                groups = readonlyGroups as PooledList<Group>;
                for (int i = 0; i < groups.Count; i++)
                {
                    var group = groups[i];

                    var groupArea = new Rect2i(group.Index * cellInGroupCount, cellInGroupCount);

                    if (!groupArea.Overlaps(invalidatedArea))
                    {
                        group = new Group(group.Index, UpdateAction.Keep);
                    }
                    groups[i] = group;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
            return groups;
        }
    }
}