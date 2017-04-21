using System;

namespace UnityEngine
{
    public struct Rect : IEquatable<Rect>
    {
        public readonly Vector2 center;
        public readonly Vector2 size;

        public Rect(Vector2 min, Vector2 size)
        {
            this.center = min+size/2;
            this.size = size;
        }

        public Vector2 extents => size / 2;

        public Vector2 max => center + extents;

        public Vector2 min => center - extents;

        public static Rect zero { get; } = new Rect();

        public float yMax => center.y + size.y / 2f;

        public float xMax => center.x + size.x / 2f;

        public float yMin => center.y - size.y / 2f;

        public float xMin => center.x - size.x / 2f;

        public float height => size.y;

        public float width => size.x;
        
        public static Rect MinMaxRect(float xmin, float ymin, float xmax, float ymax)
        {
            float width = xmax - xmin;
            float height = ymax - ymin;

            return new Rect(new Vector2(xmin , ymin ), new Vector2(width, height));
        }
        
        public Vector2 Denormalize(Vector2 normalizedRectCoordinates)
        {
            return min + size * normalizedRectCoordinates;
        }

        public Vector2 Normalize(Vector2 point)
        {
            return (point - min) / size;
        }

        public bool Contains(Vector2 point)
        {
            return point >= min && point <= max;
        }

        public static bool operator ==(Rect a, Rect b)
        {
            return a.center == b.center && a.size == b.size;
        }

        public static bool operator !=(Rect a, Rect b)
        {
            return a.center != b.center || a.size != b.size;
        }

        public bool Equals(Rect other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Rect))
                return false;

            return this.Equals((Rect)obj);
        }

        public override int GetHashCode()
        {
            return center.GetHashCode() + 7 * size.GetHashCode();
        }

        public override string ToString()
        {
            return $"center:{center} size:{size}";
        }
    }
}