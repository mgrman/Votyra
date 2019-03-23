using System;

namespace Votyra.Core.Models
{
    /// <summary>
    ///     Max is exclusive
    /// </summary>
    public struct Range2i : IEquatable<Range2i>
    {
        public static readonly Vector2i MinValue = Vector2iUtils.FromSame(int.MinValue / 2);
        public static readonly Vector2i MaxValue = Vector2iUtils.FromSame(int.MaxValue / 2);

        public static readonly Range2i All = new Range2i(MinValue, MaxValue);

        public static readonly Range2i Zero = new Range2i();

        public readonly Vector2i Min;

        public readonly Vector2i Max;

        public bool IsEmpty => Size == Vector2i.Zero;

        private Range2i(Vector2i min, Vector2i max)
        {
            Min = Vector2iUtils.Max(min, MinValue);
            Max = Vector2iUtils.Min(max, MaxValue);
            if (Size.AnyNegative())
                throw new InvalidOperationException($"{nameof(Range2i)} '{this}' cannot have a size be zero or negative!");
            if (Size.AnyZero())
                Max = Min;
        }

        public Range2i ExtendBothDirections(int distance)
        {
            if (IsEmpty)
                return this;
            return FromMinAndMax(Min - distance, Max + distance);
        }

        public Vector2i Size => Max - Min;

        public static Range2i FromMinAndSize(Vector2i min, Vector2i size)
        {
            if (size.AnyNegative())
                throw new InvalidOperationException($"When creating {nameof(Range2i)} using min '{min}' and size '{size}', size cannot have a negative coordinate!");
            return new Range2i(min, min + size);
        }

        public static Range2i FromMinAndMax(Vector2i min, Vector2i max) => new Range2i(min, max);

        public static Range2i FromCenterAndExtents(Vector2i center, Vector2i extents)
        {
            if (extents.AnyNegative())
                throw new InvalidOperationException($"When creating {nameof(Range2i)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            return new Range2i(center - extents + 1, center + extents);
        }

        public Area2i? ToArea2i()
        {
            if (Size == Vector2i.Zero)
                return null;
            return Area2i.FromMinAndMax(Min, Max - Vector2i.One);
        }

        public Area2f? ToArea2f()
        {
            if (Size == Vector2i.Zero)
                return null;
            return Area2f.FromMinAndMax(Min.ToVector2f(), (Max - Vector2i.One).ToVector2f());
        }

        public static bool operator ==(Range2i a, Range2i b) => a.Min == b.Min && a.Max == b.Max;

        public static bool operator !=(Range2i a, Range2i b) => a.Min != b.Min || a.Max != b.Max;

        public bool Contains(Vector2i point) => point >= Min && point < Max;

        public bool Overlaps(Range2i that)
        {
            if (Size == Vector2i.Zero || that.Size == Vector2i.Zero)
                return false;

            return Min < that.Max && that.Min < Max;
        }

        public Range2i CombineWith(Range2i that)
        {
            if (Size == Vector2i.Zero)
                return that;

            if (that.Size == Vector2i.Zero)
                return this;

            var min = Vector2iUtils.Min(Min, that.Min);
            var max = Vector2iUtils.Max(Max, that.Max);
            return FromMinAndMax(min, max);
        }

        public Range2i CombineWith(Vector2i point)
        {
            if (Contains(point))
                return this;

            var min = Vector2iUtils.Min(Min, point);
            var max = Vector2iUtils.Max(Max, point);

            return FromMinAndMax(min, max);
        }

        public Range2i IntersectWith(Range2i that)
        {
            if (Size == Vector2i.Zero || that.Size == Vector2i.Zero)
                return Zero;

            var min = Vector2iUtils.Max(Min, that.Min);
            var max = Vector2iUtils.Max(Vector2iUtils.Min(Max, that.Max), min);

            return FromMinAndMax(min, max);
        }

        public Range2i UnionWith(Range2i? that)
        {
            if (that == null)
                return this;
            return UnionWith(that.Value);
        }

        public Range2i UnionWith(Range2i that)
        {
            if (Size == Vector2i.Zero)
                return that;
            if (that.Size == Vector2i.Zero)
                return this;

            var min = Vector2iUtils.Min(Min, that.Min);
            var max = Vector2iUtils.Max(Max, that.Max);

            return FromMinAndMax(min, max);
        }

        public Area3f ToArea3fFromMinMax(float minZ, float maxZ) => Area3f.FromMinAndMax(Min.ToVector3f(minZ), Max.ToVector3f(maxZ));

        public bool Equals(Range2i other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Range2i))
                return false;

            return Equals((Range2i) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Min.GetHashCode() + 7 * Max.GetHashCode();
            }
        }

        public override string ToString() => $"Range2i: min={Min} max={Max} size={Size}";
    }
}