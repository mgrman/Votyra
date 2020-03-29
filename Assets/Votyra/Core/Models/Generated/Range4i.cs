using System;

namespace Votyra.Core.Models
{
    /// <summary>
    ///     Max is exclusive
    /// </summary>
    public struct Range4i : IEquatable<Range4i>
    {
        public static readonly Vector4i MinValue = Vector4iUtils.FromSame(int.MinValue / 2);
        public static readonly Vector4i MaxValue = Vector4iUtils.FromSame(int.MaxValue / 2);

        public static readonly Range4i All = new Range4i(MinValue, MaxValue);

        public static readonly Range4i Zero = new Range4i();

        public readonly Vector4i Min;

        public readonly Vector4i Max;

        public bool IsEmpty => Size == Vector4i.Zero;

        private Range4i(Vector4i min, Vector4i max)
        {
            Min = Vector4iUtils.Max(min, MinValue);
            Max = Vector4iUtils.Min(max, MaxValue);
            if (Size.AnyNegative())
            {
                throw new InvalidOperationException($"{nameof(Range4i)} '{this}' cannot have a size be zero or negative!");
            }

            if (Size.AnyZero())
            {
                Max = Min;
            }
        }

        public Range4i ExtendBothDirections(int distance)
        {
            if (IsEmpty)
            {
                return this;
            }

            return FromMinAndMax(Min - distance, Max + distance);
        }

        public Vector4i Size => Max - Min;

        public static Range4i FromMinAndSize(Vector4i min, Vector4i size)
        {
            if (size.AnyNegative())
            {
                throw new InvalidOperationException($"When creating {nameof(Range4i)} using min '{min}' and size '{size}', size cannot have a negative coordinate!");
            }

            return new Range4i(min, min + size);
        }

        public static Range4i FromMinAndMax(Vector4i min, Vector4i max) => new Range4i(min, max);

        public static Range4i FromCenterAndExtents(Vector4i center, Vector4i extents)
        {
            if (extents.AnyNegative())
            {
                throw new InvalidOperationException($"When creating {nameof(Range4i)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            }

            return new Range4i(center - extents + 1, center + extents);
        }

        public Area4i? ToArea4i()
        {
            if (Size == Vector4i.Zero)
            {
                return null;
            }

            return Area4i.FromMinAndMax(Min, Max - Vector4i.One);
        }

        public Area4f? ToArea4f()
        {
            if (Size == Vector4i.Zero)
            {
                return null;
            }

            return Area4f.FromMinAndMax(Min.ToVector4f(), (Max - Vector4i.One).ToVector4f());
        }

        public static bool operator ==(Range4i a, Range4i b) => a.Min == b.Min && a.Max == b.Max;

        public static bool operator !=(Range4i a, Range4i b) => a.Min != b.Min || a.Max != b.Max;

        public bool Contains(Vector4i point) => point >= Min && point < Max;

        public bool Overlaps(Range4i that)
        {
            if (Size == Vector4i.Zero || that.Size == Vector4i.Zero)
            {
                return false;
            }

            return Min < that.Max && that.Min < Max;
        }

        public Range4i CombineWith(Range4i that)
        {
            if (Size == Vector4i.Zero)
            {
                return that;
            }

            if (that.Size == Vector4i.Zero)
            {
                return this;
            }

            var min = Vector4iUtils.Min(Min, that.Min);
            var max = Vector4iUtils.Max(Max, that.Max);
            return FromMinAndMax(min, max);
        }

        public Range4i CombineWith(Vector4i point)
        {
            if (Contains(point))
            {
                return this;
            }

            var min = Vector4iUtils.Min(Min, point);
            var max = Vector4iUtils.Max(Max, point);

            return FromMinAndMax(min, max);
        }

        public Range4i IntersectWith(Range4i that)
        {
            if (Size == Vector4i.Zero || that.Size == Vector4i.Zero)
            {
                return Zero;
            }

            var min = Vector4iUtils.Max(Min, that.Min);
            var max = Vector4iUtils.Max(Vector4iUtils.Min(Max, that.Max), min);

            return FromMinAndMax(min, max);
        }

        public Range4i UnionWith(Range4i? that)
        {
            if (that == null)
            {
                return this;
            }

            return UnionWith(that.Value);
        }

        public Range4i UnionWith(Range4i that)
        {
            if (Size == Vector4i.Zero)
            {
                return that;
            }

            if (that.Size == Vector4i.Zero)
            {
                return this;
            }

            var min = Vector4iUtils.Min(Min, that.Min);
            var max = Vector4iUtils.Max(Max, that.Max);

            return FromMinAndMax(min, max);
        }

        public Area5f ToArea5fFromMinMax(float minZ, float maxZ) => Area5f.FromMinAndMax(Min.ToVector5f(minZ), Max.ToVector5f(maxZ));

        public bool Equals(Range4i other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Range4i))
            {
                return false;
            }

            return Equals((Range4i) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Min.GetHashCode() + 7 * Max.GetHashCode();
            }
        }

        public override string ToString() => $"Range4i: min={Min} max={Max} size={Size}";
    }
}
