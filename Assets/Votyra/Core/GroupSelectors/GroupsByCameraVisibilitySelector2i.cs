using System.Collections.Generic;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Utils;

namespace Votyra.Core.GroupSelectors
{
    public class GroupsByCameraVisibilitySelector2i : IGroupSelector<IFrameData2i, Vector2i>
    {
        private readonly IImageSampler2i _imageSampler;
        private readonly Vector2i _cellInGroupCount;

        public GroupsByCameraVisibilitySelector2i(ITerrainConfig terrainConfig, IImageSampler2i imageSampler)
        {
            _imageSampler = imageSampler;
            _cellInGroupCount = terrainConfig.CellInGroupCount.XY;
        }

        public GroupActions<Vector2i> GetGroupsToUpdate(IFrameData2i options)
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

            var invalidatedArea = _imageSampler
               .ImageToWorld(options.InvalidatedArea_imageSpace)
               .RoundToContain();

            var cameraPositionLocal = parentContainerWorldToLocalMatrix.MultiplyPoint(cameraPosition);

            var localCameraBounds = new Rect3f(cameraPositionLocal, new Vector3f());
            foreach (var frustumCorner in frustumCorners)
            {
                var vector = parentContainerWorldToLocalMatrix.MultiplyPoint(cameraLocalToWorldMatrix.MultiplyVector(frustumCorner));
                localCameraBounds = localCameraBounds.Encapsulate(cameraPositionLocal + vector);
            }
            var minZ = options.Image.RangeZ.min;
            var bounds_size = new Vector3f(_cellInGroupCount.x, _cellInGroupCount.y, options.Image.RangeZ.Size);

            var cellInGroupCount_x = _cellInGroupCount.x;
            var cellInGroupCount_y = _cellInGroupCount.y;

            int min_group_x = MathUtils.FloorToInt(localCameraBounds.min.x / cellInGroupCount_x);
            int min_group_y = MathUtils.FloorToInt(localCameraBounds.min.y / cellInGroupCount_y);

            int max_group_x = MathUtils.CeilToInt(localCameraBounds.max.x / cellInGroupCount_x);
            int max_group_y = MathUtils.CeilToInt(localCameraBounds.max.y / cellInGroupCount_y);


            var groupsToRecompute = PooledSet<Vector2i>.Create();
            var groupsToKeep = PooledSet<Vector2i>.Create();

            for (int group_x = min_group_x; group_x <= max_group_x; group_x++)
            {
                for (int group_y = min_group_y; group_y <= max_group_y; group_y++)
                {
                    var group = new Vector2i(group_x, group_y);
                    var groupBounds = new Rect3f(new Vector3f(
                         group_x * cellInGroupCount_x,
                         group_y * cellInGroupCount_y,
                        minZ
                    ), bounds_size);

                    bool isInside = TestPlanesAABB(planes, groupBounds);
                    if (isInside)
                    {
                        var groupArea = new Rect2i(group * _cellInGroupCount, _cellInGroupCount);
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
            return new GroupActions<Vector2i>(groupsToRecompute, groupsToKeep);
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
            TestPlanePoint(plane, new Vector3f(boundsMin.x, boundsMin.y, boundsMin.z)) ||
                TestPlanePoint(plane, new Vector3f(boundsMin.x, boundsMin.y, boundsMax.z)) ||
                TestPlanePoint(plane, new Vector3f(boundsMin.x, boundsMax.y, boundsMin.z)) ||
                TestPlanePoint(plane, new Vector3f(boundsMin.x, boundsMax.y, boundsMax.z)) ||
                TestPlanePoint(plane, new Vector3f(boundsMax.x, boundsMin.y, boundsMin.z)) ||
                TestPlanePoint(plane, new Vector3f(boundsMax.x, boundsMin.y, boundsMax.z)) ||
                TestPlanePoint(plane, new Vector3f(boundsMax.x, boundsMax.y, boundsMin.z)) ||
                TestPlanePoint(plane, new Vector3f(boundsMax.x, boundsMax.y, boundsMax.z));
        }

        private bool TestPlanePoint(Plane3f plane, Vector3f point)
        {
            return plane.GetDistanceToPoint(point) > 0;
        }
    }
}