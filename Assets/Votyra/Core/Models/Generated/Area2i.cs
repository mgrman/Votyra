using System;

namespace Votyra.Core.Models
{
    /// <summary>
    ///     Max is inclusive
    /// </summary>
    public struct Area2i : IEquatable<Area2i>
    {
        public static readonly Area2i All = new Area2i(Vector2iUtils.FromSame(int.MinValue / 2), Vector2iUtils.FromSame((int.MaxValue / 2) - 1));

        public static readonly Area2i Zero = new Area2i();

        public readonly Vector2i Min;

        public readonly Vector2i Max;

        private Area2i(Vector2i min, Vector2i max)
        {
            this.Min = min;
            this.Max = max;
            if (this.Size.AnyNegative())
            {
                throw new InvalidOperationException($"{nameof(Area2i)} '{this}' cannot have a size be zero or negative!");
            }

            if (this.Size.AnyZero())
            {
                this.Max = this.Min;
            }
        }

        public Vector2i Size => (this.Max - this.Min) + Vector2i.One;

        public static Area2i FromCenterAndExtents(Vector2i center, Vector2i extents)
        {
            if (extents.AnyNegative())
            {
                throw new InvalidOperationException($"When creating {nameof(Area2i)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            }

            return new Area2i(center - extents, center + extents);
        }

        public static Area2i FromMinAndSize(Vector2i min, Vector2i size)
        {
            if (size.AnyNegative())
            {
                throw new InvalidOperationException($"When creating {nameof(Area2i)} using min '{min}' and size '{size}', size cannot have a negative coordinate!");
            }

            return new Area2i(min, min + size);
        }

        public static Area2i FromMinAndMax(Vector2i min, Vector2i max) => new Area2i(min, max);

        public static bool operator ==(Area2i a, Area2i b) => (a.Min == b.Min) && (a.Max == b.Max);

        public static bool operator !=(Area2i a, Area2i b) => (a.Min != b.Min) || (a.Max != b.Max);

        public Area2i ExtendBothDirections(int distance) => FromMinAndMax(this.Min - distance, this.Max + distance);

        public Range2i ToRange2i() => Range2i.FromMinAndMax(this.Min, this.Max + Vector2i.One);

        public Area2f ToArea2f() => Area2f.FromMinAndMax(this.Min.ToVector2f(), this.Max.ToVector2f());

        public bool Contains(Vector2i point) => (point >= this.Min) && (point <= this.Max);

        public bool Overlaps(Area2i that)
        {
            if ((this.Size == Vector2i.Zero) || (that.Size == Vector2i.Zero))
            {
                return false;
            }

            return (this.Min <= that.Max) && (that.Min <= this.Max);
        }

        public Area2i CombineWith(Area2i that)
        {
            if (this.Size == Vector2i.Zero)
            {
                return that;
            }

            if (that.Size == Vector2i.Zero)
            {
                return this;
            }

            var min = Vector2iUtils.Min(this.Min, that.Min);
            var max = Vector2iUtils.Max(this.Max, that.Max);
            return FromMinAndMax(min, max);
        }

        public Area2i CombineWith(Vector2i point)
        {
            if (this.Contains(point))
            {
                return this;
            }

            var min = Vector2iUtils.Min(this.Min, point);
            var max = Vector2iUtils.Max(this.Max, point);

            return FromMinAndMax(min, max);
        }

        public Area2i IntersectWith(Area2i that)
        {
            if ((this.Size == Vector2i.Zero) || (that.Size == Vector2i.Zero))
            {
                return Zero;
            }

            var min = Vector2iUtils.Max(this.Min, that.Min);
            var max = Vector2iUtils.Max(Vector2iUtils.Min(this.Max, that.Max), min);

            return FromMinAndMax(min, max);
        }

        public Area2i UnionWith(Area2i? that)
        {
            if (that == null)
            {
                return this;
            }

            return this.UnionWith(that.Value);
        }

        public Area2i UnionWith(Area2i that)
        {
            if ((this.Size == Vector2i.Zero) || (that.Size == Vector2i.Zero))
            {
                return Zero;
            }

            var min = Vector2iUtils.Min(this.Min, that.Min);
            var max = Vector2iUtils.Max(this.Max, that.Max);

            return FromMinAndMax(min, max);
        }

        public bool Equals(Area2i other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Area2i))
            {
                return false;
            }

            return this.Equals((Area2i)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.Min.GetHashCode() + (7 * this.Max.GetHashCode());
            }
        }

        public override string ToString() => $"Area2i: min={this.Min} max={this.Max} size={this.Size}";
    }
}
