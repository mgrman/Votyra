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

        public static readonly Range5i Zero = new Range5i(Vector5i.Zero, Vector5i.Zero);

        public readonly Vector5i Min;

        public readonly Vector5i Max;

        public bool IsEmpty => this.Size == Vector5i.Zero;

        private Range5i(Vector5i min, Vector5i max)
        {
            this.Min = Vector5iUtils.Max(min, MinValue);
            this.Max = Vector5iUtils.Min(max, MaxValue);
            if (this.Size.AnyNegative())
            {
                throw new InvalidOperationException($"{nameof(Range5i)} '{this}' cannot have a size be zero or negative!");
            }

            if (this.Size.AnyZero())
            {
                this.Max = this.Min;
            }
        }

        public Range5i ExtendBothDirections(int distance)
        {
            if (this.IsEmpty)
            {
                return this;
            }

            return FromMinAndMax(this.Min - distance, this.Max + distance);
        }

        public Vector5i Size => this.Max - this.Min;

        public static Range5i FromMinAndSize(Vector5i min, Vector5i size)
        {
            if (size.AnyNegative())
            {
                throw new InvalidOperationException($"When creating {nameof(Range5i)} using min '{min}' and size '{size}', size cannot have a negative coordinate!");
            }

            return new Range5i(min, min + size);
        }

        public static Range5i FromMinAndMax(Vector5i min, Vector5i max) => new Range5i(min, max);

        public static Range5i FromCenterAndExtents(Vector5i center, Vector5i extents)
        {
            if (extents.AnyNegative())
            {
                throw new InvalidOperationException($"When creating {nameof(Range5i)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            }

            return new Range5i((center - extents) + 1, center + extents);
        }

        public Area5i? ToArea5i()
        {
            if (this.Size == Vector5i.Zero)
            {
                return null;
            }

            return Area5i.FromMinAndMax(this.Min, this.Max - Vector5i.One);
        }

        public Area5f? ToArea5f()
        {
            if (this.Size == Vector5i.Zero)
            {
                return null;
            }

            return Area5f.FromMinAndMax(this.Min.ToVector5f(), (this.Max - Vector5i.One).ToVector5f());
        }

        public static bool operator ==(Range5i a, Range5i b) => (a.Min == b.Min) && (a.Max == b.Max);

        public static bool operator !=(Range5i a, Range5i b) => (a.Min != b.Min) || (a.Max != b.Max);

        public bool Contains(Vector5i point) => (point >= this.Min) && (point < this.Max);

        public bool Overlaps(Range5i that)
        {
            if ((this.Size == Vector5i.Zero) || (that.Size == Vector5i.Zero))
            {
                return false;
            }

            return (this.Min < that.Max) && (that.Min < this.Max);
        }

        public Range5i CombineWith(Range5i that)
        {
            if (this.Size == Vector5i.Zero)
            {
                return that;
            }

            if (that.Size == Vector5i.Zero)
            {
                return this;
            }

            var min = Vector5iUtils.Min(this.Min, that.Min);
            var max = Vector5iUtils.Max(this.Max, that.Max);
            return FromMinAndMax(min, max);
        }

        public Range5i CombineWith(Vector5i point)
        {
            if (this.Contains(point))
            {
                return this;
            }

            var min = Vector5iUtils.Min(this.Min, point);
            var max = Vector5iUtils.Max(this.Max, point);

            return FromMinAndMax(min, max);
        }

        public Range5i IntersectWith(Range5i that)
        {
            if ((this.Size == Vector5i.Zero) || (that.Size == Vector5i.Zero))
            {
                return Zero;
            }

            var min = Vector5iUtils.Max(this.Min, that.Min);
            var max = Vector5iUtils.Max(Vector5iUtils.Min(this.Max, that.Max), min);

            return FromMinAndMax(min, max);
        }

        public Range5i UnionWith(Range5i? that)
        {
            if (that == null)
            {
                return this;
            }

            return this.UnionWith(that.Value);
        }

        public Range5i UnionWith(Range5i that)
        {
            if (this.Size == Vector5i.Zero)
            {
                return that;
            }

            if (that.Size == Vector5i.Zero)
            {
                return this;
            }

            var min = Vector5iUtils.Min(this.Min, that.Min);
            var max = Vector5iUtils.Max(this.Max, that.Max);

            return FromMinAndMax(min, max);
        }

        public Area6f ToArea6fFromMinMax(float minZ, float maxZ) => Area6f.FromMinAndMax(this.Min.ToVector6f(minZ), this.Max.ToVector6f(maxZ));

        public bool Equals(Range5i other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Range5i))
            {
                return false;
            }

            return this.Equals((Range5i)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.Min.GetHashCode() + (7 * this.Max.GetHashCode());
            }
        }

        public override string ToString() => $"Range5i: min={this.Min} max={this.Max} size={this.Size}";
    }
}
