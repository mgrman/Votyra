using System;

namespace Votyra.Core.Models
{
    public struct Rect2f : IEquatable<Rect2f>
    {
        public static readonly Rect2f Zero = new Rect2f();
        public static readonly Rect2f All = new Rect2f(Vector2f.FromSame(float.MinValue / 2), Vector2f.FromSame(float.MaxValue / 2));

        public readonly Vector2f Max;
        public readonly Vector2f Min;
        public Vector2f Center => (Max + Min) / 2f;
        public Vector2f Size => Max - Min;
        public Vector2f Extents => Size / 2;

        private Rect2f(Vector2f min, Vector2f max)
        {
            this.Min = min;
            this.Max = max;
        }

        public Rect2f FromCenterAndSize(Vector2f center, Vector2f size)
        {
            return new Rect2f(center - size / 2, size);
        }

        public static Rect2f FromMinAndMax(Vector2f min, Vector2f max)
        {
            return new Rect2f(min, max);
        }

        public static Rect2f FromMinAndSize(Vector2f min, Vector2f size)
        {
            return new Rect2f(min, min + size);
        }

        public bool Contains(Vector2f point)
        {
            return point >= Min && point <= Max;
        }

        public static bool operator ==(Rect2f a, Rect2f b)
        {
            return a.Center == b.Center && a.Size == b.Size;
        }

        public static bool operator !=(Rect2f a, Rect2f b)
        {
            return a.Center != b.Center || a.Size != b.Size;
        }

        public Rect2i RoundToInt()
        {
            return Rect2i.FromMinAndMax(this.Min.RoundToVector2i(), this.Max.RoundToVector2i());
        }

        public Rect2i RoundToContain()
        {
            return Rect2i.FromMinAndMax(this.Min.FloorToVector2i(), this.Max.CeilToVector2i());
        }

        public Rect2f CombineWith(Rect2f that)
        {
            if (this.Size == Vector2f.Zero)
                return that;

            if (that.Size == Vector2f.Zero)
                return this;

            var min = Vector2f.Min(this.Min, that.Min);
            var max = Vector2f.Max(this.Max, that.Max);
            return Rect2f.FromMinAndMax(min, max);
        }

        public bool Equals(Rect2f other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Rect2f))
                return false;

            return this.Equals((Rect2f)obj);
        }

        public override int GetHashCode()
        {
            return Center.GetHashCode() + 7 * Size.GetHashCode();
        }

        public override string ToString()
        {
            return $"center:{Center} size:{Size}";
        }
    }
}