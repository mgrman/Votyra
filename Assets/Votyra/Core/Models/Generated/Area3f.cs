using System;

namespace Votyra.Core.Models
{
    public struct Area3f : IEquatable<Area3f>
    {
        public static readonly Area3f Zero = new Area3f();
        public static readonly Area3f All = new Area3f(Vector3fUtils.FromSame(float.MinValue / 2), Vector3fUtils.FromSame(float.MaxValue / 2));

        public readonly Vector3f Max;
        public readonly Vector3f Min;

        public Area1f X => Area1f.FromMinAndMax(Min.X, Max.X);

        public Area1f Y => Area1f.FromMinAndMax(Min.Y, Max.Y);

        public Area1f Z => Area1f.FromMinAndMax(Min.Z, Max.Z);

        private Area3f(Vector3f min, Vector3f max)
        {
            Min = min;
            Max = max;
        }

        public Vector3f Center => (Max + Min) / 2f;
        public Vector3f Size => Max - Min;
        public Vector3f Extents => Size / 2;

        public float DiagonalLength => Size.Magnitude();

        public static Area3f FromMinAndMax(Vector3f min, Vector3f max) => new Area3f(min, max);

        public static Area3f FromMinAndSize(Vector3f min, Vector3f size) => new Area3f(min, min + size);

        public static Area3f FromCenterAndSize(Vector3f center, Vector3f size) => new Area3f(center - size / 2, size);

        public static Area3f FromCenterAndExtents(Vector3f center, Vector3f extents)
        {
            if (extents.AnyNegative())
                throw new InvalidOperationException($"When creating {nameof(Area3f)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            return new Area3f(center - extents, center + extents);
        }

        public static bool operator ==(Area3f a, Area3f b) => a.Center == b.Center && a.Size == b.Size;

        public static bool operator !=(Area3f a, Area3f b) => a.Center != b.Center || a.Size != b.Size;

        public static Area3f operator /(Area3f a, float b) => FromMinAndMax(a.Min / b, a.Max / b);

        public static Area3f operator /(Area3f a, Vector3f b) => FromMinAndMax(a.Min / b, a.Max / b);

        public Area3f IntersectWith(Area3f that)
        {
            if (Size == Vector3f.Zero || that.Size == Vector3f.Zero)
                return Zero;

            var min = Vector3fUtils.Max(Min, that.Min);
            var max = Vector3fUtils.Max(Vector3fUtils.Min(Max, that.Max), min);

            return FromMinAndMax(min, max);
        }

        public Area3f Encapsulate(Vector3f point) => FromMinAndMax(Vector3fUtils.Min(Min, point), Vector3fUtils.Max(Max, point));

        public Area3f Encapsulate(Area3f bounds) => FromMinAndMax(Vector3fUtils.Min(Min, bounds.Min), Vector3fUtils.Max(Max, bounds.Max));

        public bool Contains(Vector3f point) => point >= Min && point <= Max;

        public Range3i RoundToInt() => Range3i.FromMinAndMax(Min.RoundToVector3i(), Max.RoundToVector3i());

        public Range3i RoundToContain() => Range3i.FromMinAndMax(Min.FloorToVector3i(), Max.CeilToVector3i());

        public Area3f CombineWith(Area3f that)
        {
            if (Size == Vector3f.Zero)
                return that;

            if (that.Size == Vector3f.Zero)
                return this;

            var min = Vector3fUtils.Min(Min, that.Min);
            var max = Vector3fUtils.Max(Max, that.Max);
            return FromMinAndMax(min, max);
        }

        public Area3f UnionWith(Area3f range) => FromMinAndMax(Vector3fUtils.Min(Min, range.Min), Vector3fUtils.Min(Max, range.Max));

        public Area3f? UnionWith(Area3f? range)
        {
            if (range == null)
                return this;

            return UnionWith(range.Value);
        }

        public Area3f UnionWith(Vector3f value)
        {
            if (value < Min)
                return FromMinAndMax(value, Max);
            if (value > Max)
                return FromMinAndMax(Min, value);
            return this;
        }

        public bool Equals(Area3f other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Area3f))
                return false;

            return Equals((Area3f) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Min.GetHashCode() + 7 * Max.GetHashCode();
            }
        }

        public override string ToString() => $"Area3f: min={Min} max={Max} size={Size}";
    }
}