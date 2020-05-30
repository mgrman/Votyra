using System;
using System.Collections.Generic;
using System.Linq;

namespace Votyra.Core.Models
{
    public struct SampledData2i : IEquatable<SampledData2i>
    {
        public readonly int X0Y0;
        public readonly int X0Y1;
        public readonly int X1Y0;
        public readonly int X1Y1;

        public SampledData2i(int x0Y0, int x0Y1, int x1Y0, int x1Y1)
        {
            this.X0Y0 = x0Y0;
            this.X0Y1 = x0Y1;
            this.X1Y0 = x1Y0;
            this.X1Y1 = x1Y1;
        }

        public Area1i? Range
        {
            get
            {
                var min = this.Min;
                var max = this.Max;
                return Area1i.FromMinAndMax(min, max);
            }
        }

        public int Max => Math.Max(this.X0Y0, Math.Max(this.X0Y1, Math.Max(this.X1Y0, this.X1Y1)));

        public int Min => Math.Min(this.X0Y0, Math.Min(this.X0Y1, Math.Min(this.X1Y0, this.X1Y1)));

        public static SampledData2i operator -(SampledData2i a) => new SampledData2i(-a.X0Y0, -a.X0Y1, -a.X1Y0, -a.X1Y1);

        public static SampledData2i operator -(SampledData2i a, int b) => new SampledData2i(a.X0Y0 - b, a.X0Y1 - b, a.X1Y0 - b, a.X1Y1 - b);

        public static bool operator !=(SampledData2i a, SampledData2i b) => (a.X0Y0 != b.X0Y0) && (a.X0Y1 != b.X0Y1) && (a.X1Y0 != b.X1Y0) && (a.X1Y1 != b.X1Y1);

        // public static SampledData2i operator -(SampledData2i a, int b)
        // {
        //     return new SampledData2i(a.x0y0 - b, a.x0y1 - b, a.x1y0 - b, a.x1y1 - b);
        // }
        public static SampledData2i operator *(SampledData2i a, int b) => new SampledData2i(a.X0Y0 * b, a.X0Y1 * b, a.X1Y0 * b, a.X1Y1 * b);

        public static SampledData2i operator +(SampledData2i a, int b) => new SampledData2i(a.X0Y0 + b, a.X0Y1 + b, a.X1Y0 + b, a.X1Y1 + b);

        public static bool operator ==(SampledData2i a, SampledData2i b) => (a.X0Y0 == b.X0Y0) && (a.X0Y1 == b.X0Y1) && (a.X1Y0 == b.X1Y0) && (a.X1Y1 == b.X1Y1);

        public SampledData2i ClipMin(int clipValue) => new SampledData2i(Math.Max(this.X0Y0, clipValue), Math.Max(this.X0Y1, clipValue), Math.Max(this.X1Y0, clipValue), Math.Max(this.X1Y1, clipValue));

        public override bool Equals(object obj)
        {
            if (obj is SampledData2i)
            {
                var that = (SampledData2i)obj;
                return this.Equals(that);
            }

            return false;
        }

        public bool Equals(SampledData2i that) => this == that;

        public override int GetHashCode()
        {
            unchecked
            {
                return this.X0Y0.GetHashCode() + (this.X0Y1.GetHashCode() * 7) + (this.X1Y0.GetHashCode() * 17) + (this.X1Y1.GetHashCode() * 31);
            }
        }

        public int GetIndexedValueCw(int index)
        {
            switch (index % 4)
            {
                case 0:
                    return this.X0Y0;

                case 1:
                    return this.X0Y1;

                case 2:
                    return this.X1Y1;

                case 3:
                    return this.X1Y0;

                default:
                    throw new InvalidOperationException();
            }
        }

        public SampledData2i GetRotated(int offset) => new SampledData2i(this.GetIndexedValueCw(0 + offset), this.GetIndexedValueCw(1 + offset), this.GetIndexedValueCw(3 + offset), this.GetIndexedValueCw(2 + offset));

        public override string ToString() => string.Format("x0y0:{0} , x0y1:{1} , x1y0:{2} , x1y1:{3}", this.X0Y0, this.X0Y1, this.X1Y0, this.X1Y1);

        public static int Dif(SampledData2i a, SampledData2i b) => Math.Abs(a.X0Y0 - b.X0Y0) + Math.Abs(a.X0Y1 - b.X0Y1) + Math.Abs(a.X1Y0 - b.X1Y0) + Math.Abs(a.X1Y1 - b.X1Y1);

        public static IEnumerable<SampledData2i> GenerateAllValuesWithHoles(Area1i range)
        {
            var results = new List<SampledData2i>();
            foreach (var x0Y0 in GenerateValues(range))
            {
                foreach (var x0Y1 in GenerateValues(range))
                {
                    foreach (var x1Y0 in GenerateValues(range))
                    {
                        foreach (var x1Y1 in GenerateValues(range))
                        {
                            results.Add(new SampledData2i(x0Y0, x0Y1, x1Y0, x1Y1));
                        }
                    }
                }
            }

            return results.Distinct();
        }

        public static IEnumerable<int> GenerateValues(Area1i range)
        {
            for (var i = range.Min; i <= range.Max; i++)
            {
                yield return i;
            }
        }
    }
}
