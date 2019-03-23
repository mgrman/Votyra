using System;

namespace Votyra.Core.Models
{
    /// <summary>
    ///     Max is exclusive
    /// </summary>
    public struct Range5i : IEquatable<Range5i>
    {
        public static readonly Vector5i MinValue = Vector5iUtils.FromSame(int.MinValue / 2);
        public static readonly Vector5i MaxValue = Vector5iUtils.FromSame(int.MaxValue / 2);

        public static readonly Range5i All = new Range5i(MinValue, MaxValue);

        public static readonly Range5i Zero = new Range5i();

        public readonly Vector5i Min;

        public readonly Vector5i Max;

        public bool IsEmpty => Size == Vector5i.Zero;

        private Range5i(Vector5i min, Vector5i max)
        {
            Min = Vector5iUtils.Max(min, MinValue);
            Max = Vector5iUtils.Min(max, MaxValue);
            if (Size.AnyNegative())
                throw new InvalidOperationException($"{nameof(Range5i)} '{this}' cannot have a size be zero or negative!");
            if (Size.AnyZero())
                Max = Min;
        }

        public Range5i ExtendBothDirections(int distance)
        {
            if (IsEmpty)
                return this;
            return FromMinAndMax(Min - distance, Max + distance);
        }

        public Vector5i Size => Max - Min;

        public static Range5i FromMinAndSize(Vector5i min, Vector5i size)
        {
            if (size.AnyNegative())
                throw new InvalidOperationException($"When creating {nameof(Range5i)} using min '{min}' and size '{size}', size cannot have a negative coordinate!");
            return new Range5i(min, min + size);
        }

        public static Range5i FromMinAndMax(Vector5i min, Vector5i max) => new Range5i(min, max);

        public static Range5i FromCenterAndExtents(Vector5i center, Vector5i extents)
        {
            if (extents.AnyNegative())
                throw new InvalidOperationException($"When creating {nameof(Range5i)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            return new Range5i(center - extents + 1, center + extents);
        }

        public Area5i? ToArea5i()
        {
            if (Size == Vector5i.Zero)
                return null;
            return Area5i.FromMinAndMax(Min, Max - Vector5i.One);
        }

        public Area5f? ToArea5f()
        {
            if (Size == Vector5i.Zero)
                return null;
            return Area5f.FromMinAndMax(Min.ToVector5f(), (Max - Vector5i.One).ToVector5f());
        }

        public static bool operator ==(Range5i a, Range5i b) => a.Min == b.Min && a.Max == b.Max;

        public static bool operator !=(Range5i a, Range5i b) => a.Min != b.Min || a.Max != b.Max;

        public bool Contains(Vector5i point) => point >= Min && point < Max;

        public bool Overlaps(Range5i that)
        {
            if (Size == Vector5i.Zero || that.Size == Vector5i.Zero)
                return false;

            return Min < that.Max && that.Min < Max;
        }

        public Range5i CombineWith(Range5i that)
        {
            if (Size == Vector5i.Zero)
                return that;

            if (that.Size == Vector5i.Zero)
                return this;

            var min = Vector5iUtils.Min(Min, that.Min);
            var max = Vector5iUtils.Max(Max, that.Max);
            return FromMinAndMax(min, max);
        }

        public Range5i CombineWith(Vector5i point)
        {
            if (Contains(point))
                return this;

            var min = Vector5iUtils.Min(Min, point);
            var max = Vector5iUtils.Max(Max, point);

            return FromMinAndMax(min, max);
        }

        public Range5i IntersectWith(Range5i that)
        {
            if (Size == Vector5i.Zero || that.Size == Vector5i.Zero)
                return Zero;

            var min = Vector5iUtils.Max(Min, that.Min);
            var max = Vector5iUtils.Max(Vector5iUtils.Min(Max, that.Max), min);

            return FromMinAndMax(min, max);
        }

        public Range5i UnionWith(Range5i? that)
        {
            if (that == null)
                return this;
            return UnionWith(that.Value);
        }

        public Range5i UnionWith(Range5i that)
        {
            if (Size == Vector5i.Zero)
                return that;
            if (that.Size == Vector5i.Zero)
                return this;

            var min = Vector5iUtils.Min(Min, that.Min);
            var max = Vector5iUtils.Max(Max, that.Max);

            return FromMinAndMax(min, max);
        }

        public Area6f ToArea6fFromMinMax(float minZ, float maxZ) => Area6f.FromMinAndMax(Min.ToVector6f(minZ), Max.ToVector6f(maxZ));

        public bool Equals(Range5i other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Range5i))
                return false;

            return Equals((Range5i) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Min.GetHashCode() + 7 * Max.GetHashCode();
            }
        }

        public override string ToString() => $"Range5i: min={Min} max={Max} size={Size}";
    }
}