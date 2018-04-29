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

        public GroupsByCameraVisibilitySelector3i(ITerrainConfig terrainConfig, IImageSampler3 imageSampler)
        {
            _imageSampler = imageSampler;
            _cellInGroupCount = terrainConfig.CellInGroupCount;
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
            var invalidatedArea = _imageSampler
                .ImageToWorld(options.InvalidatedArea_imageSpace)
                .RoundToContain();

            var cameraPositionLocal = parentContainerWorldToLocalMatrix.MultiplyPoint(cameraPosition);

            var localCameraBounds = Range3f.FromMinAndSize(cameraPositionLocal, new Vector3f());
            foreach (var frustumCorner in frustumCorners)
            {
                var vector = parentContainerWorldToLocalMatrix.MultiplyPoint(cameraLocalToWorldMatrix.MultiplyVector(frustumCorner));
                localCameraBounds = localCameraBounds.Encapsulate(cameraPositionLocal + vector);
            }
            var cameraBoundsGroups = (localCameraBounds / _cellInGroupCount.ToVector3f()).RoundToContain();

            var groupsToRecompute = PooledSet<Vector3i>.Create();
            var groupsToKeep = PooledSet<Vector3i>.Create();

            cameraBoundsGroups.ForeachPointExlusive(group =>
            {
                var groupBounds = Range3i.FromMinAndSize(group * _cellInGroupCount, _cellInGroupCount).ToRange3f();

                bool isInside = TestPlanesAABB(planes, groupBounds);
                if (isInside)
                {
                    var groupArea = Range3i.FromMinAndSize(group * _cellInGroupCount, _cellInGroupCount);
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

            return new GroupActions<Vector3i>(groupsToRecompute, groupsToKeep);
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