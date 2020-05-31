using System;
using Newtonsoft.Json;

namespace Votyra.Core.Models
{
    public struct Area5f : IEquatable<Area5f>
    {
        public static readonly Area5f Zero = new Area5f(Vector5f.Zero, Vector5f.Zero);
        public static readonly Area5f All = new Area5f(Vector5fUtils.FromSame(float.MinValue / 2), Vector5fUtils.FromSame(float.MaxValue / 2));

        public readonly Vector5f Max;
        public readonly Vector5f Min;

        public Area1f X0 => Area1f.FromMinAndMax(this.Min.X0, this.Max.X0);

        public Area1f X1 => Area1f.FromMinAndMax(this.Min.X1, this.Max.X1);

        public Area1f X2 => Area1f.FromMinAndMax(this.Min.X2, this.Max.X2);

        public Area1f X3 => Area1f.FromMinAndMax(this.Min.X3, this.Max.X3);

        public Area1f X4 => Area1f.FromMinAndMax(this.Min.X4, this.Max.X4);

        [JsonConstructor]
        private Area5f(Vector5f min, Vector5f max)
        {
            this.Min = min;
            this.Max = max;
        }

        [JsonIgnore]
        public Vector5f Center => (this.Max + this.Min) / 2f;

        [JsonIgnore]
        public Vector5f Size => this.Max - this.Min;

        [JsonIgnore]
        public Vector5f Extents => this.Size / 2;

        public float DiagonalLength => this.Size.Magnitude();

        public static Area5f FromMinAndMax(Vector5f min, Vector5f max) => new Area5f(min, max);

        public static Area5f FromMinAndSize(Vector5f min, Vector5f size) => new Area5f(min, min + size);

        public static Area5f FromCenterAndSize(Vector5f center, Vector5f size) => new Area5f(center - (size / 2), size);

        public static Area5f FromCenterAndExtents(Vector5f center, Vector5f extents)
        {
            if (extents.AnyNegative())
            {
                throw new InvalidOperationException($"When creating {nameof(Area5f)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            }

            return new Area5f(center - extents, center + extents);
        }

        public static bool operator ==(Area5f a, Area5f b) => (a.Center == b.Center) && (a.Size == b.Size);

        public static bool operator !=(Area5f a, Area5f b) => (a.Center != b.Center) || (a.Size != b.Size);

        public static Area5f operator /(Area5f a, float b) => FromMinAndMax(a.Min / b, a.Max / b);

        public static Area5f operator /(Area5f a, Vector5f b) => FromMinAndMax(a.Min / b, a.Max / b);

        public Area5f IntersectWith(Area5f that)
        {
            if ((this.Size == Vector5f.Zero) || (that.Size == Vector5f.Zero))
            {
                return Zero;
            }

            var min = Vector5fUtils.Max(this.Min, that.Min);
            var max = Vector5fUtils.Max(Vector5fUtils.Min(this.Max, that.Max), min);

            return FromMinAndMax(min, max);
        }

        public Area5f Encapsulate(Vector5f point) => FromMinAndMax(Vector5fUtils.Min(this.Min, point), Vector5fUtils.Max(this.Max, point));

        public Area5f Encapsulate(Area5f bounds) => FromMinAndMax(Vector5fUtils.Min(this.Min, bounds.Min), Vector5fUtils.Max(this.Max, bounds.Max));

        public bool Contains(Vector5f point) => (point >= this.Min) && (point <= this.Max);

        public Range5i RoundToInt() => Range5i.FromMinAndMax(this.Min.RoundToVector5i(), this.Max.RoundToVector5i());

        public Range5i RoundToContain() => Range5i.FromMinAndMax(this.Min.FloorToVector5i(), this.Max.CeilToVector5i());

        public Area5f CombineWith(Area5f that)
        {
            if (this.Size == Vector5f.Zero)
            {
                return that;
            }

            if (that.Size == Vector5f.Zero)
            {
                return this;
            }

            var min = Vector5fUtils.Min(this.Min, that.Min);
            var max = Vector5fUtils.Max(this.Max, that.Max);
            return FromMinAndMax(min, max);
        }

        public Area5f UnionWith(Area5f range) => FromMinAndMax(Vector5fUtils.Min(this.Min, range.Min), Vector5fUtils.Min(this.Max, range.Max));

        public Area5f? UnionWith(Area5f? range)
        {
            if (range == null)
            {
                return this;
            }

            return this.UnionWith(range.Value);
        }

        public Area5f UnionWith(Vector5f value)
        {
            if (value < this.Min)
            {
                return FromMinAndMax(value, this.Max);
            }

            if (value > this.Max)
            {
                return FromMinAndMax(this.Min, value);
            }

            return this;
        }

        public bool Equals(Area5f other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Area5f))
            {
                return false;
            }

            return this.Equals((Area5f)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.Min.GetHashCode() + (7 * this.Max.GetHashCode());
            }
        }

        public override string ToString() => $"Area5f: min={this.Min} max={this.Max} size={this.Size}";
    }
}
