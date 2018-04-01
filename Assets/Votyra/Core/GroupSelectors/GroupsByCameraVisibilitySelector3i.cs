using System.Collections.Generic;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Logging;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Utils;

namespace Votyra.Core.GroupSelectors
{
    public class GroupsByCameraVisibilitySelector3i : IGroupSelector<IFrameData3b, Vector3i>
    {
        private readonly IImageSampler3 _imageSampler;
        private readonly Vector3i _cellInGroupCount;
        private readonly IThreadSafeLogger _logger;

        public GroupsByCameraVisibilitySelector3i(ITerrainConfig terrainConfig, IImageSampler3 imageSampler, IThreadSafeLogger logger)
        {
            _imageSampler = imageSampler;
            _cellInGroupCount = terrainConfig.CellInGroupCount;
            _logger = logger;
        }

        public GroupActions<Vector3i> GetGroupsToUpdate(IFrameData3b options)
        {
            if (options == null)
            {
                return null;
            }

            var planes = options.CameraPlanes;
            var frustumCorners = options.CameraFrustumCorners;
            var cameraPosition = options.CameraPosition;
            var cameraLocalToWorldMatrix = options.CameraLocalToWorldMatrix;
            var parentContainerWorldToLocalMatrix = options.ParentContainerWorldToLocalMatrix;
            var cellInGroupCount = _cellInGroupCount;
            var invalidatedArea = _imageSampler
                .ImageToWorld(options.InvalidatedArea_imageSpace)
                .RoundToContain();

            var cameraPositionLocal = parentContainerWorldToLocalMatrix.MultiplyPoint(cameraPosition);

            var localCameraBounds = Rect3f.FromMinAndSize(cameraPositionLocal, new Vector3f());
            foreach (var frustumCorner in frustumCorners)
            {
                var vector = parentContainerWorldToLocalMatrix.MultiplyPoint(cameraLocalToWorldMatrix.MultiplyVector(frustumCorner));
                localCameraBounds = localCameraBounds.Encapsulate(cameraPositionLocal + vector);
            }

            var cellInGroupCount_x = cellInGroupCount.X;
            var cellInGroupCount_y = cellInGroupCount.Y;
            var cellInGroupCount_z = cellInGroupCount.Z;
            int min_group_x = MathUtils.FloorToInt(localCameraBounds.min.X / cellInGroupCount_x);
            int min_group_y = MathUtils.FloorToInt(localCameraBounds.min.Y / cellInGroupCount_y);
            int min_group_z = MathUtils.FloorToInt(localCameraBounds.min.Z / cellInGroupCount_z);
            int max_group_x = MathUtils.CeilToInt(localCameraBounds.max.X / cellInGroupCount_x);
            int max_group_y = MathUtils.CeilToInt(localCameraBounds.max.Y / cellInGroupCount_y);
            int max_group_z = MathUtils.CeilToInt(localCameraBounds.max.Z / cellInGroupCount_z);

            var groupsToRecompute = PooledSet<Vector3i>.Create();
            var groupsToKeep = PooledSet<Vector3i>.Create();

            for (int group_x = min_group_x; group_x <= max_group_x; group_x++)
            {
                for (int group_y = min_group_y; group_y <= max_group_y; group_y++)
                {
                    for (int group_z = min_group_z; group_z <= max_group_z; group_z++)
                    {
                        var group = new Vector3i(group_x, group_y, group_z);
                        var groupBounds = Rect3i.FromMinAndSize(group * cellInGroupCount, cellInGroupCount).ToBounds();

                        bool isInside = TestPlanesAABB(planes, groupBounds);
                        if (isInside)
                        {
                            var groupArea = Rect3i.FromMinAndSize(group * cellInGroupCount, cellInGroupCount);
                            if (groupArea.Overlaps(invalidatedArea))
                            {
                                groupsToRecompute.Add(group);
                            }
                            else
                            {
                                if (!options.ExistingGroups.Contains(group))
                                {
                                    groupsToRecompute.Add(group);
                                }
                                else
                                {
                                    groupsToKeep.Add(group);
                                }
                            }
                        }
                    }
                }
            }
            return new GroupActions<Vector3i>(groupsToRecompute, groupsToKeep);
        }

        private bool TestPlanesAABB(IEnumerable<Plane3f> planes, Rect3f bounds)
        {
            var min = bounds.min;
            var max = bounds.max;

            bool isInside = true;
            foreach (var plane in planes)
            {
                isInside = isInside && TestPlaneAABB(plane, min, max);
            }
            return isInside;
        }

        private bool TestPlaneAABB(Plane3f plane, Vector3f boundsMin, Vector3f boundsMax)
        {
            return
            TestPlanePoint(plane, new Vector3f(boundsMin.X, boundsMin.Y, boundsMin.Z)) ||
                TestPlanePoint(plane, new Vector3f(boundsMin.X, boundsMin.Y, boundsMax.Z)) ||
                TestPlanePoint(plane, new Vector3f(boundsMin.X, boundsMax.Y, boundsMin.Z)) ||
                TestPlanePoint(plane, new Vector3f(boundsMin.X, boundsMax.Y, boundsMax.Z)) ||
                TestPlanePoint(plane, new Vector3f(boundsMax.X, boundsMin.Y, boundsMin.Z)) ||
                TestPlanePoint(plane, new Vector3f(boundsMax.X, boundsMin.Y, boundsMax.Z)) ||
                TestPlanePoint(plane, new Vector3f(boundsMax.X, boundsMax.Y, boundsMin.Z)) ||
                TestPlanePoint(plane, new Vector3f(boundsMax.X, boundsMax.Y, boundsMax.Z));
        }

        private bool TestPlanePoint(Plane3f plane, Vector3f point)
        {
            return plane.GetDistanceToPoint(point) > 0;
        }
    }
}