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

        public bool IsEmpty => this.Size == Vector1i.Zero;

        private Range1i(int min, int max)
        {
            this.Min = Vector1iUtils.Max(min, MinValue);
            this.Max = Vector1iUtils.Min(max, MaxValue);
            if (this.Size.AnyNegative())
            {
                throw new InvalidOperationException($"{nameof(Range1i)} '{this}' cannot have a size be zero or negative!");
            }

            if (this.Size.AnyZero())
            {
                this.Max = this.Min;
            }
        }

        public Range1i ExtendBothDirections(int distance)
        {
            if (this.IsEmpty)
            {
                return this;
            }

            return FromMinAndMax(this.Min - distance, this.Max + distance);
        }

        public int Size => this.Max - this.Min;

        public static Range1i FromMinAndSize(int min, int size)
        {
            if (size.AnyNegative())
            {
                throw new InvalidOperationException($"When creating {nameof(Range1i)} using min '{min}' and size '{size}', size cannot have a negative coordinate!");
            }

            return new Range1i(min, min + size);
        }

        public static Range1i FromMinAndMax(int min, int max) => new Range1i(min, max);

        public static Range1i FromCenterAndExtents(int center, int extents)
        {
            if (extents.AnyNegative())
            {
                throw new InvalidOperationException($"When creating {nameof(Range1i)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            }

            return new Range1i((center - extents) + 1, center + extents);
        }

        public Area1i? ToArea1i()
        {
            if (this.Size == Vector1i.Zero)
            {
                return null;
            }

            return Area1i.FromMinAndMax(this.Min, this.Max - Vector1i.One);
        }

        public Area1f? ToArea1f()
        {
            if (this.Size == Vector1i.Zero)
            {
                return null;
            }

            return Area1f.FromMinAndMax(this.Min.ToVector1f(), (this.Max - Vector1i.One).ToVector1f());
        }

        public static bool operator ==(Range1i a, Range1i b) => (a.Min == b.Min) && (a.Max == b.Max);

        public static bool operator !=(Range1i a, Range1i b) => (a.Min != b.Min) || (a.Max != b.Max);

        public bool Contains(int point) => (point >= this.Min) && (point < this.Max);

        public bool Overlaps(Range1i that)
        {
            if ((this.Size == Vector1i.Zero) || (that.Size == Vector1i.Zero))
            {
                return false;
            }

            return (this.Min < that.Max) && (that.Min < this.Max);
        }

        public Range1i CombineWith(Range1i that)
        {
            if (this.Size == Vector1i.Zero)
            {
                return that;
            }

            if (that.Size == Vector1i.Zero)
            {
                return this;
            }

            var min = Vector1iUtils.Min(this.Min, that.Min);
            var max = Vector1iUtils.Max(this.Max, that.Max);
            return FromMinAndMax(min, max);
        }

        public Range1i CombineWith(int point)
        {
            if (this.Contains(point))
            {
                return this;
            }

            var min = Vector1iUtils.Min(this.Min, point);
            var max = Vector1iUtils.Max(this.Max, point);

            return FromMinAndMax(min, max);
        }

        public Range1i IntersectWith(Range1i that)
        {
            if ((this.Size == Vector1i.Zero) || (that.Size == Vector1i.Zero))
            {
                return Zero;
            }

            var min = Vector1iUtils.Max(this.Min, that.Min);
            var max = Vector1iUtils.Max(Vector1iUtils.Min(this.Max, that.Max), min);

            return FromMinAndMax(min, max);
        }

        public Range1i UnionWith(Range1i? that)
        {
            if (that == null)
            {
                return this;
            }

            return this.UnionWith(that.Value);
        }

        public Range1i UnionWith(Range1i that)
        {
            if (this.Size == Vector1i.Zero)
            {
                return that;
            }

            if (that.Size == Vector1i.Zero)
            {
                return this;
            }

            var min = Vector1iUtils.Min(this.Min, that.Min);
            var max = Vector1iUtils.Max(this.Max, that.Max);

            return FromMinAndMax(min, max);
        }

        public Area2f ToArea2fFromMinMax(float minZ, float maxZ) => Area2f.FromMinAndMax(this.Min.ToVector2f(minZ), this.Max.ToVector2f(maxZ));

        public bool Equals(Range1i other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Range1i))
            {
                return false;
            }

            return this.Equals((Range1i)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.Min.GetHashCode() + (7 * this.Max.GetHashCode());
            }
        }

        public override string ToString() => $"Range1i: min={this.Min} max={this.Max} size={this.Size}";
    }
}
