using System;
using Newtonsoft.Json;

namespace Votyra.Core.Models
{
    public struct Area4f : IEquatable<Area4f>
    {
        public static readonly Area4f Zero = new Area4f(Vector4f.Zero, Vector4f.Zero);
        public static readonly Area4f All = new Area4f(Vector4fUtils.FromSame(float.MinValue / 2), Vector4fUtils.FromSame(float.MaxValue / 2));

        public readonly Vector4f Max;
        public readonly Vector4f Min;

        public Area1f X => Area1f.FromMinAndMax(this.Min.X, this.Max.X);

        public Area1f Y => Area1f.FromMinAndMax(this.Min.Y, this.Max.Y);

        public Area1f Z => Area1f.FromMinAndMax(this.Min.Z, this.Max.Z);

        public Area1f W => Area1f.FromMinAndMax(this.Min.W, this.Max.W);

        [JsonConstructor]
        private Area4f(Vector4f min, Vector4f max)
        {
            this.Min = min;
            this.Max = max;
        }

        [JsonIgnore]
        public Vector4f Center => (this.Max + this.Min) / 2f;

        [JsonIgnore]
        public Vector4f Size => this.Max - this.Min;

        [JsonIgnore]
        public Vector4f Extents => this.Size / 2;

        public float DiagonalLength => this.Size.Magnitude();

        public static Area4f FromMinAndMax(Vector4f min, Vector4f max) => new Area4f(min, max);

        public static Area4f FromMinAndSize(Vector4f min, Vector4f size) => new Area4f(min, min + size);

        public static Area4f FromCenterAndSize(Vector4f center, Vector4f size) => new Area4f(center - (size / 2), size);

        public static Area4f FromCenterAndExtents(Vector4f center, Vector4f extents)
        {
            if (extents.AnyNegative())
            {
                throw new InvalidOperationException($"When creating {nameof(Area4f)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            }

            return new Area4f(center - extents, center + extents);
        }

        public static bool operator ==(Area4f a, Area4f b) => (a.Center == b.Center) && (a.Size == b.Size);

        public static bool operator !=(Area4f a, Area4f b) => (a.Center != b.Center) || (a.Size != b.Size);

        public static Area4f operator /(Area4f a, float b) => FromMinAndMax(a.Min / b, a.Max / b);

        public static Area4f operator /(Area4f a, Vector4f b) => FromMinAndMax(a.Min / b, a.Max / b);

        public Area4f IntersectWith(Area4f that)
        {
            if ((this.Size == Vector4f.Zero) || (that.Size == Vector4f.Zero))
            {
                return Zero;
            }

            var min = Vector4fUtils.Max(this.Min, that.Min);
            var max = Vector4fUtils.Max(Vector4fUtils.Min(this.Max, that.Max), min);

            return FromMinAndMax(min, max);
        }

        public Area4f Encapsulate(Vector4f point) => FromMinAndMax(Vector4fUtils.Min(this.Min, point), Vector4fUtils.Max(this.Max, point));

        public Area4f Encapsulate(Area4f bounds) => FromMinAndMax(Vector4fUtils.Min(this.Min, bounds.Min), Vector4fUtils.Max(this.Max, bounds.Max));

        public bool Contains(Vector4f point) => (point >= this.Min) && (point <= this.Max);

        public Range4i RoundToInt() => Range4i.FromMinAndMax(this.Min.RoundToVector4i(), this.Max.RoundToVector4i());

        public Range4i RoundToContain() => Range4i.FromMinAndMax(this.Min.FloorToVector4i(), this.Max.CeilToVector4i());

        public Area4f CombineWith(Area4f that)
        {
            if (this.Size == Vector4f.Zero)
            {
                return that;
            }

            if (that.Size == Vector4f.Zero)
            {
                return this;
            }

            var min = Vector4fUtils.Min(this.Min, that.Min);
            var max = Vector4fUtils.Max(this.Max, that.Max);
            return FromMinAndMax(min, max);
        }

        public Area4f UnionWith(Area4f range) => FromMinAndMax(Vector4fUtils.Min(this.Min, range.Min), Vector4fUtils.Min(this.Max, range.Max));

        public Area4f? UnionWith(Area4f? range)
        {
            if (range == null)
            {
                return this;
            }

            return this.UnionWith(range.Value);
        }

        public Area4f UnionWith(Vector4f value)
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

        public bool Equals(Area4f other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Area4f))
            {
                return false;
            }

            return this.Equals((Area4f)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.Min.GetHashCode() + (7 * this.Max.GetHashCode());
            }
        }

        public override string ToString() => $"Area4f: min={this.Min} max={this.Max} size={this.Size}";
    }
}
