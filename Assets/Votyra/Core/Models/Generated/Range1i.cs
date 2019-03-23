using System;

namespace Votyra.Core.Models
{
    /// <summary>
    ///     Max is exclusive
    /// </summary>
    public struct Range1i : IEquatable<Range1i>
    {
        public static readonly int MinValue = Vector1iUtils.FromSame(int.MinValue / 2);
        public static readonly int MaxValue = Vector1iUtils.FromSame(int.MaxValue / 2);

        public static readonly Range1i All = new Range1i(MinValue, MaxValue);

        public static readonly Range1i Zero = new Range1i();

        public readonly int Min;

        public readonly int Max;

        public bool IsEmpty => Size == Vector1i.Zero;

        private Range1i(int min, int max)
        {
            Min = Vector1iUtils.Max(min, MinValue);
            Max = Vector1iUtils.Min(max, MaxValue);
            if (Size.AnyNegative())
                throw new InvalidOperationException($"{nameof(Range1i)} '{this}' cannot have a size be zero or negative!");
            if (Size.AnyZero())
                Max = Min;
        }

        public Range1i ExtendBothDirections(int distance)
        {
            if (IsEmpty)
                return this;
            return FromMinAndMax(Min - distance, Max + distance);
        }

        public int Size => Max - Min;

        public static Range1i FromMinAndSize(int min, int size)
        {
            if (size.AnyNegative())
                throw new InvalidOperationException($"When creating {nameof(Range1i)} using min '{min}' and size '{size}', size cannot have a negative coordinate!");
            return new Range1i(min, min + size);
        }

        public static Range1i FromMinAndMax(int min, int max) => new Range1i(min, max);

        public static Range1i FromCenterAndExtents(int center, int extents)
        {
            if (extents.AnyNegative())
                throw new InvalidOperationException($"When creating {nameof(Range1i)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            return new Range1i(center - extents + 1, center + extents);
        }

        public Area1i? ToArea1i()
        {
            if (Size == Vector1i.Zero)
                return null;
            return Area1i.FromMinAndMax(Min, Max - Vector1i.One);
        }

        public Area1f? ToArea1f()
        {
            if (Size == Vector1i.Zero)
                return null;
            return Area1f.FromMinAndMax(Min.ToVector1f(), (Max - Vector1i.One).ToVector1f());
        }

        public static bool operator ==(Range1i a, Range1i b) => a.Min == b.Min && a.Max == b.Max;

        public static bool operator !=(Range1i a, Range1i b) => a.Min != b.Min || a.Max != b.Max;

        public bool Contains(int point) => point >= Min && point < Max;

        public bool Overlaps(Range1i that)
        {
            if (Size == Vector1i.Zero || that.Size == Vector1i.Zero)
                return false;

            return Min < that.Max && that.Min < Max;
        }

        public Range1i CombineWith(Range1i that)
        {
            if (Size == Vector1i.Zero)
                return that;

            if (that.Size == Vector1i.Zero)
                return this;

            var min = Vector1iUtils.Min(Min, that.Min);
            var max = Vector1iUtils.Max(Max, that.Max);
            return FromMinAndMax(min, max);
        }

        public Range1i CombineWith(int point)
        {
            if (Contains(point))
                return this;

            var min = Vector1iUtils.Min(Min, point);
            var max = Vector1iUtils.Max(Max, point);

            return FromMinAndMax(min, max);
        }

        public Range1i IntersectWith(Range1i that)
        {
            if (Size == Vector1i.Zero || that.Size == Vector1i.Zero)
                return Zero;

            var min = Vector1iUtils.Max(Min, that.Min);
            var max = Vector1iUtils.Max(Vector1iUtils.Min(Max, that.Max), min);

            return FromMinAndMax(min, max);
        }

        public Range1i UnionWith(Range1i? that)
        {
            if (that == null)
                return this;
            return UnionWith(that.Value);
        }

        public Range1i UnionWith(Range1i that)
        {
            if (Size == Vector1i.Zero)
                return that;
            if (that.Size == Vector1i.Zero)
                return this;

            var min = Vector1iUtils.Min(Min, that.Min);
            var max = Vector1iUtils.Max(Max, that.Max);

            return FromMinAndMax(min, max);
        }

        public Area2f ToArea2fFromMinMax(float minZ, float maxZ) => Area2f.FromMinAndMax(Min.ToVector2f(minZ), Max.ToVector2f(maxZ));

        public bool Equals(Range1i other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Range1i))
                return false;

            return Equals((Range1i) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Min.GetHashCode() + 7 * Max.GetHashCode();
            }
        }

        public override string ToString() => $"Range1i: min={Min} max={Max} size={Size}";
    }
}