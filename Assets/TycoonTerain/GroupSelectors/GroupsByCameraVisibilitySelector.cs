using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class GroupsByCameraVisibilitySelector : IGroupSelector
{
    private Vector3[] frustumCorners = new Vector3[4];


    IList<Vector2i> _cachedGroups = Pool.Vector2iListPool.GetObject();

    private GroupVisibilityOptions _oldOptions;

    public IList<Vector2i> GetGroupsToUpdate(GroupVisibilityOptions options)
    {
        if (options == null)
        {
            return null;
        }
        if (_oldOptions != null && !options.IsChanged(_oldOptions))
        {
            return _cachedGroups;
        }
        _oldOptions = options;

        var camera = Camera.main;

        var projectionToLocal = camera.projectionMatrix * camera.worldToCameraMatrix * options.ParentContainer.transform.localToWorldMatrix;

        var planes = GeometryUtility.CalculateFrustumPlanes(projectionToLocal);

        camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);

        var cameraPositionLocal = options.ParentContainer.transform.InverseTransformPoint(camera.transform.position);

        Bounds localCameraBounds = new Bounds(cameraPositionLocal, new Vector3());
        for (int i = 0; i < 4; i++)
        {
            var vector = options.ParentContainer.transform.InverseTransformVector(camera.transform.TransformVector(frustumCorners[i]));

            localCameraBounds.Encapsulate(cameraPositionLocal + vector);
        }

        var bounds_center = options.GroupBounds.center;
        var bounds_size = options.GroupBounds.size;


        int min_group_x = Mathf.FloorToInt(localCameraBounds.min.x);
        int min_group_y = Mathf.FloorToInt(localCameraBounds.min.y);
        int max_group_x = Mathf.CeilToInt(localCameraBounds.max.x);
        int max_group_y = Mathf.CeilToInt(localCameraBounds.max.y);
        var cellInGroupCount_x = options.CellInGroupCount.x;
        var cellInGroupCount_y = options.CellInGroupCount.y;

        IList<Vector2i> groups = Pool.Vector2iListPool.GetObject();
        groups.Clear();
        _cachedGroups.Clear();
        for (int group_x = min_group_x; group_x <= max_group_x; group_x++)
        {
            for (int group_y = min_group_y; group_y <= max_group_y; group_y++)
            {
                var groupBounds = new Bounds(new Vector3
                    (
                        bounds_center.x + group_x * cellInGroupCount_x,
                        bounds_center.y + group_y * cellInGroupCount_y,
                        bounds_center.z
                    ), bounds_size);

                // bounds = ParentContainer.transform.TransformBounds(bounds);


                if (GeometryUtility.TestPlanesAABB(planes, groupBounds))
                {
                    var group = new Vector2i(group_x, group_y);
                    groups.Add(group);
                    _cachedGroups.Add(group);
                }
            }
        }

        options.Dispose();
        return groups;
    }

}

