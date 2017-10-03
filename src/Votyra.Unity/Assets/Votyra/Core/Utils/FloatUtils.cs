using System;
using UnityEngine;

namespace Votyra.Core.Utils
{
    public static class FloatUtils
    {
        public static float Clip(this float i, float min, float max)
        {
            return i < min ? min : (i > max ? max : i);
        }

        public static float Normalize(this float val, float min, float max)
        {
            return (val - min) / (max - min);
        }

        public static float Denormalize(this float val, float min, float max)
        {
            return (max - min) * val + min;
        }

        public static float Round(this float val, float multiple)
        {
            return (float)(Math.Round(val / multiple) * multiple);
        }

        public static int RoundToInt(this float val)
        {
            return Mathf.RoundToInt(val);
        }
        public static int FloorToInt(this float val)
        {
            return Mathf.FloorToInt(val);
        }
        public static int CeilToInt(this float val)
        {
            return Mathf.CeilToInt(val);
        }
    }
}
