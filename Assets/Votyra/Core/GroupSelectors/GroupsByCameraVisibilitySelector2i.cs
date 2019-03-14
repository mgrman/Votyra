using System;
using System.Collections.Generic;
using Votyra.Core.Models;

namespace Votyra.Core.GroupSelectors
{
    public static class GroupsByCameraVisibilitySelector2i
    {
        public static void UpdateGroupsVisibility(this IFrameData2i options, Vector2i cellInGroupCount, HashSet<Vector2i> groupsToRecompute, Action<Vector2i> onAdd, Action<Vector2i> onRemove)
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

            bool HandleRemoval(Vector2i @group)
            {
                var groupBoundsMin = (@group * cellInGroupCount).ToVector2f()
                    .ToVector3f(minZ);
                var groupBounds = Area3f.FromMinAndSize(groupBoundsMin, boundsSize);
                var isInside = planes.TestPlanesAABB(groupBounds);
                if (isInside)
                    return false;

                onRemove.Invoke(@group);
                return true;
            }

            groupsToRecompute.RemoveWhere(HandleRemoval);

            for (var ix = cameraBoundsGroups.Min.X; ix < cameraBoundsGroups.Max.X; ix++)
            {
                for (var iy = cameraBoundsGroups.Min.Y; iy < cameraBoundsGroups.Max.Y; iy++)
                {
                    var group = new Vector2i(ix, iy);
                    var groupBoundsMin = (group * cellInGroupCount).ToVector2f()
                        .ToVector3f(minZ);
                    var groupBounds = Area3f.FromMinAndSize(groupBoundsMin, boundsSize);
                    var isInside = planes.TestPlanesAABB(groupBounds);
                    if (!isInside)
                        continue;

                    if (groupsToRecompute.Add(group))
                        onAdd.Invoke(group);
                }
            }
        }
    }
}