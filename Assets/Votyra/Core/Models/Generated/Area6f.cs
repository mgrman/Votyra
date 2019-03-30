using System;

namespace Votyra.Core.Models
{
    public struct Area6f : IEquatable<Area6f>
    {
        public static readonly Area6f Zero = new Area6f();
        public static readonly Area6f All = new Area6f(Vector6fUtils.FromSame(float.MinValue / 2), Vector6fUtils.FromSame(float.MaxValue / 2));

        public readonly Vector6f Max;
        public readonly Vector6f Min;

        public Area1f X0 => Area1f.FromMinAndMax(Min.X0, Max.X0);

        public Area1f X1 => Area1f.FromMinAndMax(Min.X1, Max.X1);

        public Area1f X2 => Area1f.FromMinAndMax(Min.X2, Max.X2);

        public Area1f X3 => Area1f.FromMinAndMax(Min.X3, Max.X3);

        public Area1f X4 => Area1f.FromMinAndMax(Min.X4, Max.X4);

        public Area1f X5 => Area1f.FromMinAndMax(Min.X5, Max.X5);

        private Area6f(Vector6f min, Vector6f max)
        {
            Min = min;
            Max = max;
        }

        public Vector6f Center => (Max + Min) / 2f;
        public Vector6f Size => Max - Min;
        public Vector6f Extents => Size / 2;

        public float DiagonalLength => Size.Magnitude();

        public static Area6f FromMinAndMax(Vector6f min, Vector6f max) => new Area6f(min, max);

        public static Area6f FromMinAndSize(Vector6f min, Vector6f size) => new Area6f(min, min + size);

        public static Area6f FromCenterAndSize(Vector6f center, Vector6f size) => new Area6f(center - size / 2, size);

        public static Area6f FromCenterAndExtents(Vector6f center, Vector6f extents)
        {
            if (extents.AnyNegative())
                throw new InvalidOperationException($"When creating {nameof(Area6f)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            return new Area6f(center - extents, center + extents);
        }

        public static bool operator ==(Area6f a, Area6f b) => a.Center == b.Center && a.Size == b.Size;

        public static bool operator !=(Area6f a, Area6f b) => a.Center != b.Center || a.Size != b.Size;

        public static Area6f operator /(Area6f a, float b) => FromMinAndMax(a.Min / b, a.Max / b);

        public static Area6f operator /(Area6f a, Vector6f b) => FromMinAndMax(a.Min / b, a.Max / b);

        public Area6f IntersectWith(Area6f that)
        {
            if (Size == Vector6f.Zero || that.Size == Vector6f.Zero)
                return Zero;

            var min = Vector6fUtils.Max(Min, that.Min);
            var max = Vector6fUtils.Max(Vector6fUtils.Min(Max, that.Max), min);

            return FromMinAndMax(min, max);
        }

        public Area6f Encapsulate(Vector6f point) => FromMinAndMax(Vector6fUtils.Min(Min, point), Vector6fUtils.Max(Max, point));

        public Area6f Encapsulate(Area6f bounds) => FromMinAndMax(Vector6fUtils.Min(Min, bounds.Min), Vector6fUtils.Max(Max, bounds.Max));

        public bool Contains(Vector6f point) => point >= Min && point <= Max;

        public Range6i RoundToInt() => Range6i.FromMinAndMax(Min.RoundToVector6i(), Max.RoundToVector6i());

        public Range6i RoundToContain() => Range6i.FromMinAndMax(Min.FloorToVector6i(), Max.CeilToVector6i());

        public Area6f CombineWith(Area6f that)
        {
            if (Size == Vector6f.Zero)
                return that;

            if (that.Size == Vector6f.Zero)
                return this;

            var min = Vector6fUtils.Min(Min, that.Min);
            var max = Vector6fUtils.Max(Max, that.Max);
            return FromMinAndMax(min, max);
        }

        public Area6f UnionWith(Area6f range) => FromMinAndMax(Vector6fUtils.Min(Min, range.Min), Vector6fUtils.Min(Max, range.Max));

        public Area6f? UnionWith(Area6f? range)
        {
            if (range == null)
                return this;

            return UnionWith(range.Value);
        }

        public Area6f UnionWith(Vector6f value)
        {
            if (value < Min)
                return FromMinAndMax(value, Max);
            if (value > Max)
                return FromMinAndMax(Min, value);
            return this;
        }

        public bool Equals(Area6f other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Area6f))
                return false;

            return Equals((Area6f) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Min.GetHashCode() + 7 * Max.GetHashCode();
            }
        }

        public override string ToString() => $"Area6f: min={Min} max={Max} size={Size}";
    }
}