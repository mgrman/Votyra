using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public struct HeightData
{
    public readonly float x0y0;
    public readonly float x0y1;
    public readonly float x1y0;
    public readonly float x1y1;

    public HeightData(HeightData data, Func<float, float> transformation)
        : this(transformation(data.x0y0), transformation(data.x0y1), transformation(data.x1y0), transformation(data.x1y1))
    {
    }

    public HeightData(float x0y0, float x0y1, float x1y0, float x1y1)
    {
        this.x0y0 = x0y0;
        this.x0y1 = x0y1;
        this.x1y0 = x1y0;
        this.x1y1 = x1y1;
    }

    public float Max
    {
        get
        {
            return Math.Max(x0y0, Math.Max(x0y1, Math.Max(x1y0, x1y1)));
        }
    }

    public float Min
    {
        get
        {
            return Math.Min(x0y0, Math.Min(x0y1, Math.Min(x1y0, x1y1)));
        }
    }

    public float Avg
    {
        get
        {
            return (x0y0 + x0y1 + x1y0 + x1y1) / 4;
        }
    }

    public bool KeepSides
    {
        get
        {
            return (x0y0 < x0y1 && x0y0 < x1y0) || (x1y1 < x0y1 && x1y1 < x1y0) || (x0y0 > x0y1 && x0y0 > x1y0) || (x1y1 > x0y1 && x1y1 > x1y0);
        }
    }

    public HeightData ClipZ(float min, float max)
    {
        return new HeightData(x0y0.Clip(min, max),
             x0y1.Clip(min, max),
             x1y0.Clip(min, max),
             x1y1.Clip(min, max));
    }

    public static HeightData Constant(float z)
    {
        return new HeightData(z,z,z,z);
    }

    public HeightData Normalize(float minZ,float maxZ)
    {
        return new HeightData(x0y0.Normalize(minZ, maxZ),
             x0y1.Normalize(minZ, maxZ),
             x1y0.Normalize(minZ, maxZ),
             x1y1.Normalize(minZ, maxZ));
    }

    public HeightData Round(float multiple)
    {
        return new HeightData(x0y0.Round(multiple),
             x0y1.Round(multiple),
             x1y0.Round(multiple),
             x1y1.Round(multiple));
    }

    public HeightData Denormalize(float minZ, float maxZ)
    {
        return new HeightData(this.x0y0.Denormalize(minZ, maxZ),
            this.x0y1.Denormalize(minZ, maxZ),
            this.x1y0.Denormalize(minZ, maxZ),
            this.x1y1.Denormalize(minZ, maxZ));
    }

    public float DifWith(HeightData that)
    {
        return Mathf.Abs(this.x0y0 - that.x0y0) +
               Mathf.Abs(this.x0y1 - that.x0y1) +
               Mathf.Abs(this.x1y0 - that.x1y0) +
               Mathf.Abs(this.x1y1 - that.x1y1);
    }

    public static float Dif(HeightData a, HeightData b)
    {
        return Mathf.Abs(a.x0y0 - b.x0y0) +
               Mathf.Abs(a.x0y1 - b.x0y1) +
               Mathf.Abs(a.x1y0 - b.x1y0) +
               Mathf.Abs(a.x1y1 - b.x1y1);
    }

    public Vector3 x0y0Position(Rect cell)
    {
        return new Vector3(cell.xMin, cell.yMin, x0y0);
    }
    public Vector3 x0y0Position(float xMin,float yMin)
    {
        return new Vector3(xMin, yMin, x0y0);
    }

    public Vector3 x0y1Position(Rect cell)
    {
        return new Vector3(cell.xMin, cell.yMax, x0y1);
    }
    public Vector3 x0y1Position(float xMin, float yMax)
    {
        return new Vector3(xMin, yMax, x0y1);
    }

    public Vector3 x1y0Position(Rect cell)
    {
        return new Vector3(cell.xMax, cell.yMin, x1y0);
    }
    public Vector3 x1y0Position(float xMax, float yMin)
    {
        return new Vector3(xMax, yMin, x1y0);
    }

    public Vector3 x1y1Position(Rect cell)
    {
        return new Vector3(cell.xMax, cell.yMax, x1y1);
    }
    public Vector3 x1y1Position(float xMax, float yMax)
    {
        return new Vector3(xMax, yMax, x1y1);
    }

    public override string ToString()
    {
        return string.Format("x0y0:{0} , x0y1:{1} , x1y0:{2} , x1y1:{3}", x0y0, x0y1, x1y0, x1y1);
    }
}
