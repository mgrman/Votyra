using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public static class Vector2Utils
{
    public static Vector2 ToVectorXY(this Vector3 a)
    {
        return new Vector2(a.x, a.y);
    }

    public static Vector2 ToAbs(this Vector2 a)
    {
        return new Vector2(Mathf.Abs(a.x), Mathf.Abs(a.y));
    }

    public static Vector2 DivideBy(this Vector2 a, Vector2 b)
    {
        return new Vector2(a.x / b.x, a.y / b.y);
    }

    public static Vector2 DivideBy(this Vector2 a, Vector2i b)
    {
        return new Vector2(a.x / b.x, a.y / b.y);
    }


    public static Vector2 MinBy(this Vector2 a, Vector2 b)
    {
        return new Vector2(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y));
    }

    public static Vector2 MaxBy(this Vector2 a, Vector2 b)
    {
        return new Vector2(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y));
    }
    public static Vector3 ToVector3(this Vector2 a, float z)
    {
        return new Vector3(a.x, a.y, z);
    }
    public static bool ZeroOrPositive(this Vector2 @this)
    {
      
            return @this.x >= 0 && @this.y >= 0;

    }
    public static bool Positive(this Vector2 @this)
    {

        return @this.x > 0 && @this.y > 0;

    }
}

