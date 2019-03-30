using System;

namespace Votyra.Core.Models
{
    public struct Area5f : IEquatable<Area5f>
    {
        public static readonly Area5f Zero = new Area5f();
        public static readonly Area5f All = new Area5f(Vector5fUtils.FromSame(float.MinValue / 2), Vector5fUtils.FromSame(float.MaxValue / 2));

        public readonly Vector5f Max;
        public readonly Vector5f Min;

        public Area1f X0 => Area1f.FromMinAndMax(Min.X0, Max.X0);

        public Area1f X1 => Area1f.FromMinAndMax(Min.X1, Max.X1);

        public Area1f X2 => Area1f.FromMinAndMax(Min.X2, Max.X2);

        public Area1f X3 => Area1f.FromMinAndMax(Min.X3, Max.X3);

        public Area1f X4 => Area1f.FromMinAndMax(Min.X4, Max.X4);

        private Area5f(Vector5f min, Vector5f max)
        {
            Min = min;
            Max = max;
        }

        public Vector5f Center => (Max + Min) / 2f;
        public Vector5f Size => Max - Min;
        public Vector5f Extents => Size / 2;

        public float DiagonalLength => Size.Magnitude();

        public static Area5f FromMinAndMax(Vector5f min, Vector5f max) => new Area5f(min, max);

        public static Area5f FromMinAndSize(Vector5f min, Vector5f size) => new Area5f(min, min + size);

        public static Area5f FromCenterAndSize(Vector5f center, Vector5f size) => new Area5f(center - size / 2, size);

        public static Area5f FromCenterAndExtents(Vector5f center, Vector5f extents)
        {
            if (extents.AnyNegative())
                throw new InvalidOperationException($"When creating {nameof(Area5f)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            return new Area5f(center - extents, center + extents);
        }

        public static bool operator ==(Area5f a, Area5f b) => a.Center == b.Center && a.Size == b.Size;

        public static bool operator !=(Area5f a, Area5f b) => a.Center != b.Center || a.Size != b.Size;

        public static Area5f operator /(Area5f a, float b) => FromMinAndMax(a.Min / b, a.Max / b);

        public static Area5f operator /(Area5f a, Vector5f b) => FromMinAndMax(a.Min / b, a.Max / b);

        public Area5f IntersectWith(Area5f that)
        {
            if (Size == Vector5f.Zero || that.Size == Vector5f.Zero)
                return Zero;

            var min = Vector5fUtils.Max(Min, that.Min);
            var max = Vector5fUtils.Max(Vector5fUtils.Min(Max, that.Max), min);

            return FromMinAndMax(min, max);
        }

        public Area5f Encapsulate(Vector5f point) => FromMinAndMax(Vector5fUtils.Min(Min, point), Vector5fUtils.Max(Max, point));

        public Area5f Encapsulate(Area5f bounds) => FromMinAndMax(Vector5fUtils.Min(Min, bounds.Min), Vector5fUtils.Max(Max, bounds.Max));

        public bool Contains(Vector5f point) => point >= Min && point <= Max;

        public Range5i RoundToInt() => Range5i.FromMinAndMax(Min.RoundToVector5i(), Max.RoundToVector5i());

        public Range5i RoundToContain() => Range5i.FromMinAndMax(Min.FloorToVector5i(), Max.CeilToVector5i());

        public Area5f CombineWith(Area5f that)
        {
            if (Size == Vector5f.Zero)
                return that;

            if (that.Size == Vector5f.Zero)
                return this;

            var min = Vector5fUtils.Min(Min, that.Min);
            var max = Vector5fUtils.Max(Max, that.Max);
            return FromMinAndMax(min, max);
        }

        public Area5f UnionWith(Area5f range) => FromMinAndMax(Vector5fUtils.Min(Min, range.Min), Vector5fUtils.Min(Max, range.Max));

        public Area5f? UnionWith(Area5f? range)
        {
            if (range == null)
                return this;

            return UnionWith(range.Value);
        }

        public Area5f UnionWith(Vector5f value)
        {
            if (value < Min)
                return FromMinAndMax(value, Max);
            if (value > Max)
                return FromMinAndMax(Min, value);
            return this;
        }

        public bool Equals(Area5f other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Area5f))
                return false;

            return Equals((Area5f) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Min.GetHashCode() + 7 * Max.GetHashCode();
            }
        }

        public override string ToString() => $"Area5f: min={Min} max={Max} size={Size}";
    }
}