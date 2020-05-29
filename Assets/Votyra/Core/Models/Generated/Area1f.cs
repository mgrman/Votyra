using System;
using Newtonsoft.Json;

namespace Votyra.Core.Models
{
    public struct Area1f : IEquatable<Area1f>
    {
        public static readonly Area1f Zero = new Area1f();
        public static readonly Area1f All = new Area1f(Vector1fUtils.FromSame(float.MinValue / 2), Vector1fUtils.FromSame(float.MaxValue / 2));

        public readonly float Max;
        public readonly float Min;

        [JsonConstructor]
        private Area1f(float min, float max)
        {
            this.Min = min;
            this.Max = max;
        }

        [JsonIgnore]
        public float Center => (this.Max + this.Min) / 2f;

        [JsonIgnore]
        public float Size => this.Max - this.Min;

        [JsonIgnore]
        public float Extents => this.Size / 2;

        public float DiagonalLength => this.Size.Magnitude();

        public static Area1f FromMinAndMax(float min, float max) => new Area1f(min, max);

        public static Area1f FromMinAndSize(float min, float size) => new Area1f(min, min + size);

        public static Area1f FromCenterAndSize(float center, float size) => new Area1f(center - (size / 2), size);

        public static Area1f FromCenterAndExtents(float center, float extents)
        {
            if (extents.AnyNegative())
            {
                throw new InvalidOperationException($"When creating {nameof(Area1f)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            }

            return new Area1f(center - extents, center + extents);
        }

        public static bool operator ==(Area1f a, Area1f b) => (a.Center == b.Center) && (a.Size == b.Size);

        public static bool operator !=(Area1f a, Area1f b) => (a.Center != b.Center) || (a.Size != b.Size);

        public static Area1f operator /(Area1f a, float b) => FromMinAndMax(a.Min / b, a.Max / b);

        public Area1f IntersectWith(Area1f that)
        {
            if ((this.Size == Vector1f.Zero) || (that.Size == Vector1f.Zero))
            {
                return Zero;
            }

            var min = Vector1fUtils.Max(this.Min, that.Min);
            var max = Vector1fUtils.Max(Vector1fUtils.Min(this.Max, that.Max), min);

            return FromMinAndMax(min, max);
        }

        public Area1f Encapsulate(float point) => FromMinAndMax(Vector1fUtils.Min(this.Min, point), Vector1fUtils.Max(this.Max, point));

        public Area1f Encapsulate(Area1f bounds) => FromMinAndMax(Vector1fUtils.Min(this.Min, bounds.Min), Vector1fUtils.Max(this.Max, bounds.Max));

        public bool Contains(float point) => (point >= this.Min) && (point <= this.Max);

        public Range1i RoundToInt() => Range1i.FromMinAndMax(this.Min.RoundToVector1i(), this.Max.RoundToVector1i());

        public Range1i RoundToContain() => Range1i.FromMinAndMax(this.Min.FloorToVector1i(), this.Max.CeilToVector1i());

        public Area1f CombineWith(Area1f that)
        {
            if (this.Size == Vector1f.Zero)
            {
                return that;
            }

            if (that.Size == Vector1f.Zero)
            {
                return this;
            }

            var min = Vector1fUtils.Min(this.Min, that.Min);
            var max = Vector1fUtils.Max(this.Max, that.Max);
            return FromMinAndMax(min, max);
        }

        public Area1f UnionWith(Area1f range) => FromMinAndMax(Vector1fUtils.Min(this.Min, range.Min), Vector1fUtils.Min(this.Max, range.Max));

        public Area1f? UnionWith(Area1f? range)
        {
            if (range == null)
            {
                return this;
            }

            return this.UnionWith(range.Value);
        }

        public Area1f UnionWith(float value)
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

        public bool Equals(Area1f other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Area1f))
            {
                return false;
            }

            return this.Equals((Area1f)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.Min.GetHashCode() + (7 * this.Max.GetHashCode());
            }
        }

        public override string ToString() => $"Area1f: min={this.Min} max={this.Max} size={this.Size}";
    }
}
