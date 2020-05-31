using System;

namespace Votyra.Core.Models
{
    /// <summary>
    ///     Max is inclusive
    /// </summary>
    public struct Area1i : IEquatable<Area1i>
    {
        public static readonly Area1i All = new Area1i(Vector1iUtils.FromSame(int.MinValue / 2), Vector1iUtils.FromSame((int.MaxValue / 2) - 1));

        public static readonly Area1i Zero = new Area1i(Vector1i.Zero, Vector1i.Zero);

        public readonly int Min;

        public readonly int Max;

        private Area1i(int min, int max)
        {
            this.Min = min;
            this.Max = max;
            if (this.Size.AnyNegative())
            {
                throw new InvalidOperationException($"{nameof(Area1i)} '{this}' cannot have a size be zero or negative!");
            }

            if (this.Size.AnyZero())
            {
                this.Max = this.Min;
            }
        }

        public int Size => (this.Max - this.Min) + Vector1i.One;

        public static Area1i FromCenterAndExtents(int center, int extents)
        {
            if (extents.AnyNegative())
            {
                throw new InvalidOperationException($"When creating {nameof(Area1i)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            }

            return new Area1i(center - extents, center + extents);
        }

        public static Area1i FromMinAndSize(int min, int size)
        {
            if (size.AnyNegative())
            {
                throw new InvalidOperationException($"When creating {nameof(Area1i)} using min '{min}' and size '{size}', size cannot have a negative coordinate!");
            }

            return new Area1i(min, min + size);
        }

        public static Area1i FromMinAndMax(int min, int max) => new Area1i(min, max);

        public static bool operator ==(Area1i a, Area1i b) => (a.Min == b.Min) && (a.Max == b.Max);

        public static bool operator !=(Area1i a, Area1i b) => (a.Min != b.Min) || (a.Max != b.Max);

        public Area1i ExtendBothDirections(int distance) => FromMinAndMax(this.Min - distance, this.Max + distance);

        public Range1i ToRange1i() => Range1i.FromMinAndMax(this.Min, this.Max + Vector1i.One);

        public Area1f ToArea1f() => Area1f.FromMinAndMax(this.Min.ToVector1f(), this.Max.ToVector1f());

        public bool Contains(int point) => (point >= this.Min) && (point <= this.Max);

        public bool Overlaps(Area1i that)
        {
            if ((this.Size == Vector1i.Zero) || (that.Size == Vector1i.Zero))
            {
                return false;
            }

            return (this.Min <= that.Max) && (that.Min <= this.Max);
        }

        public Area1i CombineWith(Area1i that)
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

        public Area1i CombineWith(int point)
        {
            if (this.Contains(point))
            {
                return this;
            }

            var min = Vector1iUtils.Min(this.Min, point);
            var max = Vector1iUtils.Max(this.Max, point);

            return FromMinAndMax(min, max);
        }

        public Area1i IntersectWith(Area1i that)
        {
            if ((this.Size == Vector1i.Zero) || (that.Size == Vector1i.Zero))
            {
                return Zero;
            }

            var min = Vector1iUtils.Max(this.Min, that.Min);
            var max = Vector1iUtils.Max(Vector1iUtils.Min(this.Max, that.Max), min);

            return FromMinAndMax(min, max);
        }

        public Area1i UnionWith(Area1i? that)
        {
            if (that == null)
            {
                return this;
            }

            return this.UnionWith(that.Value);
        }

        public Area1i UnionWith(Area1i that)
        {
            if ((this.Size == Vector1i.Zero) || (that.Size == Vector1i.Zero))
            {
                return Zero;
            }

            var min = Vector1iUtils.Min(this.Min, that.Min);
            var max = Vector1iUtils.Max(this.Max, that.Max);

            return FromMinAndMax(min, max);
        }

        public bool Equals(Area1i other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Area1i))
            {
                return false;
            }

            return this.Equals((Area1i)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.Min.GetHashCode() + (7 * this.Max.GetHashCode());
            }
        }

        public override string ToString() => $"Area1i: min={this.Min} max={this.Max} size={this.Size}";
    }
}
