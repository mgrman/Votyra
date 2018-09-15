using System;

namespace Votyra.Core.Models
{
    public struct Range2f : IEquatable<Range2f>
    {
        public static readonly Range2f Zero = new Range2f();
        public static readonly Range2f All = new Range2f(Vector2f.FromSame(float.MinValue / 2), Vector2f.FromSame(float.MaxValue / 2));

        public readonly Vector2f Max;
        public readonly Vector2f Min;

        private Range2f(Vector2f min, Vector2f max)
        {
            this.Min = min;
            this.Max = max;
        }

        public Vector2f Center => (Max + Min) / 2f;
        public Vector2f Size => Max - Min;
        public Vector2f Extents => Size / 2;

        public static Range2f FromMinAndMax(Vector2f min, Vector2f max)
        {
            return new Range2f(min, max);
        }

        public static Range2f FromMinAndSize(Vector2f min, Vector2f size)
        {
            return new Range2f(min, min + size);
        }

        public static bool operator ==(Range2f a, Range2f b)
        {
            return a.Center == b.Center && a.Size == b.Size;
        }

        public static bool operator !=(Range2f a, Range2f b)
        {
            return a.Center != b.Center || a.Size != b.Size;
        }

        public static Range2f operator /(Range2f a, float b)
        {
            return Range2f.FromMinAndMax(a.Min / b, a.Max / b);
        }

        public static Range2f operator /(Range2f a, Vector2f b)
        {
            return Range2f.FromMinAndMax(a.Min / b, a.Max / b);
        }

        public Range2f FromCenterAndSize(Vector2f center, Vector2f size)
        {
            return new Range2f(center - size / 2, size);
        }

        public Range2f Encapsulate(Vector2f point)
        {
            return Range2f.FromMinAndMax(Vector2f.Min(this.Min, point), Vector2f.Max(this.Max, point));
        }

        public Range2f Encapsulate(Range2f bounds)
        {
            return Range2f.FromMinAndMax(Vector2f.Min(this.Min, bounds.Min), Vector2f.Max(this.Max, bounds.Max));
        }

        public bool Contains(Vector2f point)
        {
            return point >= Min && point <= Max;
        }

        public Range2i RoundToInt()
        {
            return Range2i.FromMinAndMax(this.Min.RoundToVector2i(), this.Max.RoundToVector2i());
        }

        public Range2i RoundToContain()
        {
            return Range2i.FromMinAndMax(this.Min.FloorToVector2i(), this.Max.CeilToVector2i());
        }

        public Range2f CombineWith(Range2f that)
        {
            if (this.Size == Vector2f.Zero)
                return that;

            if (that.Size == Vector2f.Zero)
                return this;

            var min = Vector2f.Min(this.Min, that.Min);
            var max = Vector2f.Max(this.Max, that.Max);
            return Range2f.FromMinAndMax(min, max);
        }

        public bool Equals(Range2f other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Range2f))
                return false;

            return this.Equals((Range2f)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Center.GetHashCode() + 7 * Size.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"center:{Center} size:{Size}";
        }
    }
}