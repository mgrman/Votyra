using System;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Core.GroupSelectors
{
    public class GroupsByCameraVisibilitySelector2i : IGroupsByCameraVisibilitySelector2i
    {
        private readonly Vector2i cellInGroupCount;
        private Range2i previousArea = Range2i.Zero;

        public GroupsByCameraVisibilitySelector2i(ITerrainConfig config)
        {
            this.cellInGroupCount = config.CellInGroupCount.XY();
        }

        public void UpdateGroupsVisibility(ArcResource<IFrameData2i> optionsResource, Func<Vector2i, bool> wasVisible, Action<Vector2i, ArcResource<IFrameData2i>> onGroupBecameVisible, Action<Vector2i> onGroupNotVisibleAnyMore)
        {
            var options = optionsResource.Value;
            if (options == null)
            {
                return;
            }

            var planes = options.CameraPlanes;
            var frustumCorners = options.CameraFrustumCorners;
            var cameraPosition = options.CameraRay.Origin;

            var cameraPositionLocal = cameraPosition.XY();

            var localCameraBounds = Area2f.FromMinAndSize(cameraPositionLocal, Vector2f.Zero);
            for (var i = 0; i < frustumCorners.Count; i++)
            {
                var frustumCorner = frustumCorners[i];
                var vector = frustumCorner.XY();
                localCameraBounds = localCameraBounds.Encapsulate(cameraPositionLocal + vector);
            }

            var cameraBoundsGroups = (localCameraBounds / this.cellInGroupCount.ToVector2f()).RoundToContain();

            var minZ = options.RangeZ.Min;
            var boundsSize = new Vector2f(this.cellInGroupCount.X, this.cellInGroupCount.Y).ToVector3f(options.RangeZ.Size);

            var areaToCheck = cameraBoundsGroups.UnionWith(this.previousArea);
            this.previousArea = cameraBoundsGroups;
            for (var ix = areaToCheck.Min.X; ix < areaToCheck.Max.X; ix++)
            {
                for (var iy = areaToCheck.Min.Y; iy < areaToCheck.Max.Y; iy++)
                {
                    var group = new Vector2i(ix, iy);
                    var groupBoundsMin = (group * this.cellInGroupCount).ToVector2f()
                        .ToVector3f(minZ);

                    var groupBounds = Area3f.FromMinAndSize(groupBoundsMin, boundsSize);
                    var isVisible = planes.TestPlanesAABB(groupBounds);

                    var isExistingGroup = wasVisible(group);
                    if (!isVisible && isExistingGroup)
                    {
                        onGroupNotVisibleAnyMore.Invoke(group);
                    }
                    else if (isVisible && !isExistingGroup)
                    {
                        onGroupBecameVisible(group, optionsResource);
                    }
                }
            }
        }
    }
}
