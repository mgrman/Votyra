using System;
using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct SampledData2i : IEquatable<SampledData2i>
    {
        public readonly Height x0y0;
        public readonly Height x0y1;
        public readonly Height x1y0;
        public readonly Height x1y1;

        public SampledData2i(Height x0y0, Height x0y1, Height x1y0, Height x1y1)
        {
            this.x0y0 = x0y0;
            this.x0y1 = x0y1;
            this.x1y0 = x1y0;
            this.x1y1 = x1y1;
        }

        public SampledData2i(int x0y0, int x0y1, int x1y0, int x1y1)
        {
            this.x0y0 = x0y0.CreateHeight();
            this.x0y1 = x0y1.CreateHeight();
            this.x1y0 = x1y0.CreateHeight();
            this.x1y1 = x1y1.CreateHeight();
        }

        public Range1h? Range
        {
            get
            {
                var min = Min;
                var max = Max;
                return Height.Range(min, max);
            }
        }

        public Height Max => Height.Max(x0y0, Height.Max(x0y1, Height.Max(x1y0, x1y1)));

        public Height Min => Height.Min(x0y0, Height.Min(x0y1, Height.Min(x1y0, x1y1)));

        public static IEnumerable<SampledData2i> GenerateAllValuesWithHoles(Range1h range)
        {
            var results = new List<SampledData2i>();
            foreach (Height x0y0 in GenerateValues(range))
            {
                foreach (Height x0y1 in GenerateValues(range))
                {
                    foreach (Height x1y0 in GenerateValues(range))
                    {
                        foreach (Height x1y1 in GenerateValues(range))
                        {
                            results.Add(new SampledData2i
                                (
                                    x0y0,
                                    x0y1,
                                    x1y0,
                                    x1y1
                                ));
                        }
                    }
                }
            }
            return results.Distinct();
        }

        public static IEnumerable<Height> GenerateValues(Range1h range)
        {
            for (Height value = range.Min; value <= range.Max; value = value.Above)
            {
                yield return value;
            }
        }

        public static SampledData2i operator -(SampledData2i a)
        {
            return new SampledData2i(-a.x0y0, -a.x0y1, -a.x1y0, -a.x1y1);
        }

        public static SampledData2i operator -(SampledData2i a, Height.Difference b)
        {
            return new SampledData2i(a.x0y0 - b, a.x0y1 - b, a.x1y0 - b, a.x1y1 - b);
        }

        public static bool operator !=(SampledData2i a, SampledData2i b)
        {
            return a.x0y0 != b.x0y0 && a.x0y1 != b.x0y1 && a.x1y0 != b.x1y0 && a.x1y1 != b.x1y1;
        }

        // public static SampledData2i operator -(SampledData2i a, int b)
        // {
        //     return new SampledData2i(a.x0y0 - b, a.x0y1 - b, a.x1y0 - b, a.x1y1 - b);
        // }
        public static SampledData2i operator *(SampledData2i a, int b)
        {
            return new SampledData2i(a.x0y0 * b, a.x0y1 * b, a.x1y0 * b, a.x1y1 * b);
        }

        public static SampledData2i operator +(SampledData2i a, Height.Difference b)
        {
            return new SampledData2i(a.x0y0 + b, a.x0y1 + b, a.x1y0 + b, a.x1y1 + b);
        }

        public static bool operator ==(SampledData2i a, SampledData2i b)
        {
            return a.x0y0 == b.x0y0 && a.x0y1 == b.x0y1 && a.x1y0 == b.x1y0 && a.x1y1 == b.x1y1;
        }

        public SampledData2i ClipMin(Height clipValue)
        {
            return new SampledData2i(Height.Max(this.x0y0, clipValue),
                Height.Max(this.x0y1, clipValue),
                Height.Max(this.x1y0, clipValue),
                Height.Max(this.x1y1, clipValue));
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

        public bool Equals(SampledData2i that)
        {
            return this == that;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.x0y0.GetHashCode() + this.x0y1.GetHashCode() * 7 + this.x1y0.GetHashCode() * 17 + this.x1y1.GetHashCode() * 31;
            }
        }


        public Height GetIndexedValueCW(int index)
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

        public SampledData2i GetRotated(int offset)
        {
            return new SampledData2i(GetIndexedValueCW(0 + offset), GetIndexedValueCW(1 + offset), GetIndexedValueCW(3 + offset), GetIndexedValueCW(2 + offset));
        }

        public override string ToString()
        {
            return string.Format("x0y0:{0} , x0y1:{1} , x1y0:{2} , x1y1:{3}", x0y0, x0y1, x1y0, x1y1);
        }

        public static Height.Difference Dif(SampledData2i a, SampledData2i b)
        {
            return Height.Difference.AbsoluteDif(a.x0y0, b.x0y0)
                + Height.Difference.AbsoluteDif(a.x0y1, b.x0y1)
                + Height.Difference.AbsoluteDif(a.x1y0, b.x1y0)
                + Height.Difference.AbsoluteDif(a.x1y1, b.x1y1);
        }
    }
}