using System;

namespace Votyra.Core.Models
{
    /// <summary>
    ///     Max is inclusive
    /// </summary>
    public struct Area4i : IEquatable<Area4i>
    {
        public static readonly Area4i All = new Area4i(Vector4iUtils.FromSame(int.MinValue / 2), Vector4iUtils.FromSame((int.MaxValue / 2) - 1));

        public static readonly Area4i Zero = new Area4i();

        public readonly Vector4i Min;

        public readonly Vector4i Max;

        private Area4i(Vector4i min, Vector4i max)
        {
            this.Min = min;
            this.Max = max;
            if (this.Size.AnyNegative())
            {
                throw new InvalidOperationException($"{nameof(Area4i)} '{this}' cannot have a size be zero or negative!");
            }

            if (this.Size.AnyZero())
            {
                this.Max = this.Min;
            }
        }

        public Vector4i Size => (this.Max - this.Min) + Vector4i.One;

        public static Area4i FromCenterAndExtents(Vector4i center, Vector4i extents)
        {
            if (extents.AnyNegative())
            {
                throw new InvalidOperationException($"When creating {nameof(Area4i)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            }

            return new Area4i(center - extents, center + extents);
        }

        public static Area4i FromMinAndSize(Vector4i min, Vector4i size)
        {
            if (size.AnyNegative())
            {
                throw new InvalidOperationException($"When creating {nameof(Area4i)} using min '{min}' and size '{size}', size cannot have a negative coordinate!");
            }

            return new Area4i(min, min + size);
        }

        public static Area4i FromMinAndMax(Vector4i min, Vector4i max) => new Area4i(min, max);

        public static bool operator ==(Area4i a, Area4i b) => (a.Min == b.Min) && (a.Max == b.Max);

        public static bool operator !=(Area4i a, Area4i b) => (a.Min != b.Min) || (a.Max != b.Max);

        public Area4i ExtendBothDirections(int distance) => FromMinAndMax(this.Min - distance, this.Max + distance);

        public Range4i ToRange4i() => Range4i.FromMinAndMax(this.Min, this.Max + Vector4i.One);

        public Area4f ToArea4f() => Area4f.FromMinAndMax(this.Min.ToVector4f(), this.Max.ToVector4f());

        public bool Contains(Vector4i point) => (point >= this.Min) && (point <= this.Max);

        public bool Overlaps(Area4i that)
        {
            if ((this.Size == Vector4i.Zero) || (that.Size == Vector4i.Zero))
            {
                return false;
            }

            return (this.Min <= that.Max) && (that.Min <= this.Max);
        }

        public Area4i CombineWith(Area4i that)
        {
            if (this.Size == Vector4i.Zero)
            {
                return that;
            }

            if (that.Size == Vector4i.Zero)
            {
                return this;
            }

            var min = Vector4iUtils.Min(this.Min, that.Min);
            var max = Vector4iUtils.Max(this.Max, that.Max);
            return FromMinAndMax(min, max);
        }

        public Area4i CombineWith(Vector4i point)
        {
            if (this.Contains(point))
            {
                return this;
            }

            var min = Vector4iUtils.Min(this.Min, point);
            var max = Vector4iUtils.Max(this.Max, point);

            return FromMinAndMax(min, max);
        }

        public Area4i IntersectWith(Area4i that)
        {
            if ((this.Size == Vector4i.Zero) || (that.Size == Vector4i.Zero))
            {
                return Zero;
            }

            var min = Vector4iUtils.Max(this.Min, that.Min);
            var max = Vector4iUtils.Max(Vector4iUtils.Min(this.Max, that.Max), min);

            return FromMinAndMax(min, max);
        }

        public Area4i UnionWith(Area4i? that)
        {
            if (that == null)
            {
                return this;
            }

            return this.UnionWith(that.Value);
        }

        public Area4i UnionWith(Area4i that)
        {
            if ((this.Size == Vector4i.Zero) || (that.Size == Vector4i.Zero))
            {
                return Zero;
            }

            var min = Vector4iUtils.Min(this.Min, that.Min);
            var max = Vector4iUtils.Max(this.Max, that.Max);

            return FromMinAndMax(min, max);
        }

        public bool Equals(Area4i other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Area4i))
            {
                return false;
            }

            return this.Equals((Area4i)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.Min.GetHashCode() + (7 * this.Max.GetHashCode());
            }
        }

        public override string ToString() => $"Area4i: min={this.Min} max={this.Max} size={this.Size}";
    }
}
