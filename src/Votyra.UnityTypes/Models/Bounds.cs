using System;

namespace UnityEngine
{
    public struct Bounds : IEquatable<Bounds>
    {
        public readonly Vector3 center;
        public readonly Vector3 size;

        public Bounds(Vector3 center, Vector3 size)
        {
            this.center = center;
            this.size = size;
        }

        public static Bounds FromMinMax(Vector3 min, Vector3 max)
        {
            var size = max - min;
            return new Bounds(min + (size / 2), size);
        }

        public Vector3 extents => size / 2;

        public Vector3 min => center - extents;

        public Vector3 max => center + extents;

        public bool Contains(Vector3 point)
        {
            return point >= min && point <= max;
        }

        public Bounds Encapsulate(Vector3 point)
        {
            return Bounds.FromMinMax(Vector3.Min(this.min, point), Vector3.Max(this.max, point));
        }

        public Bounds Encapsulate(Bounds bounds)
        {
            return Bounds.FromMinMax(Vector3.Min(this.min, bounds.min), Vector3.Max(this.max, bounds.max));
        }

        public Bounds Expand(float amount)
        {
            return new Bounds(this.center, this.size + amount);
        }

        public Bounds Expand(Vector3 amount)
        {
            return new Bounds(this.center, this.size + amount);
        }

        public static bool operator ==(Bounds a, Bounds b)
        {
            return a.center == b.center && a.size == b.size;
        }

        public static bool operator !=(Bounds a, Bounds b)
        {
            return a.center != b.center || a.size != b.size;
        }
        
        public bool Equals(Bounds other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Bounds))
                return false;

            return this.Equals((Bounds)obj);
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