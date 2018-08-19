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

        public SampledData2i(int? x0y0, int? x0y1, int? x1y0, int? x1y1)
        {
            this.x0y0 = x0y0.CreateHeight();
            this.x0y1 = x0y1.CreateHeight();
            this.x1y0 = x1y0.CreateHeight();
            this.x1y1 = x1y1.CreateHeight();
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

        public Height Max => Height.MaxHoleless(x0y0, Height.MaxHoleless(x0y1, Height.MaxHoleless(x1y0, x1y1)));

        public Height Min => Height.MinHoleless(x0y0, Height.MinHoleless(x0y1, Height.MinHoleless(x1y0, x1y1)));

        public static IEnumerable<SampledData2i> GenerateAllValuesWithHoles(Range1h range)
        {
            // var minValue = generateWithHoles ?nu : range.Min;

            var results = new List<SampledData2i>();
            for (Height x0y0 = range.Min; x0y0 <= range.Max; x0y0 = x0y0.Above)
            {
                for (Height x0y1 = range.Min; x0y1 <= range.Max; x0y1 = x0y1.Above)
                {
                    for (Height x1y0 = range.Min; x1y0 <= range.Max; x1y0 = x1y0.Above)
                    {
                        for (Height x1y1 = range.Min; x1y1 <= range.Max; x1y1 = x1y1.Above)
                        {
                            results.Add(new SampledData2i
                                (
                                    x0y0,
                                    x0y1,
                                    x1y0,
                                    x1y1
                                ));
                            results.Add(new SampledData2i
                                (
                                    Height.Hole,
                                    x0y1,
                                    x1y0,
                                    x1y1
                                ));
                            results.Add(new SampledData2i
                                 (
                                     x0y0,
                                     Height.Hole,
                                     x1y0,
                                     x1y1
                                 ));
                            results.Add(new SampledData2i
                                 (
                                     x0y0,
                                     x0y1,
                                     Height.Hole,
                                     x1y1
                                 ));
                            results.Add(new SampledData2i
                                 (
                                     x0y0,
                                     x0y1,
                                     x1y0,
                                     Height.Hole
                                 ));
                        }
                    }
                }
            }
            return results.Distinct();
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

        // public SampledData2i ClipMax(int? clipValue)
        // {
        //     return new SampledData2i(MathUtils.Min(this.x0y0, clipValue),
        //         MathUtils.Min(this.x0y1, clipValue),
        //         MathUtils.Min(this.x1y0, clipValue),
        //         MathUtils.Min(this.x1y1, clipValue));
        // }
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

        // public static SampledData2i operator +(SampledData2i a, int b)
        // {
        //     return new SampledData2i(a.x0y0 + b, a.x0y1 + b, a.x1y0 + b, a.x1y1 + b);
        // }
        public bool Equals(SampledData2i that)
        {
            return this == that;
        }

        // public static SampledData2i operator *(SampledData2i a, decimal b)
        // {
        //     return new SampledData2i(MathUtils.RoundToInt(a.x0y0 * b), MathUtils.RoundToInt(a.x0y1 * b), MathUtils.RoundToInt(a.x1y0 * b), MathUtils.RoundToInt(a.x1y1 * b));
        // }
        public override int GetHashCode()
        {
            unchecked
            {
                return this.x0y0.GetHashCode() + this.x0y1.GetHashCode() * 7 + this.x1y0.GetHashCode() * 17 + this.x1y1.GetHashCode() * 31;
            }
        }

        // public static SampledData2i operator *(SampledData2i a, double b)
        // {
        //     return new SampledData2i((int)(a.x0y0 * b), (int)(a.x0y1 * b), (int)(a.x1y0 * b), (int)(a.x1y1 * b));
        // }
        public int GetHoleCount() => (x0y0.IsHole ? 1 : 0)
                                                                                                                                                                            + (x0y1.IsHole ? 1 : 0)
            + (x1y0.IsHole ? 1 : 0)
            + (x1y1.IsHole ? 1 : 0);

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

        public SampledData2i SetHolesUsing(SampledData2i that)
        {
            return new SampledData2i
                (
                    that.x0y0.IsNotHole ? this.x0y0 : Height.Hole,
                    that.x0y1.IsNotHole ? this.x0y1 : Height.Hole,
                    that.x1y0.IsNotHole ? this.x1y0 : Height.Hole,
                    that.x1y1.IsNotHole ? this.x1y1 : Height.Hole
                );
        }

        //     SampledData2i normalizedHeightData = new SampledData2i(this.x0y0.LowerClip(height, range.Min),
        //         this.x0y1.LowerClip(height, range.Min),
        //         this.x1y0.LowerClip(height, range.Min),
        //         this.x1y1.LowerClip(height, range.Min));
        //     return normalizedHeightData;
        // }
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

        // public SampledData2i NormalizeFromTop(Range1h range)
        // {
        //     Height.Difference height = this.Max - range.Max.CreateHeight();
    }
}