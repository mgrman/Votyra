using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class RectUtils
{
    public static Rect GetRectangle(this Rect rect, Vector2i cellCount, Vector2i cell)
    {
        var step = rect.size.DivideBy(cellCount);
        var pos = rect.position + step * cell;

        return new Rect(pos, step);
    }
}
