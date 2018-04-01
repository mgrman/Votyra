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

            var cameraPositionLocal = parentContainerWorldToLocalMatrix.MultiplyPoint(cameraPosition).XY;

            var localCameraBounds = Range2f.FromMinAndSize(cameraPositionLocal, new Vector2f());
            foreach (var frustumCorner in frustumCorners)
            {
                var vector = parentContainerWorldToLocalMatrix
                    .MultiplyPoint(cameraLocalToWorldMatrix.MultiplyVector(frustumCorner))
                    .XY;
                localCameraBounds = localCameraBounds.Encapsulate(cameraPositionLocal + vector);
            }
            var cameraBoundsGroups = (localCameraBounds / _cellInGroupCount.ToVector2f()).RoundToContain();

            var minZ = options.Image.RangeZ.Min;
            var bounds_size = new Vector3f(_cellInGroupCount.X, _cellInGroupCount.Y, options.Image.RangeZ.Size);

            var groupsToRecompute = PooledSet<Vector2i>.Create();
            var groupsToKeep = PooledSet<Vector2i>.Create();

            cameraBoundsGroups.ForeachPointExlusive(group =>
            {
                var groupBoundsMin = (group * _cellInGroupCount).ToVector2f().ToVector3f(minZ);
                var groupBounds = Range3f.FromMinAndSize(groupBoundsMin, bounds_size);

                bool isInside = TestPlanesAABB(planes, groupBounds);
                if (isInside)
                {
                    var groupArea = Range2i.FromMinAndSize(group * _cellInGroupCount, _cellInGroupCount);
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
            });
            return new GroupActions<Vector2i>(groupsToRecompute, groupsToKeep);
        }

        private bool TestPlanesAABB(IEnumerable<Plane3f> planes, Range3f bounds)
        {
            var min = bounds.Min;
            var max = bounds.Max;

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