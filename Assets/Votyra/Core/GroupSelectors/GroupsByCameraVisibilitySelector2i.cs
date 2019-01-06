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
            var cameraPosition = options.CameraPosition;
            var cameraLocalToWorldMatrix = options.CameraLocalToWorldMatrix;
            var parentContainerWorldToLocalMatrix = options.ParentContainerWorldToLocalMatrix;

            var cameraPositionLocal = parentContainerWorldToLocalMatrix.MultiplyPoint(cameraPosition)
                .XY;

            var localCameraBounds = Area2f.FromMinAndSize(cameraPositionLocal, new Vector2f());
            for (var i = 0; i < frustumCorners.Count; i++)
            {
                var frustumCorner = frustumCorners[i];
                var vector = parentContainerWorldToLocalMatrix.MultiplyPoint(cameraLocalToWorldMatrix.MultiplyVector(frustumCorner))
                    .XY;
                localCameraBounds = localCameraBounds.Encapsulate(cameraPositionLocal + vector);
            }

            var cameraBoundsGroups = (localCameraBounds / cellInGroupCount.ToVector2f()).RoundToContain();

            var minZ = options.RangeZ.Min;
            var boundsSize = new Vector2f(cellInGroupCount.X, cellInGroupCount.Y).ToVector3f(options.RangeZ.Size);

            groupsToRecompute.RemoveWhere(group =>
            {
                var groupBoundsMin = (group * cellInGroupCount).ToVector2f()
                    .ToVector3f(minZ);
                var groupBounds = Area3f.FromMinAndSize(groupBoundsMin, boundsSize);
                var isInside = planes.TestPlanesAABB(groupBounds);
                if (isInside)
                    return false;

                onRemove.Invoke(group);
                return true;
            });

            cameraBoundsGroups.ForeachPointExlusive(group =>
            {
                var groupBoundsMin = (group * cellInGroupCount).ToVector2f()
                    .ToVector3f(minZ);
                var groupBounds = Area3f.FromMinAndSize(groupBoundsMin, boundsSize);
                var isInside = planes.TestPlanesAABB(groupBounds);
                if (!isInside)
                    return;

                if (groupsToRecompute.Add(group))
                    onAdd.Invoke(group);
            });
        }
    }
}