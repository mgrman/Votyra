using System;

namespace Votyra.Core.Models
{
    /// <summary>
    ///     Max is inclusive
    /// </summary>
    public struct Area5i : IEquatable<Area5i>
    {
        public static readonly Area5i All = new Area5i(Vector5iUtils.FromSame(int.MinValue / 2), Vector5iUtils.FromSame((int.MaxValue / 2) - 1));

        public static readonly Area5i Zero = new Area5i();

        public readonly Vector5i Min;

        public readonly Vector5i Max;

        private Area5i(Vector5i min, Vector5i max)
        {
            this.Min = min;
            this.Max = max;
            if (this.Size.AnyNegative())
            {
                throw new InvalidOperationException($"{nameof(Area5i)} '{this}' cannot have a size be zero or negative!");
            }

            if (this.Size.AnyZero())
            {
                this.Max = this.Min;
            }
        }

        public Vector5i Size => (this.Max - this.Min) + Vector5i.One;

        public static Area5i FromCenterAndExtents(Vector5i center, Vector5i extents)
        {
            if (extents.AnyNegative())
            {
                throw new InvalidOperationException($"When creating {nameof(Area5i)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            }

            return new Area5i(center - extents, center + extents);
        }

        public static Area5i FromMinAndSize(Vector5i min, Vector5i size)
        {
            if (size.AnyNegative())
            {
                throw new InvalidOperationException($"When creating {nameof(Area5i)} using min '{min}' and size '{size}', size cannot have a negative coordinate!");
            }

            return new Area5i(min, min + size);
        }

        public static Area5i FromMinAndMax(Vector5i min, Vector5i max) => new Area5i(min, max);

        public static bool operator ==(Area5i a, Area5i b) => (a.Min == b.Min) && (a.Max == b.Max);

        public static bool operator !=(Area5i a, Area5i b) => (a.Min != b.Min) || (a.Max != b.Max);

        public Area5i ExtendBothDirections(int distance) => FromMinAndMax(this.Min - distance, this.Max + distance);

        public Range5i ToRange5i() => Range5i.FromMinAndMax(this.Min, this.Max + Vector5i.One);

        public Area5f ToArea5f() => Area5f.FromMinAndMax(this.Min.ToVector5f(), this.Max.ToVector5f());

        public bool Contains(Vector5i point) => (point >= this.Min) && (point <= this.Max);

        public bool Overlaps(Area5i that)
        {
            if ((this.Size == Vector5i.Zero) || (that.Size == Vector5i.Zero))
            {
                return false;
            }

            return (this.Min <= that.Max) && (that.Min <= this.Max);
        }

        public Area5i CombineWith(Area5i that)
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

        public Area5i CombineWith(Vector5i point)
        {
            if (this.Contains(point))
            {
                return this;
            }

            var min = Vector5iUtils.Min(this.Min, point);
            var max = Vector5iUtils.Max(this.Max, point);

            return FromMinAndMax(min, max);
        }

        public Area5i IntersectWith(Area5i that)
        {
            if ((this.Size == Vector5i.Zero) || (that.Size == Vector5i.Zero))
            {
                return Zero;
            }

            var min = Vector5iUtils.Max(this.Min, that.Min);
            var max = Vector5iUtils.Max(Vector5iUtils.Min(this.Max, that.Max), min);

            return FromMinAndMax(min, max);
        }

        public Area5i UnionWith(Area5i? that)
        {
            if (that == null)
            {
                return this;
            }

            return this.UnionWith(that.Value);
        }

        public Area5i UnionWith(Area5i that)
        {
            if ((this.Size == Vector5i.Zero) || (that.Size == Vector5i.Zero))
            {
                return Zero;
            }

            var min = Vector5iUtils.Min(this.Min, that.Min);
            var max = Vector5iUtils.Max(this.Max, that.Max);

            return FromMinAndMax(min, max);
        }

        public bool Equals(Area5i other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Area5i))
            {
                return false;
            }

            return this.Equals((Area5i)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.Min.GetHashCode() + (7 * this.Max.GetHashCode());
            }
        }

        public override string ToString() => $"Area5i: min={this.Min} max={this.Max} size={this.Size}";
    }
}
