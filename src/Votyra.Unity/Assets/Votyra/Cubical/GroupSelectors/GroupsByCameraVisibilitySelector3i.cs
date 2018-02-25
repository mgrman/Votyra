using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Cubical.GroupSelectors
{
    public class GroupsByCameraVisibilitySelector3i : IGroupSelector3i
    {
        public GroupActions3i GetGroupsToUpdate(IGroupVisibilityContext3i options)
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
            var cellInGroupCount = options.CellInGroupCount;
            var invalidatedArea = options.InvalidatedArea_worldSpace;

            var cameraPositionLocal = parentContainerWorldToLocalMatrix.MultiplyPoint(cameraPosition);

            var localCameraBounds = new Bounds(cameraPositionLocal, new Vector3());
            for (int i = 0; i < 4; i++)
            {
                var vector = parentContainerWorldToLocalMatrix.MultiplyPoint(cameraLocalToWorldMatrix.MultiplyVector(frustumCorners[i]));
                localCameraBounds.Encapsulate(cameraPositionLocal + vector);
            }

            var cellInGroupCount_x = cellInGroupCount.x;
            var cellInGroupCount_y = cellInGroupCount.y;
            var cellInGroupCount_z = cellInGroupCount.z;
            int min_group_x = Mathf.FloorToInt(localCameraBounds.min.x / cellInGroupCount_x);
            int min_group_y = Mathf.FloorToInt(localCameraBounds.min.y / cellInGroupCount_y);
            int min_group_z = Mathf.FloorToInt(localCameraBounds.min.z / cellInGroupCount_z);
            int max_group_x = Mathf.CeilToInt(localCameraBounds.max.x / cellInGroupCount_x);
            int max_group_y = Mathf.CeilToInt(localCameraBounds.max.y / cellInGroupCount_y);
            int max_group_z = Mathf.CeilToInt(localCameraBounds.max.z / cellInGroupCount_z);

            var groupsToRecompute = PooledList<Vector3i>.Create();
            var groupsToKeep = PooledList<Vector3i>.Create();

            for (int group_x = min_group_x; group_x <= max_group_x; group_x++)
            {
                for (int group_y = min_group_y; group_y <= max_group_y; group_y++)
                {
                    for (int group_z = min_group_z; group_z <= max_group_z; group_z++)
                    {
                        var group = new Vector3i(group_x, group_y, group_z);
                        var groupBounds = new Rect3i(group * cellInGroupCount, cellInGroupCount).ToBounds();

                        bool isInside = TestPlanesAABB(planes, groupBounds);
                        if (isInside)
                        {
                            var groupArea = new Rect3i(group * cellInGroupCount, cellInGroupCount);
                            if (invalidatedArea != null && groupArea.Overlaps(invalidatedArea.Value))
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
            return new GroupActions3i(groupsToRecompute, groupsToKeep);
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