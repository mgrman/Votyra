using System;
using System.Collections.Generic;

namespace Votyra.Core.Models
{
    public struct Area3f : IEquatable<Area3f>
    {
        public static readonly Area3f Zero = new Area3f();

        public readonly Vector3f Min;

        public readonly Vector3f Max;

        private Area3f(Vector3f min, Vector3f max)
        {
            Min = min;
            Max = max;
        }

        public Vector3f Center => (Min + Max) / 2f;

        public Vector3f Size => Max - Min;

        public Vector3f Extents => Size / 2f;

        public IEnumerable<Vector3f> Corners
        {
            get
            {
                yield return new Vector3f(Min.X, Min.Y, Min.Z);
                yield return new Vector3f(Min.X, Min.Y, Max.Z);
                yield return new Vector3f(Min.X, Max.Y, Min.Z);
                yield return new Vector3f(Min.X, Max.Y, Max.Z);
                yield return new Vector3f(Max.X, Min.Y, Min.Z);
                yield return new Vector3f(Max.X, Min.Y, Max.Z);
                yield return new Vector3f(Max.X, Max.Y, Min.Z);
                yield return new Vector3f(Max.X, Max.Y, Max.Z);
            }
        }

        public float DiagonalLength => Size.Magnitude;

        public static Area3f FromCenterAndExtents(Vector3f center, Vector3f extents)
        {
            if (extents.AnyNegative)
                throw new InvalidOperationException($"When creating {nameof(Area3f)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            return new Area3f(center - extents, center + extents);
        }

        public static Area3f FromMinAndMax(Vector3f min, Vector3f max) => new Area3f(min, max);

        public static Area3f FromMinAndSize(Vector3f min, Vector3f size)
        {
            if (size.AnyNegative)
                throw new InvalidOperationException($"When creating {nameof(Area3f)} using min '{min}' and size '{size}', size cannot have a negative coordinate!");
            return new Area3f(min, min + size);
        }

        public static Area3f operator /(Area3f a, float b) => FromMinAndMax(a.Min / b, a.Max / b);

        public static Area3f operator /(Area3f a, Vector3f b) => FromMinAndMax(a.Min / b, a.Max / b);

        public static bool operator ==(Area3f a, Area3f b) => a.Min == b.Min && a.Max == b.Max;

        public static bool operator !=(Area3f a, Area3f b) => a.Min != b.Min || a.Max != b.Max;

        public Area3f IntersectWith(Area3f that)
        {
            if (Size == Vector3f.Zero || that.Size == Vector3f.Zero)
                return Zero;

            var min = Vector3f.Max(Min, that.Min);
            var max = Vector3f.Max(Vector3f.Min(Max, that.Max), min);

            return FromMinAndMax(min, max);
        }

        public Area3f Encapsulate(Vector3f point) => FromMinAndMax(Vector3f.Min(Min, point), Vector3f.Max(Max, point));
        public Area3f Encapsulate(Vector3f a, Vector3f b, Vector3f c) => FromMinAndMax(Vector3f.Min(Vector3f.Min(Vector3f.Min(Min, a), b), c), Vector3f.Max(Vector3f.Max(Vector3f.Max(Max, a), b), c));

        public Area3f Encapsulate(Area3f bounds) => FromMinAndMax(Vector3f.Min(Min, bounds.Min), Vector3f.Max(Max, bounds.Max));

        public Range3i RoundToContain() => Range3i.FromMinAndMax(Min.FloorToVector3i(), Max.CeilToVector3i());

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
                return Center.GetHashCode() + 7 * Size.GetHashCode();
            }
        }

        public override string ToString() => $"Area3f: min={Min} max={Max} size={Size}";
    }
}