using System;
using TycoonTerrain.Common.Utils;

namespace TycoonTerrain.Common.Models
{
    public struct HeightData : IEquatable<HeightData>
    {
        public readonly int x0y0;
        public readonly int x0y1;
        public readonly int x1y0;
        public readonly int x1y1;

        public HeightData(HeightData data, Func<int, int> transformation)
            : this(transformation(data.x0y0), transformation(data.x0y1), transformation(data.x1y0), transformation(data.x1y1))
        {
        }

        public HeightData(int x0y0, int x0y1, int x1y0, int x1y1)
        {
            this.x0y0 = x0y0;
            this.x0y1 = x0y1;
            this.x1y0 = x1y0;
            this.x1y1 = x1y1;
        }

        public int Max
        {
            get
            {
                return Math.Max(x0y0, Math.Max(x0y1, Math.Max(x1y0, x1y1)));
            }
        }

        public int Min
        {
            get
            {
                return Math.Min(x0y0, Math.Min(x0y1, Math.Min(x1y0, x1y1)));
            }
        }

        public int Avg
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

        public static HeightData operator +(HeightData a, int b)
        {
            return new HeightData(a.x0y0 + b, a.x0y1 + b, a.x1y0 + b, a.x1y1 + b);
        }

        public static HeightData operator -(HeightData a, int b)
        {
            return new HeightData(a.x0y0 - b, a.x0y1 - b, a.x1y0 - b, a.x1y1 - b);
        }

        public HeightData ClipZ(int min, int max)
        {
            return new HeightData(x0y0.Clip(min, max),
                 x0y1.Clip(min, max),
                 x1y0.Clip(min, max),
                 x1y1.Clip(min, max));
        }

        public HeightData ClipMinZ(int min)
        {
            return new HeightData(Math.Max(x0y0, min),
                 Math.Max(x0y1, min),
                 Math.Max(x1y0, min),
                 Math.Max(x1y1, min));
        }

        public static HeightData Constant(int z)
        {
            return new HeightData(z, z, z, z);
        }

        public int DifWith(HeightData that)
        {
            return Math.Abs(this.x0y0 - that.x0y0) +
                   Math.Abs(this.x0y1 - that.x0y1) +
                   Math.Abs(this.x1y0 - that.x1y0) +
                   Math.Abs(this.x1y1 - that.x1y1);
        }

        public static int Dif(HeightData a, HeightData b)
        {
            return Math.Abs(a.x0y0 - b.x0y0) +
                   Math.Abs(a.x0y1 - b.x0y1) +
                   Math.Abs(a.x1y0 - b.x1y0) +
                   Math.Abs(a.x1y1 - b.x1y1);
        }

        public Vector3 x0y0Position(Rect cell)
        {
            return new Vector3(cell.xMin, cell.yMin, x0y0);
        }

        public Vector3 x0y0Position(int xMin, int yMin)
        {
            return new Vector3(xMin, yMin, x0y0);
        }

        public Vector3 x0y1Position(Rect cell)
        {
            return new Vector3(cell.xMin, cell.yMax, x0y1);
        }

        public Vector3 x0y1Position(int xMin, int yMax)
        {
            return new Vector3(xMin, yMax, x0y1);
        }

        public Vector3 x1y0Position(Rect cell)
        {
            return new Vector3(cell.xMax, cell.yMin, x1y0);
        }

        public Vector3 x1y0Position(int xMax, int yMin)
        {
            return new Vector3(xMax, yMin, x1y0);
        }

        public Vector3 x1y1Position(Rect cell)
        {
            return new Vector3(cell.xMax, cell.yMax, x1y1);
        }

        public Vector3 x1y1Position(int xMax, int yMax)
        {
            return new Vector3(xMax, yMax, x1y1);
        }

        public override bool Equals(object obj)
        {
            if (obj is HeightData)
            {
                var that = (HeightData)obj;
                return this.Equals(that);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.x0y0 + this.x0y1 * 7 + this.x1y0 * 17 + this.x1y1 * 31;
        }

        public override string ToString()
        {
            return string.Format("x0y0:{0} , x0y1:{1} , x1y0:{2} , x1y1:{3}", x0y0, x0y1, x1y0, x1y1);
        }

        public bool Equals(HeightData that)
        {
            return this == that;
        }

        public static bool operator ==(HeightData a, HeightData b)
        {
            return a.x0y0 == b.x0y0 && a.x0y1 == b.x0y1 && a.x1y0 == b.x1y0 && a.x1y1 == b.x1y1;
        }

        public static bool operator !=(HeightData a, HeightData b)
        {
            return a.x0y0 != b.x0y0 && a.x0y1 != b.x0y1 && a.x1y0 != b.x1y0 && a.x1y1 != b.x1y1;
        }
    }
}