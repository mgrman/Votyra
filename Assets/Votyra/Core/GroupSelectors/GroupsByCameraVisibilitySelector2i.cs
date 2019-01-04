using System.Collections.Generic;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Core.GroupSelectors
{
    public static class GroupsByCameraVisibilitySelector2i
    {
        public static GroupActions<Vector2i> GetGroupsToUpdate(this IFrameData2i options)
        {
            if (options == null)
            {
                return null;
            }
            // var logger = options.LoggerFactory(this.GetType().Name, this);

            var planes = options.CameraPlanes;
            var frustumCorners = options.CameraFrustumCorners;
            var cameraPosition = options.CameraPosition;
            var cameraLocalToWorldMatrix = options.CameraLocalToWorldMatrix;
            var parentContainerWorldToLocalMatrix = options.ParentContainerWorldToLocalMatrix;
            var invalidatedArea = options.InvalidatedArea;

            var cameraPositionLocal = parentContainerWorldToLocalMatrix.MultiplyPoint(cameraPosition).XY;

            var localCameraBounds = Area2f.FromMinAndSize(cameraPositionLocal, new Vector2f());
            foreach (var frustumCorner in frustumCorners)
            {
                var vector = parentContainerWorldToLocalMatrix.MultiplyPoint(cameraLocalToWorldMatrix.MultiplyVector(frustumCorner)).XY;
                localCameraBounds = localCameraBounds.Encapsulate(cameraPositionLocal + vector);
            }

            var cameraBoundsGroups = (localCameraBounds / options.CellInGroupCount.ToVector2f()).RoundToContain();

            var minZ = options.RangeZ.Min;
            var bounds_size = new Vector2f(options.CellInGroupCount.X, options.CellInGroupCount.Y).ToVector3f(options.RangeZ.Size);

            var groupsToRecompute = PooledSet<Vector2i>.Create();
            var groupsToKeep = PooledSet<Vector2i>.Create();

            cameraBoundsGroups.ForeachPointExlusive(group =>
            {
                var groupBoundsMin = (group * options.CellInGroupCount).ToVector2f().ToVector3f(minZ);
                var groupBounds = Area3f.FromMinAndSize(groupBoundsMin, bounds_size);

                bool isInside = planes.TestPlanesAABB(groupBounds);
                if (isInside)
                {
                    var groupArea = Range2i.FromMinAndSize(group * options.CellInGroupCount, options.CellInGroupCount);
                    bool isInvalidated = groupArea.Overlaps(invalidatedArea);
                    if (isInvalidated)
                    {
                        groupsToRecompute.Add(group);
                        options.SkippedAreas.Remove(group);
                    }
                    else
                    {
                        if (!options.ExistingGroups.Contains(group))
                        {
                            var groupBoundsXYMin = (group * options.CellInGroupCount);
                            var groupBoundsXY = Range2i.FromMinAndSize(groupBoundsXYMin, options.CellInGroupCount);
                            var noData = options.SkippedAreas.Contains(group) || (options.Mask != null && (!options.Mask.AnyData(groupBoundsXY)));
                            if (noData)
                            {
                                groupsToKeep.Add(group);
                                options.SkippedAreas.Add(group);
                            }
                            else
                            {
                                groupsToRecompute.Add(group);
                            }
                        }
                        else
                        {
                            groupsToKeep.Add(group);
                        }
                    }
                }
            });
            return new GroupActions<Vector2i>(groupsToRecompute, groupsToKeep);
        }
    }
}