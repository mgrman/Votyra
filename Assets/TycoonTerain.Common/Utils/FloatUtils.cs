using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


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
        return Mathf.Round(val / multiple) * multiple;
    }
    public static int RoundToInt(this float val)
    {
        return Mathf.RoundToInt(val);
    }
}

