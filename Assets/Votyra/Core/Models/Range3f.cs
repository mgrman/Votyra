using System;
using System.Collections.Generic;

namespace Votyra.Core.Models
{
    public struct Range3f : IEquatable<Range3f>
    {
        public static readonly Range3f zero = new Range3f();

        public readonly Vector3f Max;
        public readonly Vector3f Min;

        private Range3f(Vector3f min, Vector3f max)
        {
            this.Min = min;
            this.Max = max;
        }

        public Vector3f Center => (Min + Max) / 2f;

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

        public float DiagonalLength
        {
            get
            {
                return Size.Magnitude;
            }
        }

        public Vector3f Extents => Size / 2f;
        public Vector3f Size => Max - Min;

        public static Range3f FromCenterAndExtents(Vector3f center, Vector3f extents)
        {
            if (extents.AnyNegative)
            {
                throw new InvalidOperationException($"When creating {nameof(Range3f)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            }
            return new Range3f(center - extents, center + extents);
        }

        public static Range3f FromMinAndMax(Vector3f min, Vector3f max)
        {
            return new Range3f(min, max);
        }

        public static Range3f FromMinAndSize(Vector3f min, Vector3f size)
        {
            if (size.AnyNegative)
            {
                throw new InvalidOperationException($"When creating {nameof(Range3f)} using min '{min}' and size '{size}', size cannot have a negative coordinate!");
            }
            return new Range3f(min, min + size);
        }

        public static bool operator !=(Range3f a, Range3f b)
        {
            return a.Min != b.Min || a.Max != b.Max;
        }

        public static Range3f operator /(Range3f a, float b)
        {
            return Range3f.FromMinAndMax(a.Min / b, a.Max / b);
        }

        public static Range3f operator /(Range3f a, Vector3f b)
        {
            return Range3f.FromMinAndMax(a.Min / b, a.Max / b);
        }

        public static bool operator ==(Range3f a, Range3f b)
        {
            return a.Min == b.Min && a.Max == b.Max;
        }

        public Range3f Encapsulate(Vector3f point)
        {
            return Range3f.FromMinAndMax(Vector3f.Min(this.Min, point), Vector3f.Max(this.Max, point));
        }

        public Range3f Encapsulate(Range3f bounds)
        {
            return Range3f.FromMinAndMax(Vector3f.Min(this.Min, bounds.Min), Vector3f.Max(this.Max, bounds.Max));
        }

        public bool Equals(Range3f other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Range3f))
                return false;

            return this.Equals((Range3f)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Center.GetHashCode() + (7 * Size.GetHashCode());
            }
        }

        public Range3f IntersectWith(Range3f that)
        {
            if (this.Size == Vector3f.Zero || that.Size == Vector3f.Zero)
                return Range3f.zero;

            var min = Vector3f.Max(this.Min, that.Min);
            var max = Vector3f.Max(Vector3f.Min(this.Max, that.Max), min);

            return Range3f.FromMinAndMax(min, max);
        }

        public Range3i RoundToContain()
        {
            return Range3i.FromMinAndMax(this.Min.FloorToVector3i(), this.Max.CeilToVector3i());
        }

        public override string ToString()
        {
            return $"center:{Center} size:{Size}";
        }
    }
}