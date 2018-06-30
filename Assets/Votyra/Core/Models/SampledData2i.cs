using System;
using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct SampledData2i : IEquatable<SampledData2i>
    {
        public readonly int? x0y0;
        public readonly int? x0y1;
        public readonly int? x1y0;
        public readonly int? x1y1;

        public SampledData2i(int? x0y0, int? x0y1, int? x1y0, int? x1y1)
        {
            this.x0y0 = x0y0;
            this.x0y1 = x0y1;
            this.x1y0 = x1y0;
            this.x1y1 = x1y1;
        }


        public int GetHoleCount() => (x0y0.IsHole() ? 1 : 0)
            + (x0y1.IsHole() ? 1 : 0)
            + (x1y0.IsHole() ? 1 : 0)
            + (x1y1.IsHole() ? 1 : 0);

        public SampledData2i SetHolesUsing(SampledData2i that)
        {
            return new SampledData2i
                (
                    that.x0y0.IsNotHole() ? this.x0y0 : (int?)null,
                    that.x0y1.IsNotHole() ? this.x0y1 : (int?)null,
                    that.x1y0.IsNotHole() ? this.x1y0 : (int?)null,
                    that.x1y1.IsNotHole() ? this.x1y1 : (int?)null
                );
        }

        public SampledData2i GetRotated(int offset)
        {
            return new SampledData2i(GetIndexedValueCW(0 + offset), GetIndexedValueCW(1 + offset), GetIndexedValueCW(3 + offset), GetIndexedValueCW(2 + offset));
        }

        public int? GetIndexedValueCW(int index)
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

        public int? Max => MathUtils.Max(x0y0, MathUtils.Max(x0y1, MathUtils.Max(x1y0, x1y1)));

        public int? Min => MathUtils.Min(x0y0, MathUtils.Min(x0y1, MathUtils.Min(x1y0, x1y1)));

        public SampledData2i ClipMin(int? clipValue)
        {
            return new SampledData2i(MathUtils.Max(this.x0y0, clipValue),
                MathUtils.Max(this.x0y1, clipValue),
                MathUtils.Max(this.x1y0, clipValue),
                MathUtils.Max(this.x1y1, clipValue));
        }

        public SampledData2i NormalizeFromTop(Range1i range)
        {
            int? height = this.Max - range.Max;

            SampledData2i normalizedHeightData = new SampledData2i(this.x0y0.LowerClip(height, range.Min),
                this.x0y1.LowerClip(height, range.Min),
                this.x1y0.LowerClip(height, range.Min),
                this.x1y1.LowerClip(height, range.Min));
            return normalizedHeightData;
        }

        public SampledData2i ClipMax(int? clipValue)
        {
            return new SampledData2i(MathUtils.Min(this.x0y0, clipValue),
                MathUtils.Min(this.x0y1, clipValue),
                MathUtils.Min(this.x1y0, clipValue),
                MathUtils.Min(this.x1y1, clipValue));
        }

        public Range1i? Range
        {
            get
            {
                var min = Min;
                var max = Max;
                if (min.IsHole() || max.IsHole())
                {
                    return null;
                }
                else
                {
                    return new Range1i(min.Value, max.Value);
                }
            }
        }

        public static SampledData2i operator +(SampledData2i a, int b)
        {
            return new SampledData2i(a.x0y0 + b, a.x0y1 + b, a.x1y0 + b, a.x1y1 + b);
        }

        public static SampledData2i operator -(SampledData2i a, int b)
        {
            return new SampledData2i(a.x0y0 - b, a.x0y1 - b, a.x1y0 - b, a.x1y1 - b);
        }

        public static SampledData2i operator +(SampledData2i a, int? b)
        {
            return new SampledData2i(a.x0y0 + b, a.x0y1 + b, a.x1y0 + b, a.x1y1 + b);
        }

        public static SampledData2i operator -(SampledData2i a, int? b)
        {
            return new SampledData2i(a.x0y0 - b, a.x0y1 - b, a.x1y0 - b, a.x1y1 - b);
        }

        public static SampledData2i operator -(SampledData2i a)
        {
            return new SampledData2i(-a.x0y0, -a.x0y1, -a.x1y0, -a.x1y1);
        }

        public static int Dif(SampledData2i a, SampledData2i b)
        {
            return CombineDifs(CombineDifs(CombineDifs(Dif(a.x0y0, b.x0y0), Dif(a.x0y1, b.x0y1)), Dif(a.x1y0, b.x1y0)), Dif(a.x1y1, b.x1y1));
        }

        private static int Dif(int? a, int? b)
        {
            if (a.IsHole() && b.IsHole())
                return 0;
            else if (a.IsHole() || b.IsHole())
                return int.MaxValue;
            else
                return (a.Value - b.Value).Abs();
        }

        private static int CombineDifs(int a, int b)
        {
            return a == int.MaxValue || b == int.MaxValue ? int.MaxValue : a + b;
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
            unchecked
            {
                return this.x0y0.GetHashCode() + this.x0y1.GetHashCode() * 7 + this.x1y0.GetHashCode() * 17 + this.x1y1.GetHashCode() * 31;
            }
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

        public static IEnumerable<SampledData2i> GenerateAllValues(Range1i range, bool generateWithHoles)
        {
            var minValue = generateWithHoles ? range.Min - 1 : range.Min;
            for (int x0y0 = minValue; x0y0 <= range.Max; x0y0++)
            {
                for (int x0y1 = minValue; x0y1 <= range.Max; x0y1++)
                {
                    for (int x1y0 = minValue; x1y0 <= range.Max; x1y0++)
                    {
                        for (int x1y1 = minValue; x1y1 <= range.Max; x1y1++)
                        {
                            yield return new SampledData2i
                                (
                                    x0y0 >= range.Min ? x0y0 : (int?)null,
                                    x0y1 >= range.Min ? x0y1 : (int?)null,
                                    x1y0 >= range.Min ? x1y0 : (int?)null,
                                    x1y1 >= range.Min ? x1y1 : (int?)null
                                );
                        }
                    }
                }
            }
        }

    }
    public static class SampledData2iExtensions
    {

        public static bool IsHole(this int? value)
        {
            return !value.HasValue;
        }

        public static bool IsNotHole(this int? value)
        {
            return value.HasValue;
        }

        public static int? LowerClip(this int? value, int? height, int min)
        {
            return (value.IsNotHole() && height.IsNotHole()) ? Math.Max(value.Value - height.Value, min) : value;
        }

        public static Range1i RangeUnion(this IEnumerable<SampledData2i> templates)
        {
            return templates
                .Select(o => o.Range)
                .Aggregate((Range1i?)null,
                (a, b) => a?.UnionWith(b) ?? b) ?? Range1i.Zero;
        }
    }
}