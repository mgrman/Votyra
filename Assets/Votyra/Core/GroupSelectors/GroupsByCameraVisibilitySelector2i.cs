using System;
using System.Collections.Generic;
using Votyra.Core.Models;

namespace Votyra.Core.GroupSelectors
{
    public class GroupsByCameraVisibilitySelector2i : IGroupsByCameraVisibilitySelector2i
    {
        private Vector2i _cellInGroupCount;

        private readonly HashSet<Vector2i> _groupsToRecompute = new HashSet<Vector2i>();

        public GroupsByCameraVisibilitySelector2i(ITerrainConfig terrainConfig)
        {
            _cellInGroupCount = terrainConfig.CellInGroupCount;
        }

        public event Action<Vector2i> OnAdd;
        public event Action<Vector2i> OnRemove;
        
        public void UpdateGroupsVisibility(IFrameData2i options )
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
            for (var i = 0; i < frustumCorners.Length; i++)
            {
                var frustumCorner = frustumCorners[i];
                var vector = parentContainerWorldToLocalMatrix.MultiplyPoint(cameraLocalToWorldMatrix.MultiplyVector(frustumCorner))
                    .XY;
                localCameraBounds = localCameraBounds.Encapsulate(cameraPositionLocal + vector);
            }

            var cameraBoundsGroups = (localCameraBounds / _cellInGroupCount.ToVector2f()).RoundToContain();

            var minZ = options.RangeZ.Min;
            var boundsSize = new Vector2f(_cellInGroupCount.X, _cellInGroupCount.Y).ToVector3f(options.RangeZ.Size);

            _groupsToRecompute.RemoveWhere(group =>
            {
                var groupBoundsMin = (group * _cellInGroupCount).ToVector2f()
                    .ToVector3f(minZ);
                var groupBounds = Area3f.FromMinAndSize(groupBoundsMin, boundsSize);
                var isInside = planes.TestPlanesAABB(groupBounds);
                if (isInside)
                {
                    return false;
                }

                OnRemove?.Invoke(group);
                return true;
            });

            for (int ix = cameraBoundsGroups.Min.X; ix < cameraBoundsGroups.Max.X; ix++)
            {
                for (int iy = cameraBoundsGroups.Min.Y; iy < cameraBoundsGroups.Max.Y; iy++)
                {
                    var group = new Vector2i(ix, iy);
                    var groupBoundsMin = (group * _cellInGroupCount).ToVector2f()
                        .ToVector3f(minZ);
                    var groupBounds = Area3f.FromMinAndSize(groupBoundsMin, boundsSize);
                    var isInside = planes.TestPlanesAABB(groupBounds);
                    if (!isInside)
                    {
                        continue;
                    }

                    if (_groupsToRecompute.Add(group))
                    {
                        OnAdd?.Invoke(group);
                    }
                }
            }
        }
    }
}