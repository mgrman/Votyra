using System;
using Votyra.Models;
using UnityEngine;

namespace Votyra.Utils
{
    public static class Vector2Utils
    {
        public static Vector2i ToVector2i(this Vector2 vec)
        {
            return new Vector2i(vec);
        }

        public static Vector2i FloorToVector2i(this Vector2 vec)
        {
            return new Vector2i(Mathf.FloorToInt(vec.x), Mathf.FloorToInt(vec.y));
        }

        public static Vector2i CeilToVector2i(this Vector2 vec)
        {
            return new Vector2i(Mathf.CeilToInt(vec.x), Mathf.CeilToInt(vec.y));
        }

        public static Vector2 ToVectorXY(this Vector3 a)
        {
            return new Vector2(a.x, a.y);
        }

        public static Vector2 ToAbs(this Vector2 a)
        {
            return new Vector2(Math.Abs(a.x), Math.Abs(a.y));
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
            return new Vector2(Math.Min(a.x, b.x), Math.Min(a.y, b.y));
        }

        public static Vector2 MaxBy(this Vector2 a, Vector2 b)
        {
            return new Vector2(Math.Max(a.x, b.x), Math.Max(a.y, b.y));
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
}