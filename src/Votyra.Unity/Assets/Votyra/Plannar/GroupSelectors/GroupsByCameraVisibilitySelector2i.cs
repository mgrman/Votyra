using System.Collections.Generic;
using UnityEngine;


using System.Linq;
using Votyra.Core.Pooling;
using Votyra.Core.Models;

namespace Votyra.Plannar.GroupSelectors
{
    public class GroupsByCameraVisibilitySelector2i : IGroupSelector2i
    {
        public GroupActions2i GetGroupsToUpdate(IGroupVisibilityContext2i options)
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
            var groupBoundsTemplate = options.GroupBounds;
            var cellInGroupCount = options.CellInGroupCount;
            var invalidatedArea = options.InvalidatedArea_worldSpace;

            var cameraPositionLocal = parentContainerWorldToLocalMatrix.MultiplyPoint(cameraPosition);

            var localCameraBounds = new Bounds(cameraPositionLocal, new Vector3());
            for (int i = 0; i < 4; i++)
            {
                var vector = parentContainerWorldToLocalMatrix.MultiplyPoint(cameraLocalToWorldMatrix.MultiplyVector(frustumCorners[i]));
                localCameraBounds.Encapsulate(cameraPositionLocal + vector);
            }

            var bounds_center = groupBoundsTemplate.center;
            var bounds_size = groupBoundsTemplate.size;

            var cellInGroupCount_x = cellInGroupCount.x;
            var cellInGroupCount_y = cellInGroupCount.y;
            int min_group_x = Mathf.FloorToInt(localCameraBounds.min.x / cellInGroupCount_x);
            int min_group_y = Mathf.FloorToInt(localCameraBounds.min.y / cellInGroupCount_y);
            int max_group_x = Mathf.CeilToInt(localCameraBounds.max.x / cellInGroupCount_x);
            int max_group_y = Mathf.CeilToInt(localCameraBounds.max.y / cellInGroupCount_y);

            var groupsToRecompute = PooledList<Vector2i>.Create();
            var groupsToKeep = PooledList<Vector2i>.Create();

            for (int group_x = min_group_x; group_x <= max_group_x; group_x++)
            {
                for (int group_y = min_group_y; group_y <= max_group_y; group_y++)
                {
                    var group = new Vector2i(group_x, group_y);
                    var groupBounds = new Bounds(new Vector3
                        (
                            bounds_center.x + group_x * cellInGroupCount_x,
                            bounds_center.y + group_y * cellInGroupCount_y,
                            bounds_center.z
                        ), bounds_size);

                    bool isInside = TestPlanesAABB(planes, groupBounds);
                    if (isInside)
                    {
                        var groupArea = new Rect2i(group * cellInGroupCount, cellInGroupCount);
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
            return new GroupActions2i(groupsToRecompute, groupsToKeep);
        }

        private bool TestPlanesAABB(IEnumerable<Plane> planes, Bounds bounds)
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

        private bool TestPlaneAABB(Plane plane, Vector3 boundsMin, Vector3 boundsMax)
        {
            return
                TestPlanePoint(plane, new Vector3(boundsMin.x, boundsMin.y, boundsMin.z)) ||
                TestPlanePoint(plane, new Vector3(boundsMin.x, boundsMin.y, boundsMax.z)) ||
                TestPlanePoint(plane, new Vector3(boundsMin.x, boundsMax.y, boundsMin.z)) ||
                TestPlanePoint(plane, new Vector3(boundsMin.x, boundsMax.y, boundsMax.z)) ||
                TestPlanePoint(plane, new Vector3(boundsMax.x, boundsMin.y, boundsMin.z)) ||
                TestPlanePoint(plane, new Vector3(boundsMax.x, boundsMin.y, boundsMax.z)) ||
                TestPlanePoint(plane, new Vector3(boundsMax.x, boundsMax.y, boundsMin.z)) ||
                TestPlanePoint(plane, new Vector3(boundsMax.x, boundsMax.y, boundsMax.z));
        }

        private bool TestPlanePoint(Plane plane, Vector3 point)
        {
            return plane.GetDistanceToPoint(point) > 0;
        }
    }
}
