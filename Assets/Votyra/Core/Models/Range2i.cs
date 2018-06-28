using System;

namespace Votyra.Core.Models
{
    /// <summary>
    /// Max is exclusive
    /// </summary>
    public struct Range2i : IEquatable<Range2i>
    {
        public static readonly Range2i All = new Range2i(Vector2i.FromSame(int.MinValue / 2), Vector2i.FromSame(int.MaxValue) / 2);

        public static readonly Range2i Zero = new Range2i();

        public readonly Vector2i Min;

        public readonly Vector2i Max;

        public Vector2i Size => Max - Min;

        private Range2i(Vector2i min, Vector2i max)
        {
            this.Min = min;
            this.Max = max;
            if (this.Size.AnyNegative)
            {
                throw new InvalidOperationException($"{nameof(Range2i)} '{this}' cannot have a size be zero or negative!");
            }
            if (this.Size.AnyZero)
            {
                this.Max = this.Min;
            }
        }

        public static Range2i FromCenterAndExtents(Vector2i center, Vector2i extents)
        {
            if (extents.AnyNegative)
            {
                throw new InvalidOperationException($"When creating {nameof(Range2i)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            }
            return new Range2i(center - extents + 1, center + extents);
        }

        public static Range2i FromMinAndSize(Vector2i min, Vector2i size)
        {
            if (size.AnyNegative)
            {
                throw new InvalidOperationException($"When creating {nameof(Range2i)} using min '{min}' and size '{size}', size cannot have a negative coordinate!");
            }
            return new Range2i(min, min + size);
        }

        public static Range2i FromMinAndMax(Vector2i min, Vector2i max)
        {
            return new Range2i(min, max);
        }

        public void ForeachPointExlusive(Action<Vector2i> action)
        {
            var min = Min;
            Size.ForeachPointExlusive(i => action(i + min));
        }

        public bool Contains(Vector2i point)
        {
            return point >= Min && point < Max;
        }

        public bool Overlaps(Range2i that)
        {
            if (this.Size == Vector2i.Zero || that.Size == Vector2i.Zero)
                return false;

            return this.Min <= that.Max && that.Min <= this.Max;
        }

        public Range2i CombineWith(Range2i that)
        {
            if (this.Size == Vector2i.Zero)
                return that;

            if (that.Size == Vector2i.Zero)
                return this;

            var min = Vector2i.Min(this.Min, that.Min);
            var max = Vector2i.Max(this.Max, that.Max);
            return Range2i.FromMinAndMax(min, max);
        }

        public Range2i CombineWith(Vector2i point)
        {
            if (Contains(point))
                return this;

            var min = Vector2i.Min(this.Min, point);
            var max = Vector2i.Max(this.Max, point);

            return Range2i.FromMinAndMax(min, max);
        }

        public Range2f ToBounds()
        {
            return Range2f.FromMinAndMax(Min.ToVector2f(), Max.ToVector2f());
        }

        public Range2i IntersectWith(Range2i that)
        {
            if (this.Size == Vector2i.Zero || that.Size == Vector2i.Zero)
                return Range2i.Zero;

            var min = Vector2i.Max(this.Min, that.Min);
            var max = Vector2i.Max(Vector2i.Min(this.Max, that.Max), min);

            return Range2i.FromMinAndMax(min, max);
        }

        public static bool operator ==(Range2i a, Range2i b)
        {
            return a.Min == b.Min && a.Max == b.Max;
        }

        public static bool operator !=(Range2i a, Range2i b)
        {
            return a.Min != b.Min || a.Max != b.Max;
        }

        public bool Equals(Range2i other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Range2i))
                return false;

            return this.Equals((Range2i)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Min.GetHashCode() + 7 * Max.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"min:{Min} max:{Max} size:{Size}";
        }
    }
}
