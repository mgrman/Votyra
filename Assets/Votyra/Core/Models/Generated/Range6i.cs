using System;

namespace Votyra.Core.Models
{
    /// <summary>
    ///     Max is exclusive
    /// </summary>
    public struct Range6i : IEquatable<Range6i>
    {
        public static readonly Vector6i MinValue = Vector6iUtils.FromSame(int.MinValue / 2);
        public static readonly Vector6i MaxValue = Vector6iUtils.FromSame(int.MaxValue / 2);

        public static readonly Range6i All = new Range6i(MinValue, MaxValue);

        public static readonly Range6i Zero = new Range6i();

        public readonly Vector6i Min;

        public readonly Vector6i Max;

        public bool IsEmpty => Size == Vector6i.Zero;

        private Range6i(Vector6i min, Vector6i max)
        {
            Min = Vector6iUtils.Max(min, MinValue);
            Max = Vector6iUtils.Min(max, MaxValue);
            if (Size.AnyNegative())
                throw new InvalidOperationException($"{nameof(Range6i)} '{this}' cannot have a size be zero or negative!");
            if (Size.AnyZero())
                Max = Min;
        }

        public Range6i ExtendBothDirections(int distance)
        {
            if (IsEmpty)
                return this;
            return FromMinAndMax(Min - distance, Max + distance);
        }

        public Vector6i Size => Max - Min;

        public static Range6i FromMinAndSize(Vector6i min, Vector6i size)
        {
            if (size.AnyNegative())
                throw new InvalidOperationException($"When creating {nameof(Range6i)} using min '{min}' and size '{size}', size cannot have a negative coordinate!");
            return new Range6i(min, min + size);
        }

        public static Range6i FromMinAndMax(Vector6i min, Vector6i max) => new Range6i(min, max);

        public static Range6i FromCenterAndExtents(Vector6i center, Vector6i extents)
        {
            if (extents.AnyNegative())
                throw new InvalidOperationException($"When creating {nameof(Range6i)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            return new Range6i(center - extents + 1, center + extents);
        }

        public Area6i? ToArea6i()
        {
            if (Size == Vector6i.Zero)
                return null;
            return Area6i.FromMinAndMax(Min, Max - Vector6i.One);
        }

        public Area6f? ToArea6f()
        {
            if (Size == Vector6i.Zero)
                return null;
            return Area6f.FromMinAndMax(Min.ToVector6f(), (Max - Vector6i.One).ToVector6f());
        }

        public static bool operator ==(Range6i a, Range6i b) => a.Min == b.Min && a.Max == b.Max;

        public static bool operator !=(Range6i a, Range6i b) => a.Min != b.Min || a.Max != b.Max;

        public bool Contains(Vector6i point) => point >= Min && point < Max;

        public bool Overlaps(Range6i that)
        {
            if (Size == Vector6i.Zero || that.Size == Vector6i.Zero)
                return false;

            return Min < that.Max && that.Min < Max;
        }

        public Range6i CombineWith(Range6i that)
        {
            if (Size == Vector6i.Zero)
                return that;

            if (that.Size == Vector6i.Zero)
                return this;

            var min = Vector6iUtils.Min(Min, that.Min);
            var max = Vector6iUtils.Max(Max, that.Max);
            return FromMinAndMax(min, max);
        }

        public Range6i CombineWith(Vector6i point)
        {
            if (Contains(point))
                return this;

            var min = Vector6iUtils.Min(Min, point);
            var max = Vector6iUtils.Max(Max, point);

            return FromMinAndMax(min, max);
        }

        public Range6i IntersectWith(Range6i that)
        {
            if (Size == Vector6i.Zero || that.Size == Vector6i.Zero)
                return Zero;

            var min = Vector6iUtils.Max(Min, that.Min);
            var max = Vector6iUtils.Max(Vector6iUtils.Min(Max, that.Max), min);

            return FromMinAndMax(min, max);
        }

        public Range6i UnionWith(Range6i? that)
        {
            if (that == null)
                return this;
            return UnionWith(that.Value);
        }

        public Range6i UnionWith(Range6i that)
        {
            if (Size == Vector6i.Zero)
                return that;
            if (that.Size == Vector6i.Zero)
                return this;

            var min = Vector6iUtils.Min(Min, that.Min);
            var max = Vector6iUtils.Max(Max, that.Max);

            return FromMinAndMax(min, max);
        }

        public bool Equals(Range6i other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Range6i))
                return false;

            return Equals((Range6i) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Min.GetHashCode() + 7 * Max.GetHashCode();
            }
        }

        public override string ToString() => $"Range6i: min={Min} max={Max} size={Size}";
    }
}