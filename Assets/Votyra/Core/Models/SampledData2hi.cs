using System;
using System.Collections.Generic;
using System.Linq;

namespace Votyra.Core.Models
{
    public struct SampledData2hi : IEquatable<SampledData2hi>
    {
        public readonly Height1i x0y0;
        public readonly Height1i x0y1;
        public readonly Height1i x1y0;
        public readonly Height1i x1y1;

        public SampledData2hi(Height1i x0y0, Height1i x0y1, Height1i x1y0, Height1i x1y1)
        {
            this.x0y0 = x0y0;
            this.x0y1 = x0y1;
            this.x1y0 = x1y0;
            this.x1y1 = x1y1;
        }

        public SampledData2hi(int x0y0, int x0y1, int x1y0, int x1y1)
        {
            this.x0y0 = x0y0.CreateHeight();
            this.x0y1 = x0y1.CreateHeight();
            this.x1y0 = x1y0.CreateHeight();
            this.x1y1 = x1y1.CreateHeight();
        }

        public Range1hi? Range
        {
            get
            {
                var min = Min;
                var max = Max;
                return Height1i.Range(min, max);
            }
        }

        public Height1i Max => Height1i.Max(x0y0, Height1i.Max(x0y1, Height1i.Max(x1y0, x1y1)));

        public Height1i Min => Height1i.Min(x0y0, Height1i.Min(x0y1, Height1i.Min(x1y0, x1y1)));

        public static IEnumerable<SampledData2hi> GenerateAllValuesWithHoles(Range1hi range)
        {
            var results = new List<SampledData2hi>();
            foreach (Height1i x0y0 in GenerateValues(range))
            {
                foreach (Height1i x0y1 in GenerateValues(range))
                {
                    foreach (Height1i x1y0 in GenerateValues(range))
                    {
                        foreach (Height1i x1y1 in GenerateValues(range))
                        {
                            results.Add(new SampledData2hi
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

        public static IEnumerable<Height1i> GenerateValues(Range1hi range)
        {
            for (Height1i value = range.Min; value <= range.Max; value = value.Above)
            {
                yield return value;
            }
        }

        public static SampledData2hi operator -(SampledData2hi a)
        {
            return new SampledData2hi(-a.x0y0, -a.x0y1, -a.x1y0, -a.x1y1);
        }

        public static SampledData2hi operator -(SampledData2hi a, Height1i.Difference b)
        {
            return new SampledData2hi(a.x0y0 - b, a.x0y1 - b, a.x1y0 - b, a.x1y1 - b);
        }

        public static bool operator !=(SampledData2hi a, SampledData2hi b)
        {
            return a.x0y0 != b.x0y0 && a.x0y1 != b.x0y1 && a.x1y0 != b.x1y0 && a.x1y1 != b.x1y1;
        }

        // public static SampledData2i operator -(SampledData2i a, int b)
        // {
        //     return new SampledData2i(a.x0y0 - b, a.x0y1 - b, a.x1y0 - b, a.x1y1 - b);
        // }
        public static SampledData2hi operator *(SampledData2hi a, int b)
        {
            return new SampledData2hi(a.x0y0 * b, a.x0y1 * b, a.x1y0 * b, a.x1y1 * b);
        }

        public static SampledData2hi operator +(SampledData2hi a, Height1i.Difference b)
        {
            return new SampledData2hi(a.x0y0 + b, a.x0y1 + b, a.x1y0 + b, a.x1y1 + b);
        }

        public static bool operator ==(SampledData2hi a, SampledData2hi b)
        {
            return a.x0y0 == b.x0y0 && a.x0y1 == b.x0y1 && a.x1y0 == b.x1y0 && a.x1y1 == b.x1y1;
        }

        public SampledData2hi ClipMin(Height1i clipValue)
        {
            return new SampledData2hi(Height1i.Max(this.x0y0, clipValue),
                Height1i.Max(this.x0y1, clipValue),
                Height1i.Max(this.x1y0, clipValue),
                Height1i.Max(this.x1y1, clipValue));
        }

        public override bool Equals(object obj)
        {
            if (obj is SampledData2hi)
            {
                var that = (SampledData2hi)obj;
                return this.Equals(that);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(SampledData2hi that)
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

        public Height1i GetIndexedValueCW(int index)
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

        public SampledData2hi GetRotated(int offset)
        {
            return new SampledData2hi(GetIndexedValueCW(0 + offset), GetIndexedValueCW(1 + offset), GetIndexedValueCW(3 + offset), GetIndexedValueCW(2 + offset));
        }

        public override string ToString()
        {
            return string.Format("x0y0:{0} , x0y1:{1} , x1y0:{2} , x1y1:{3}", x0y0, x0y1, x1y0, x1y1);
        }

        public static Height1i.Difference Dif(SampledData2hi a, SampledData2hi b)
        {
            return Height1i.Difference.AbsoluteDif(a.x0y0, b.x0y0)
                + Height1i.Difference.AbsoluteDif(a.x0y1, b.x0y1)
                + Height1i.Difference.AbsoluteDif(a.x1y0, b.x1y0)
                + Height1i.Difference.AbsoluteDif(a.x1y1, b.x1y1);
        }
    }
}