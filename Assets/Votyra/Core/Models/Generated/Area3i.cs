using System;

namespace Votyra.Core.Models
{
    /// <summary>
    ///     Max is inclusive
    /// </summary>
    public struct Area3i : IEquatable<Area3i>
    {
        public static readonly Area3i All = new Area3i(Vector3iUtils.FromSame(int.MinValue / 2), Vector3iUtils.FromSame((int.MaxValue / 2) - 1));

        public static readonly Area3i Zero = new Area3i();

        public readonly Vector3i Min;

        public readonly Vector3i Max;

        private Area3i(Vector3i min, Vector3i max)
        {
            this.Min = min;
            this.Max = max;
            if (this.Size.AnyNegative())
            {
                throw new InvalidOperationException($"{nameof(Area3i)} '{this}' cannot have a size be zero or negative!");
            }

            if (this.Size.AnyZero())
            {
                this.Max = this.Min;
            }
        }

        public Vector3i Size => (this.Max - this.Min) + Vector3i.One;

        public static Area3i FromCenterAndExtents(Vector3i center, Vector3i extents)
        {
            if (extents.AnyNegative())
            {
                throw new InvalidOperationException($"When creating {nameof(Area3i)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            }

            return new Area3i(center - extents, center + extents);
        }

        public static Area3i FromMinAndSize(Vector3i min, Vector3i size)
        {
            if (size.AnyNegative())
            {
                throw new InvalidOperationException($"When creating {nameof(Area3i)} using min '{min}' and size '{size}', size cannot have a negative coordinate!");
            }

            return new Area3i(min, min + size);
        }

        public static Area3i FromMinAndMax(Vector3i min, Vector3i max) => new Area3i(min, max);

        public static bool operator ==(Area3i a, Area3i b) => (a.Min == b.Min) && (a.Max == b.Max);

        public static bool operator !=(Area3i a, Area3i b) => (a.Min != b.Min) || (a.Max != b.Max);

        public Area3i ExtendBothDirections(int distance) => FromMinAndMax(this.Min - distance, this.Max + distance);

        public Range3i ToRange3i() => Range3i.FromMinAndMax(this.Min, this.Max + Vector3i.One);

        public Area3f ToArea3f() => Area3f.FromMinAndMax(this.Min.ToVector3f(), this.Max.ToVector3f());

        public bool Contains(Vector3i point) => (point >= this.Min) && (point <= this.Max);

        public bool Overlaps(Area3i that)
        {
            if ((this.Size == Vector3i.Zero) || (that.Size == Vector3i.Zero))
            {
                return false;
            }

            return (this.Min <= that.Max) && (that.Min <= this.Max);
        }

        public Area3i CombineWith(Area3i that)
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

        public Area3i CombineWith(Vector3i point)
        {
            if (this.Contains(point))
            {
                return this;
            }

            var min = Vector3iUtils.Min(this.Min, point);
            var max = Vector3iUtils.Max(this.Max, point);

            return FromMinAndMax(min, max);
        }

        public Area3i IntersectWith(Area3i that)
        {
            if ((this.Size == Vector3i.Zero) || (that.Size == Vector3i.Zero))
            {
                return Zero;
            }

            var min = Vector3iUtils.Max(this.Min, that.Min);
            var max = Vector3iUtils.Max(Vector3iUtils.Min(this.Max, that.Max), min);

            return FromMinAndMax(min, max);
        }

        public Area3i UnionWith(Area3i? that)
        {
            if (that == null)
            {
                return this;
            }

            return this.UnionWith(that.Value);
        }

        public Area3i UnionWith(Area3i that)
        {
            if ((this.Size == Vector3i.Zero) || (that.Size == Vector3i.Zero))
            {
                return Zero;
            }

            var min = Vector3iUtils.Min(this.Min, that.Min);
            var max = Vector3iUtils.Max(this.Max, that.Max);

            return FromMinAndMax(min, max);
        }

        public bool Equals(Area3i other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Area3i))
            {
                return false;
            }

            return this.Equals((Area3i)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.Min.GetHashCode() + (7 * this.Max.GetHashCode());
            }
        }

        public override string ToString() => $"Area3i: min={this.Min} max={this.Max} size={this.Size}";
    }
}
