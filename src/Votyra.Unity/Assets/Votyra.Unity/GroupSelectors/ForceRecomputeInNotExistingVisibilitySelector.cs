using System.Collections.Generic;
using Votyra.Pooling;
using UnityEngine;
using Votyra.Common.Models;
using Votyra.Unity.Assets.Votyra.Pooling;
using System.Linq;
using System;

namespace Votyra.Unity.GroupSelectors
{
    public class ForceRecomputeInNotExistingVisibilitySelector : IGroupSelector
    {
        IGroupSelector _innerSelector;

        public ForceRecomputeInNotExistingVisibilitySelector(IGroupSelector innerSelector)
        {
            _innerSelector = innerSelector;
        }

        public IReadOnlyPooledCollection<Group> GetGroupsToUpdate(GroupVisibilityOptions options)
        {
            var readonlyGroups = _innerSelector.GetGroupsToUpdate(options);

            PooledList<Group> groups;
            if (readonlyGroups is PooledList<Group>)
            {
                groups = readonlyGroups as PooledList<Group>;
                for (int i = 0; i < groups.Count; i++)
                {
                    var group = groups[i];

                    if (!options.ExistingGroups.Contains(group.Index))
                    {
                        group = new Group(group.Index, UpdateAction.Recompute);
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