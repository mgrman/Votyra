using System;
using System.Collections.Generic;
using System.Linq;

namespace Votyra.Core.Models
{
    public struct SampledData2hf : IEquatable<SampledData2hf>
    {
        public readonly Height1f x0y0;
        public readonly Height1f x0y1;
        public readonly Height1f x1y0;
        public readonly Height1f x1y1;

        public SampledData2hf(Height1f x0y0, Height1f x0y1, Height1f x1y0, Height1f x1y1)
        {
            this.x0y0 = x0y0;
            this.x0y1 = x0y1;
            this.x1y0 = x1y0;
            this.x1y1 = x1y1;
        }

        public SampledData2hf(float x0y0, float x0y1, float x1y0, float x1y1)
        {
            this.x0y0 = x0y0.CreateHeight1f();
            this.x0y1 = x0y1.CreateHeight1f();
            this.x1y0 = x1y0.CreateHeight1f();
            this.x1y1 = x1y1.CreateHeight1f();
        }

        public Range1hf? Range
        {
            get
            {
                var min = Min;
                var max = Max;
                return Height1f.Range(min, max);
            }
        }

        public Height1f Max => Height1f.Max(x0y0, Height1f.Max(x0y1, Height1f.Max(x1y0, x1y1)));

        public Height1f Min => Height1f.Min(x0y0, Height1f.Min(x0y1, Height1f.Min(x1y0, x1y1)));

        public static SampledData2hf operator -(SampledData2hf a)
        {
            return new SampledData2hf(-a.x0y0, -a.x0y1, -a.x1y0, -a.x1y1);
        }

        public static SampledData2hf operator -(SampledData2hf a, Height1f.Difference b)
        {
            return new SampledData2hf(a.x0y0 - b, a.x0y1 - b, a.x1y0 - b, a.x1y1 - b);
        }

        public static bool operator !=(SampledData2hf a, SampledData2hf b)
        {
            return a.x0y0 != b.x0y0 && a.x0y1 != b.x0y1 && a.x1y0 != b.x1y0 && a.x1y1 != b.x1y1;
        }

        // public static SampledData2i operator -(SampledData2i a, int b)
        // {
        //     return new SampledData2i(a.x0y0 - b, a.x0y1 - b, a.x1y0 - b, a.x1y1 - b);
        // }
        public static SampledData2hf operator *(SampledData2hf a, int b)
        {
            return new SampledData2hf(a.x0y0 * b, a.x0y1 * b, a.x1y0 * b, a.x1y1 * b);
        }

        public static SampledData2hf operator +(SampledData2hf a, Height1f.Difference b)
        {
            return new SampledData2hf(a.x0y0 + b, a.x0y1 + b, a.x1y0 + b, a.x1y1 + b);
        }

        public static bool operator ==(SampledData2hf a, SampledData2hf b)
        {
            return a.x0y0 == b.x0y0 && a.x0y1 == b.x0y1 && a.x1y0 == b.x1y0 && a.x1y1 == b.x1y1;
        }

        public SampledData2hf ClipMin(Height1f clipValue)
        {
            return new SampledData2hf(Height1f.Max(this.x0y0, clipValue),
                Height1f.Max(this.x0y1, clipValue),
                Height1f.Max(this.x1y0, clipValue),
                Height1f.Max(this.x1y1, clipValue));
        }

        public override bool Equals(object obj)
        {
            if (obj is SampledData2hf)
            {
                var that = (SampledData2hf)obj;
                return this.Equals(that);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(SampledData2hf that)
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

        public Height1f GetIndexedValueCW(int index)
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

        public SampledData2hf GetRotated(int offset)
        {
            return new SampledData2hf(GetIndexedValueCW(0 + offset), GetIndexedValueCW(1 + offset), GetIndexedValueCW(3 + offset), GetIndexedValueCW(2 + offset));
        }

        public override string ToString()
        {
            return string.Format("x0y0:{0} , x0y1:{1} , x1y0:{2} , x1y1:{3}", x0y0, x0y1, x1y0, x1y1);
        }

        public static Height1f.Difference Dif(SampledData2hf a, SampledData2hf b)
        {
            return Height1f.Difference.AbsoluteDif(a.x0y0, b.x0y0)
                + Height1f.Difference.AbsoluteDif(a.x0y1, b.x0y1)
                + Height1f.Difference.AbsoluteDif(a.x1y0, b.x1y0)
                + Height1f.Difference.AbsoluteDif(a.x1y1, b.x1y1);
        }
    }

    public static class SampledData2hfExtensions
    {
        public static SampledData2hf ToSampledData2hf(this SampledData2hf data)
        {
            return new SampledData2hf(data.x0y0.RawValue, data.x0y1.RawValue, data.x1y0.RawValue, data.x1y1.RawValue);
        }

        public static SampledData2hf ToSampledData2hf(this SampledMask2e data)
        {
            return new SampledData2hf(MaskToHeight(data.x0y0), MaskToHeight(data.x0y1), MaskToHeight(data.x1y0),
                MaskToHeight(data.x1y1));
        }

        private static float MaskToHeight(this MaskValues data)
        {
            return data.IsHole() ? -1 : 1;
        }
    }

}