using System;
using Newtonsoft.Json;

namespace Votyra.Core.Models
{
    public struct Area6f : IEquatable<Area6f>
    {
        public static readonly Area6f Zero = new Area6f(Vector6f.Zero, Vector6f.Zero);
        public static readonly Area6f All = new Area6f(Vector6fUtils.FromSame(float.MinValue / 2), Vector6fUtils.FromSame(float.MaxValue / 2));

        public readonly Vector6f Max;
        public readonly Vector6f Min;

        public Area1f X0 => Area1f.FromMinAndMax(this.Min.X0, this.Max.X0);

        public Area1f X1 => Area1f.FromMinAndMax(this.Min.X1, this.Max.X1);

        public Area1f X2 => Area1f.FromMinAndMax(this.Min.X2, this.Max.X2);

        public Area1f X3 => Area1f.FromMinAndMax(this.Min.X3, this.Max.X3);

        public Area1f X4 => Area1f.FromMinAndMax(this.Min.X4, this.Max.X4);

        public Area1f X5 => Area1f.FromMinAndMax(this.Min.X5, this.Max.X5);

        [JsonConstructor]
        private Area6f(Vector6f min, Vector6f max)
        {
            this.Min = min;
            this.Max = max;
        }

        [JsonIgnore]
        public Vector6f Center => (this.Max + this.Min) / 2f;

        [JsonIgnore]
        public Vector6f Size => this.Max - this.Min;

        [JsonIgnore]
        public Vector6f Extents => this.Size / 2;

        public float DiagonalLength => this.Size.Magnitude();

        public bool AnyNan => this.Max.AnyNan() || this.Min.AnyNan();

        public static Area6f FromMinAndMax(Vector6f min, Vector6f max) => new Area6f(min, max);

        public static Area6f FromMinAndSize(Vector6f min, Vector6f size) => new Area6f(min, min + size);

        public static Area6f FromCenterAndSize(Vector6f center, Vector6f size) => new Area6f(center - (size / 2), size);

        public static Area6f FromCenterAndExtents(Vector6f center, Vector6f extents)
        {
            if (extents.AnyNegative())
            {
                throw new InvalidOperationException($"When creating {nameof(Area6f)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            }

            return new Area6f(center - extents, center + extents);
        }

        public static bool operator ==(Area6f a, Area6f b) => (a.Center == b.Center) && (a.Size == b.Size);

        public static bool operator !=(Area6f a, Area6f b) => (a.Center != b.Center) || (a.Size != b.Size);

        public static Area6f operator /(Area6f a, float b) => FromMinAndMax(a.Min / b, a.Max / b);

        public static Area6f operator /(Area6f a, Vector6f b) => FromMinAndMax(a.Min / b, a.Max / b);

        public Area6f IntersectWith(Area6f that)
        {
            if ((this.Size == Vector6f.Zero) || (that.Size == Vector6f.Zero))
            {
                return Zero;
            }

            var min = Vector6fUtils.Max(this.Min, that.Min);
            var max = Vector6fUtils.Max(Vector6fUtils.Min(this.Max, that.Max), min);

            return FromMinAndMax(min, max);
        }

        public Area6f Encapsulate(Vector6f point) => FromMinAndMax(Vector6fUtils.Min(this.Min, point), Vector6fUtils.Max(this.Max, point));

        public Area6f Encapsulate(Area6f bounds) => FromMinAndMax(Vector6fUtils.Min(this.Min, bounds.Min), Vector6fUtils.Max(this.Max, bounds.Max));

        public bool Contains(Vector6f point) => (point >= this.Min) && (point <= this.Max);

        public Range6i RoundToInt() => Range6i.FromMinAndMax(this.Min.RoundToVector6i(), this.Max.RoundToVector6i());

        public Range6i RoundToContain() => Range6i.FromMinAndMax(this.Min.FloorToVector6i(), this.Max.CeilToVector6i());

        public Area6f CombineWith(Area6f that)
        {
            if (this.Size == Vector6f.Zero)
            {
                return that;
            }

            if (that.Size == Vector6f.Zero)
            {
                return this;
            }

            var min = Vector6fUtils.Min(this.Min, that.Min);
            var max = Vector6fUtils.Max(this.Max, that.Max);
            return FromMinAndMax(min, max);
        }

        public Area6f UnionWith(Area6f range) => FromMinAndMax(Vector6fUtils.Min(this.Min, range.Min), Vector6fUtils.Min(this.Max, range.Max));

        public Area6f? UnionWith(Area6f? range)
        {
            if (range == null)
            {
                return this;
            }

            return this.UnionWith(range.Value);
        }

        public Area6f UnionWith(Vector6f value)
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

        public bool Equals(Area6f other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Area6f))
            {
                return false;
            }

            return this.Equals((Area6f)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.Min.GetHashCode() + (7 * this.Max.GetHashCode());
            }
        }

        public override string ToString() => $"Area6f: min={this.Min} max={this.Max} size={this.Size}";
    }
}
