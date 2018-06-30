using System;

namespace Votyra.Core.Models
{
    /// <summary>
    /// Max is exclusive
    /// </summary>
    public struct Range3i : IEquatable<Range3i>
    {
        public static readonly Range3i All = new Range3i(Vector3i.FromSame(int.MinValue / 2), Vector3i.FromSame(int.MaxValue) / 2);

        public static readonly Range3i Zero = new Range3i();

        public readonly Vector3i Min;

        public readonly Vector3i Max;

        public Vector3i Size => Max - Min;

        private Range3i(Vector3i min, Vector3i max)
        {
            this.Min = min;
            this.Max = max;
            if (this.Size.AnyNegative)
            {
                throw new InvalidOperationException($"{nameof(Range3i)} '{this}' cannot have a size be zero or negative!");
            }
            if (this.Size.AnyZero)
            {
                this.Max = this.Min;
            }
        }

        public static Range3i FromCenterAndExtents(Vector3i center, Vector3i extents)
        {
            if (extents.AnyNegative)
            {
                throw new InvalidOperationException($"When creating {nameof(Range3i)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            }
            return new Range3i(center - extents + 1, center + extents);
        }

        public static Range3i FromMinAndSize(Vector3i min, Vector3i size)
        {
            if (size.AnyNegative)
            {
                throw new InvalidOperationException($"When creating {nameof(Range3i)} using min '{min}' and size '{size}', size cannot have a negative coordinate!");
            }
            return new Range3i(min, min + size);
        }

        public static Range3i FromMinAndMax(Vector3i min, Vector3i max)
        {
            return new Range3i(min, max);
        }

        public void ForeachPointExlusive(Action<Vector3i> action)
        {
            var min = Min;
            Size.ForeachPointExlusive(i => action(i + min));
        }

        public bool Contains(Vector3i point)
        {
            return point >= Min && point < Max;
        }

        public bool Overlaps(Range3i that)
        {
            if (this.Size == Vector3i.Zero || that.Size == Vector3i.Zero)
                return false;

            return this.Min <= that.Max && that.Min <= this.Max;
        }

        public Range3i CombineWith(Range3i that)
        {
            if (this.Size == Vector3i.Zero)
                return that;

            if (that.Size == Vector3i.Zero)
                return this;

            var min = Vector3i.Min(this.Min, that.Min);
            var max = Vector3i.Max(this.Max, that.Max);
            return Range3i.FromMinAndMax(min, max);
        }

        public Range3i CombineWith(Vector3i point)
        {
            if (this.Size == Vector3i.Zero)
                return new Range3i(point, Vector3i.One);

            if (Contains(point))
                return this;

            var min = Vector3i.Min(this.Min, point);
            var max = Vector3i.Max(this.Max, point);

            return Range3i.FromMinAndMax(min, max);
        }

        public Range3f ToRange3f()
        {
            return Range3f.FromMinAndMax(Min.ToVector3f(), Max.ToVector3f());
        }

        public Range3i IntersectWith(Range3i that)
        {
            if (this.Size == Vector3i.Zero || that.Size == Vector3i.Zero)
                return Range3i.Zero;

            var min = Vector3i.Max(this.Min, that.Min);
            var max = Vector3i.Max(Vector3i.Min(this.Max, that.Max), min);

            return Range3i.FromMinAndMax(min, max);
        }

        public static bool operator ==(Range3i a, Range3i b)
        {
            return a.Min == b.Min && a.Max == b.Max;
        }

        public static bool operator !=(Range3i a, Range3i b)
        {
            return a.Min != b.Min || a.Max != b.Max;
        }

        public bool Equals(Range3i other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Range3i))
                return false;

            return this.Equals((Range3i)obj);
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