using System;

namespace Votyra.Core.Models
{
    public struct Rect2i : IEquatable<Rect2i>
    {
        public static readonly Rect2i All = new Rect2i(Vector2i.FromSame(int.MinValue / 2), Vector2i.FromSame(int.MaxValue) / 2);
        public static Rect2i Zero { get; } = new Rect2i();

        public readonly Vector2i Min;
        public readonly Vector2i Max;
        public Vector2i Size => Max - Min;

        private Rect2i(Vector2i min, Vector2i size)
        {
            this.Min = min;
            this.Max = min + size;
        }

        public static Rect2i FromCenterAndExtents(Vector2i center, Vector2i extents)
        {
            if (extents.AnyNegative)
            {
                throw new InvalidOperationException($"When creating {nameof(Rect2i)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            }
            return new Rect2i(center - extents + 1, center + extents);

            //return new Rect2i(center - extents, center + Vector2i.One + extents);
        }

        public static Rect2i FromMinAndSize(Vector2i min, Vector2i size)
        {
            if (size.AnyNegative)
            {
                throw new InvalidOperationException($"When creating {nameof(Rect2i)} using min '{min}' and size '{size}', size cannot have a negative coordinate!");
            }
            return new Rect2i(min, min + size);
        }

        public static Rect2i FromMinAndMax(Vector2i min, Vector2i max)
        {
            return new Rect2i(min, max - min);
        }

        public bool Contains(Vector2i point)
        {
            return point >= Min && point <= Max;
        }

        public bool Overlaps(Rect2i that)
        {
            if (this.Size == Vector2i.Zero || that.Size == Vector2i.Zero)
                return false;

            bool overlapX = this.Min.X < that.Max.X && that.Min.X < this.Max.X;
            bool overlapY = this.Min.Y < that.Max.Y && that.Min.Y < this.Max.Y;
            return overlapX && overlapY;
        }

        public Rect2i CombineWith(Rect2i that)
        {
            if (this.Size == Vector2i.Zero)
                return that;

            if (that.Size == Vector2i.Zero)
                return this;

            var min = Vector2i.Min(this.Min, that.Min);
            var max = Vector2i.Max(this.Max, that.Max);
            return Rect2i.FromMinAndMax(min, max);
        }

        public Rect2i IntersectWith(Rect2i that)
        {
            if (this.Size == Vector2i.Zero || that.Size == Vector2i.Zero)
                return Rect2i.Zero;

            var min = Vector2i.Max(this.Min, that.Min);
            var max = Vector2i.Max(Vector2i.Min(this.Max, that.Max), min);

            return Rect2i.FromMinAndMax(min, max);
        }

        public Rect2i CombineWith(Vector2i point)
        {
            if (Contains(point))
                return this;

            var min = Vector2i.Min(this.Min, point);
            var max = Vector2i.Max(this.Max, point);

            return Rect2i.FromMinAndMax(min, max);
        }

        public Rect2f ToRect2f()
        {
            return Rect2f.FromMinAndMax(Min.ToVector2f(), Max.ToVector2f());
        }

        public static bool operator ==(Rect2i a, Rect2i b)
        {
            return a.Min == b.Min && a.Max == b.Max;
        }

        public static bool operator !=(Rect2i a, Rect2i b)
        {
            return a.Min != b.Min || a.Max != b.Max;
        }

        public bool Equals(Rect2i other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Rect2i))
                return false;

            return this.Equals((Rect2i)obj);
        }

        public override int GetHashCode()
        {
            return Min.GetHashCode() + 7 * Max.GetHashCode();
        }

        public override string ToString()
        {
            return $"min:{Min} max:{Max}";
        }
    }
}