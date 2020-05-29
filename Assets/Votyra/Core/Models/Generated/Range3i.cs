using System;

namespace Votyra.Core.Models
{
    /// <summary>
    ///     Max is exclusive
    /// </summary>
    public struct Range3i : IEquatable<Range3i>
    {
        public static readonly Vector3i MinValue = Vector3iUtils.FromSame(int.MinValue / 2);
        public static readonly Vector3i MaxValue = Vector3iUtils.FromSame(int.MaxValue / 2);

        public static readonly Range3i All = new Range3i(MinValue, MaxValue);

        public static readonly Range3i Zero = new Range3i();

        public readonly Vector3i Min;

        public readonly Vector3i Max;

        public bool IsEmpty => this.Size == Vector3i.Zero;

        private Range3i(Vector3i min, Vector3i max)
        {
            this.Min = Vector3iUtils.Max(min, MinValue);
            this.Max = Vector3iUtils.Min(max, MaxValue);
            if (this.Size.AnyNegative())
            {
                throw new InvalidOperationException($"{nameof(Range3i)} '{this}' cannot have a size be zero or negative!");
            }

            if (this.Size.AnyZero())
            {
                this.Max = this.Min;
            }
        }

        public Range3i ExtendBothDirections(int distance)
        {
            if (this.IsEmpty)
            {
                return this;
            }

            return FromMinAndMax(this.Min - distance, this.Max + distance);
        }

        public Vector3i Size => this.Max - this.Min;

        public static Range3i FromMinAndSize(Vector3i min, Vector3i size)
        {
            if (size.AnyNegative())
            {
                throw new InvalidOperationException($"When creating {nameof(Range3i)} using min '{min}' and size '{size}', size cannot have a negative coordinate!");
            }

            return new Range3i(min, min + size);
        }

        public static Range3i FromMinAndMax(Vector3i min, Vector3i max) => new Range3i(min, max);

        public static Range3i FromCenterAndExtents(Vector3i center, Vector3i extents)
        {
            if (extents.AnyNegative())
            {
                throw new InvalidOperationException($"When creating {nameof(Range3i)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            }

            return new Range3i((center - extents) + 1, center + extents);
        }

        public Area3i? ToArea3i()
        {
            if (this.Size == Vector3i.Zero)
            {
                return null;
            }

            return Area3i.FromMinAndMax(this.Min, this.Max - Vector3i.One);
        }

        public Area3f? ToArea3f()
        {
            if (this.Size == Vector3i.Zero)
            {
                return null;
            }

            return Area3f.FromMinAndMax(this.Min.ToVector3f(), (this.Max - Vector3i.One).ToVector3f());
        }

        public static bool operator ==(Range3i a, Range3i b) => (a.Min == b.Min) && (a.Max == b.Max);

        public static bool operator !=(Range3i a, Range3i b) => (a.Min != b.Min) || (a.Max != b.Max);

        public bool Contains(Vector3i point) => (point >= this.Min) && (point < this.Max);

        public bool Overlaps(Range3i that)
        {
            if ((this.Size == Vector3i.Zero) || (that.Size == Vector3i.Zero))
            {
                return false;
            }

            return (this.Min < that.Max) && (that.Min < this.Max);
        }

        public Range3i CombineWith(Range3i that)
        {
            if (this.Size == Vector3i.Zero)
            {
                return that;
            }

            if (that.Size == Vector3i.Zero)
            {
                return this;
            }

            var min = Vector3iUtils.Min(this.Min, that.Min);
            var max = Vector3iUtils.Max(this.Max, that.Max);
            return FromMinAndMax(min, max);
        }

        public Range3i CombineWith(Vector3i point)
        {
            if (this.Contains(point))
            {
                return this;
            }

            var min = Vector3iUtils.Min(this.Min, point);
            var max = Vector3iUtils.Max(this.Max, point);

            return FromMinAndMax(min, max);
        }

        public Range3i IntersectWith(Range3i that)
        {
            if ((this.Size == Vector3i.Zero) || (that.Size == Vector3i.Zero))
            {
                return Zero;
            }

            var min = Vector3iUtils.Max(this.Min, that.Min);
            var max = Vector3iUtils.Max(Vector3iUtils.Min(this.Max, that.Max), min);

            return FromMinAndMax(min, max);
        }

        public Range3i UnionWith(Range3i? that)
        {
            if (that == null)
            {
                return this;
            }

            return this.UnionWith(that.Value);
        }

        public Range3i UnionWith(Range3i that)
        {
            if (this.Size == Vector3i.Zero)
            {
                return that;
            }

            if (that.Size == Vector3i.Zero)
            {
                return this;
            }

            var min = Vector3iUtils.Min(this.Min, that.Min);
            var max = Vector3iUtils.Max(this.Max, that.Max);

            return FromMinAndMax(min, max);
        }

        public Area4f ToArea4fFromMinMax(float minZ, float maxZ) => Area4f.FromMinAndMax(this.Min.ToVector4f(minZ), this.Max.ToVector4f(maxZ));

        public bool Equals(Range3i other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Range3i))
            {
                return false;
            }

            return this.Equals((Range3i)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.Min.GetHashCode() + (7 * this.Max.GetHashCode());
            }
        }

        public override string ToString() => $"Range3i: min={this.Min} max={this.Max} size={this.Size}";
    }
}
