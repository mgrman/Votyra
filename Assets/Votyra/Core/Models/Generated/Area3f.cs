using System;
using Newtonsoft.Json;

namespace Votyra.Core.Models
{
    public struct Area3f : IEquatable<Area3f>
    {
        public static readonly Area3f Zero = new Area3f();
        public static readonly Area3f All = new Area3f(Vector3fUtils.FromSame(float.MinValue / 2), Vector3fUtils.FromSame(float.MaxValue / 2));

        public readonly Vector3f Max;
        public readonly Vector3f Min;

        public Area1f X => Area1f.FromMinAndMax(this.Min.X, this.Max.X);

        public Area1f Y => Area1f.FromMinAndMax(this.Min.Y, this.Max.Y);

        public Area1f Z => Area1f.FromMinAndMax(this.Min.Z, this.Max.Z);

        [JsonConstructor]
        private Area3f(Vector3f min, Vector3f max)
        {
            this.Min = min;
            this.Max = max;
        }

        [JsonIgnore]
        public Vector3f Center => (this.Max + this.Min) / 2f;

        [JsonIgnore]
        public Vector3f Size => this.Max - this.Min;

        [JsonIgnore]
        public Vector3f Extents => this.Size / 2;

        public float DiagonalLength => this.Size.Magnitude();

        public static Area3f FromMinAndMax(Vector3f min, Vector3f max) => new Area3f(min, max);

        public static Area3f FromMinAndSize(Vector3f min, Vector3f size) => new Area3f(min, min + size);

        public static Area3f FromCenterAndSize(Vector3f center, Vector3f size) => new Area3f(center - (size / 2), size);

        public static Area3f FromCenterAndExtents(Vector3f center, Vector3f extents)
        {
            if (extents.AnyNegative())
            {
                throw new InvalidOperationException($"When creating {nameof(Area3f)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            }

            return new Area3f(center - extents, center + extents);
        }

        public static bool operator ==(Area3f a, Area3f b) => (a.Center == b.Center) && (a.Size == b.Size);

        public static bool operator !=(Area3f a, Area3f b) => (a.Center != b.Center) || (a.Size != b.Size);

        public static Area3f operator /(Area3f a, float b) => FromMinAndMax(a.Min / b, a.Max / b);

        public static Area3f operator /(Area3f a, Vector3f b) => FromMinAndMax(a.Min / b, a.Max / b);

        public Area3f IntersectWith(Area3f that)
        {
            if ((this.Size == Vector3f.Zero) || (that.Size == Vector3f.Zero))
            {
                return Zero;
            }

            var min = Vector3fUtils.Max(this.Min, that.Min);
            var max = Vector3fUtils.Max(Vector3fUtils.Min(this.Max, that.Max), min);

            return FromMinAndMax(min, max);
        }

        public Area3f Encapsulate(Vector3f point) => FromMinAndMax(Vector3fUtils.Min(this.Min, point), Vector3fUtils.Max(this.Max, point));

        public Area3f Encapsulate(Area3f bounds) => FromMinAndMax(Vector3fUtils.Min(this.Min, bounds.Min), Vector3fUtils.Max(this.Max, bounds.Max));

        public bool Contains(Vector3f point) => (point >= this.Min) && (point <= this.Max);

        public Range3i RoundToInt() => Range3i.FromMinAndMax(this.Min.RoundToVector3i(), this.Max.RoundToVector3i());

        public Range3i RoundToContain() => Range3i.FromMinAndMax(this.Min.FloorToVector3i(), this.Max.CeilToVector3i());

        public Area3f CombineWith(Area3f that)
        {
            if (this.Size == Vector3f.Zero)
            {
                return that;
            }

            if (that.Size == Vector3f.Zero)
            {
                return this;
            }

            var min = Vector3fUtils.Min(this.Min, that.Min);
            var max = Vector3fUtils.Max(this.Max, that.Max);
            return FromMinAndMax(min, max);
        }

        public Area3f UnionWith(Area3f range) => FromMinAndMax(Vector3fUtils.Min(this.Min, range.Min), Vector3fUtils.Min(this.Max, range.Max));

        public Area3f? UnionWith(Area3f? range)
        {
            if (range == null)
            {
                return this;
            }

            return this.UnionWith(range.Value);
        }

        public Area3f UnionWith(Vector3f value)
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

        public bool Equals(Area3f other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Area3f))
            {
                return false;
            }

            return this.Equals((Area3f)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.Min.GetHashCode() + (7 * this.Max.GetHashCode());
            }
        }

        public override string ToString() => $"Area3f: min={this.Min} max={this.Max} size={this.Size}";
    }
}
