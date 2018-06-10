using System;
using System.Collections.Generic;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct SampledData2i : IEquatable<SampledData2i>
    {
        public readonly int x0y0;
        public readonly int x0y1;
        public readonly int x1y0;
        public readonly int x1y1;

        public SampledData2i(int x0y0, int x0y1, int x1y0, int x1y1)
        {
            this.x0y0 = x0y0;
            this.x0y1 = x0y1;
            this.x1y0 = x1y0;
            this.x1y1 = x1y1;
        }

        public SampledData2i GetRotated(int offset)
        {
            return new SampledData2i(GetIndexedValueCW(0 + offset), GetIndexedValueCW(1 + offset), GetIndexedValueCW(3 + offset), GetIndexedValueCW(2 + offset));
        }

        public int GetIndexedValueCW(int index)
        {
            switch (index % 4)
            {
                case 0:
                    return x0y0;

                case 1:
                    return x0y1;

                case 2:
                    return x1y1;

                case 3:
                    return x1y0;

                default:
                    throw new InvalidOperationException();
            }
        }

        public int Max => Math.Max(x0y0, Math.Max(x0y1, Math.Max(x1y0, x1y1)));

        public int Min => Math.Min(x0y0, Math.Min(x0y1, Math.Min(x1y0, x1y1)));

        public SampledData2i ClipMin(int clipValue)
        {
            return new SampledData2i(Math.Max(this.x0y0, clipValue),
                Math.Max(this.x0y1, clipValue),
                Math.Max(this.x1y0, clipValue),
                Math.Max(this.x1y1, clipValue));
        }

        public SampledData2i ClipMax(int clipValue)
        {
            return new SampledData2i(Math.Min(this.x0y0, clipValue),
                Math.Min(this.x0y1, clipValue),
                Math.Min(this.x1y0, clipValue),
                Math.Min(this.x1y1, clipValue));
        }

        public static SampledData2i operator +(SampledData2i a, int b)
        {
            return new SampledData2i(a.x0y0 + b, a.x0y1 + b, a.x1y0 + b, a.x1y1 + b);
        }

        public static SampledData2i operator -(SampledData2i a, int b)
        {
            return new SampledData2i(a.x0y0 - b, a.x0y1 - b, a.x1y0 - b, a.x1y1 - b);
        }

        public static SampledData2i operator -(SampledData2i a)
        {
            return new SampledData2i(-a.x0y0, -a.x0y1, -a.x1y0, -a.x1y1);
        }

        public static int Dif(SampledData2i a, SampledData2i b)
        {
            return Math.Abs(a.x0y0 - b.x0y0) +
                Math.Abs(a.x0y1 - b.x0y1) +
                Math.Abs(a.x1y0 - b.x1y0) +
                Math.Abs(a.x1y1 - b.x1y1);
        }

        public override bool Equals(object obj)
        {
            if (obj is SampledData2i)
            {
                var that = (SampledData2i)obj;
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

        public bool Equals(SampledData2i that)
        {
            return this == that;
        }

        public static bool operator ==(SampledData2i a, SampledData2i b)
        {
            return a.x0y0 == b.x0y0 && a.x0y1 == b.x0y1 && a.x1y0 == b.x1y0 && a.x1y1 == b.x1y1;
        }

        public static bool operator !=(SampledData2i a, SampledData2i b)
        {
            return a.x0y0 != b.x0y0 && a.x0y1 != b.x0y1 && a.x1y0 != b.x1y0 && a.x1y1 != b.x1y1;
        }

        public static IEnumerable<SampledData2i> GenerateAllValues(Range1i range)
        {
            for (int x0y0 = range.Min; x0y0 <= range.Max; x0y0++)
            {
                for (int x0y1 = range.Min; x0y1 <= range.Max; x0y1++)
                {
                    for (int x1y0 = range.Min; x1y0 <= range.Max; x1y0++)
                    {
                        for (int x1y1 = range.Min; x1y1 <= range.Max; x1y1++)
                        {
                            yield return new SampledData2i(x0y0, x0y1, x1y0, x1y1);
                        }
                    }
                }
            }
        }
    }
}