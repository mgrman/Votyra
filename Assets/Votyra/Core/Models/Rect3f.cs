using System;

namespace Votyra.Core.Models
{
    public struct Rect3f : IEquatable<Rect3f>
    {
        public static readonly Rect3f zero = new Rect3f();

        public readonly Vector3f min;

        public readonly Vector3f max;

        public Vector3f center => (min + max) / 2f;

        public Vector3f size => max - min;

        public Vector3f extents => size / 2f;

        private Rect3f(Vector3f min, Vector3f max)
        {
            this.min = min;
            this.max = max;
        }

        public static Rect3f FromMinAndMax(Vector3f min, Vector3f max)
        {
            return new Rect3f(min, max);
        }

        public static Rect3f FromMinAndSize(Vector3f min, Vector3f size)
        {
            if (size.AnyNegative)
            {
                throw new InvalidOperationException($"When creating {nameof(Rect3f)} using min '{min}' and size '{size}', size cannot have a negative coordinate!");
            }
            return new Rect3f(min, min + size);
        }

        public float DiagonalLength
        {
            get
            {
                return (float)Math.Sqrt(size.X * size.X + size.Y * size.Y + size.Z * size.Z);
            }
        }

        public Rect3f Encapsulate(Vector3f point)
        {
            return Rect3f.FromMinAndMax(Vector3f.Min(this.min, point), Vector3f.Max(this.max, point));
        }

        public Rect3f Encapsulate(Rect3f bounds)
        {
            return Rect3f.FromMinAndMax(Vector3f.Min(this.min, bounds.min), Vector3f.Max(this.max, bounds.max));
        }

        public Rect3i RoundToContain()
        {
            return Rect3i.FromMinAndMax(this.min.FloorToVector3i(), this.max.CeilToVector3i());
        }

        public static bool operator ==(Rect3f a, Rect3f b)
        {
            return a.min == b.min && a.max == b.max;
        }

        public static bool operator !=(Rect3f a, Rect3f b)
        {
            return a.min != b.min || a.max != b.max;
        }

        public bool Equals(Rect3f other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Rect3f))
                return false;

            return this.Equals((Rect3f)obj);
        }

        public override int GetHashCode()
        {
            return center.GetHashCode() + (7 * size.GetHashCode());
        }

        public override string ToString()
        {
            return $"center:{center} size:{size}";
        }
    }
}