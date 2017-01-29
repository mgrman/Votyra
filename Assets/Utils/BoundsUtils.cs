using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class BoundsUtils
{
    public static Bounds GetBounds(this Bounds bounds, Vector2i cellCount, Vector2i cell)
    {
        var rect = new Rect(bounds.min.x, bounds.min.y, bounds.size.x, bounds.size.y);

        var step = rect.size.DivideBy(cellCount);
        var pos = rect.position + step * cell;

        var center = new Vector3(pos.x + step.x / 2, pos.y + step.y / 2, bounds.center.z);
        var size = new Vector3(step.x, step.y, bounds.size.z);

        return new Bounds(center, size);
    }

    public static List<Vector3> GetCorners(this Bounds obj, bool includePosition = true)
    {
        var result = new List<Vector3>();
        for (int x = -1; x <= 1; x += 2)
            for (int y = -1; y <= 1; y += 2)
                for (int z = -1; z <= 1; z += 2)
                    result.Add((includePosition ? obj.center : Vector3.zero) + (obj.size / 2).Times(new Vector3(x, y, z)));
        return result;
    }
}
