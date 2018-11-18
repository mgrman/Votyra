using System;

namespace Votyra.Core.Models
{
    public struct SampledData2f : IEquatable<SampledData2f>
    {
        public readonly float x0y0;
        public readonly float x0y1;
        public readonly float x1y0;
        public readonly float x1y1;

        public SampledData2f(float x0y0, float x0y1, float x1y0, float x1y1)
        {
            this.x0y0 = x0y0;
            this.x0y1 = x0y1;
            this.x1y0 = x1y0;
            this.x1y1 = x1y1;
        }

        public static bool operator !=(SampledData2f a, SampledData2f b)
        {
            return a.x0y0 != b.x0y0 && a.x0y1 != b.x0y1 && a.x1y0 != b.x1y0 && a.x1y1 != b.x1y1;
        }

        public static bool operator ==(SampledData2f a, SampledData2f b)
        {
            return a.x0y0 == b.x0y0 && a.x0y1 == b.x0y1 && a.x1y0 == b.x1y0 && a.x1y1 == b.x1y1;
        }

        public override bool Equals(object obj)
        {
            if (obj is SampledData2f)
            {
                var that = (SampledData2f)obj;
                return this.Equals(that);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(SampledData2f that)
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

        public override string ToString()
        {
            return string.Format("x0y0:{0} , x0y1:{1} , x1y0:{2} , x1y1:{3}", x0y0, x0y1, x1y0, x1y1);
        }
    }

    public static class SampledData2fExtensions
    {
        public static SampledData2f ToSampledData2F(this SampledData2h data)
        {
            return new SampledData2f(data.x0y0.RawValue, data.x0y1.RawValue, data.x1y0.RawValue, data.x1y1.RawValue);
        }

        public static SampledData2f ToSampledData2F(this SampledMask2e data)
        {
            return new SampledData2f(MaskToHeight(data.x0y0), MaskToHeight(data.x0y1), MaskToHeight(data.x1y0), MaskToHeight(data.x1y1));
        }

        private static float MaskToHeight(this MaskValues data)
        {
            return data.IsHole() ? -1 : 1;
        }
    }
}