using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Models;

namespace Votyra.Core.GroupSelectors
{
    public class GroupsByCameraVisibilitySelector2i : IGroupsByCameraVisibilitySelector2i
    {
        private Range2i _previousArea=Range2i.Zero;
        
        public void UpdateGroupsVisibility<T>( IFrameData2i options, Vector2i cellInGroupCount, IDictionary<Vector2i, T> existingGroups, object existingGroupsLock, Func<Vector2i,T> create, Action<T> dispose)
        {
            if (options == null)
                return;
            var planes = options.CameraPlanes;
            var frustumCorners = options.CameraFrustumCorners;
            var cameraPosition = options.CameraRay.Origin;

            var cameraPositionLocal = cameraPosition.XY();

            var localCameraBounds = Area2f.FromMinAndSize(cameraPositionLocal, new Vector2f());
            for (var i = 0; i < frustumCorners.Count; i++)
            {
                var frustumCorner = frustumCorners[i];
                var vector = frustumCorner.XY();
                localCameraBounds = localCameraBounds.Encapsulate(cameraPositionLocal + vector);
            }

            var cameraBoundsGroups = (localCameraBounds / cellInGroupCount.ToVector2f()).RoundToContain();

            var minZ = options.RangeZ.Min;
            var boundsSize = new Vector2f(cellInGroupCount.X, cellInGroupCount.Y).ToVector3f(options.RangeZ.Size);


            var areaToCheck = cameraBoundsGroups.UnionWith(_previousArea);
            _previousArea = cameraBoundsGroups;
            for (var ix = areaToCheck.Min.X; ix < areaToCheck.Max.X; ix++)
            {
                for (var iy = areaToCheck.Min.Y; iy < areaToCheck.Max.Y; iy++)
                {
                    var group = new Vector2i(ix, iy);
                    var groupBoundsMin = (group * cellInGroupCount).ToVector2f()
                        .ToVector3f(minZ);
                    var groupBounds = Area3f.FromMinAndSize(groupBoundsMin, boundsSize);
                    var isVisible = planes.TestPlanesAABB(groupBounds);

                    T existingGroup;
                    var isExistingGroup = existingGroups.TryGetValue(group,out existingGroup);
                    if (!isVisible && isExistingGroup)
                    {
                        lock (existingGroupsLock)
                        {
                            existingGroups.Remove(group);
                        }

                        dispose.Invoke(existingGroup);
                    }
                    else if (isVisible && !isExistingGroup)
                    {
                       var value = create(group);
                       lock (existingGroupsLock)
                       {
                           existingGroups.Add(group, value);
                       }
                    }
                }
            }
        }
    }
}