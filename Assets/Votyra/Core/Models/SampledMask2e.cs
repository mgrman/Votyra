using System;

namespace Votyra.Core.Models
{
    public struct SampledMask2e : IEquatable<SampledMask2e>
    {
        public static readonly SampledMask2e AllTerain = new SampledMask2e(MaskValues.Terrain, MaskValues.Terrain, MaskValues.Terrain, MaskValues.Terrain);
        public readonly MaskValues x0y0;
        public readonly MaskValues x0y1;
        public readonly MaskValues x1y0;
        public readonly MaskValues x1y1;

        public SampledMask2e(MaskValues x0y0, MaskValues x0y1, MaskValues x1y0, MaskValues x1y1)
        {
            this.x0y0 = x0y0;
            this.x0y1 = x0y1;
            this.x1y0 = x1y0;
            this.x1y1 = x1y1;
        }

        public static bool operator ==(SampledMask2e a, SampledMask2e b) => a.x0y0 == b.x0y0 && a.x0y1 == b.x0y1 && a.x1y0 == b.x1y0 && a.x1y1 == b.x1y1;

        public static bool operator !=(SampledMask2e a, SampledMask2e b) => a.x0y0 != b.x0y0 || a.x0y1 != b.x0y1 || a.x1y0 != b.x1y0 || a.x1y1 != b.x1y1;

        public int GetHoleCount() => (x0y0.IsHole() ? 1 : 0) + (x0y1.IsHole() ? 1 : 0) + (x1y0.IsHole() ? 1 : 0) + (x1y1.IsHole() ? 1 : 0);

        public override bool Equals(object obj)
        {
            if (obj is SampledMask2e)
            {
                var that = (SampledMask2e) obj;
                return Equals(that);
            }

            return false;
        }

        public bool Equals(SampledMask2e that) => this == that;

        public override int GetHashCode()
        {
            unchecked
            {
                return x0y0.GetHashCode() + x0y1.GetHashCode() * 7 + x1y0.GetHashCode() * 17 + x1y1.GetHashCode() * 31;
            }
        }

        public override string ToString() => string.Format("x0y0:{0} , x0y1:{1} , x1y0:{2} , x1y1:{3}", x0y0, x0y1, x1y0, x1y1);
    }
}