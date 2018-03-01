using System;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct Rect2i : IEquatable<Rect2i>
    {
        public static readonly Rect2i All = new Rect2i(int.MinValue / 2, int.MinValue / 2, int.MaxValue, int.MaxValue);

        public readonly Vector2i min;
        public readonly Vector2i max;

        public Rect2i(int minX, int minY, int sizeX, int sizeY)
        {
            this.min = new Vector2i(minX, minY);
            this.max = new Vector2i(minX + sizeX, minY + sizeY);
        }

        public Rect2i(Vector2i min, Vector2i size)
        {
            this.min = min;
            this.max = min + size;
        }

        public Vector2i extents => (max - min) / 2;

        public Vector2i center => min + extents;

        public Vector2i size => max - min;

        public static Rect2i zero { get; } = new Rect2i();

        public int yMax => max.y;

        public int xMax => max.x;

        public int yMin => min.y;

        public int xMin => min.x;

        public int height => max.y - min.y;

        public int width => max.x - min.x;

        public static Rect2i CenterAndExtents(Vector2i center, Vector2i extents)
        {
            return new Rect2i(center - extents, Vector2i.One + extents + extents);
        }

        public static Rect2i MinMaxRect(int xmin, int ymin, int xmax, int ymax)
        {

            return new Rect2i(new Vector2i(xmin, ymin), new Vector2i(xmax - xmin, ymax - ymin));
        }

        public static Rect2i MinMaxRect(Vector2i min, Vector2i max)
        {
            return new Rect2i(min, max - min);
        }

        public Vector2i Denormalize(Vector2f normalizedRectCoordinates)
        {
            return min + (size * normalizedRectCoordinates).ToVector2i();
        }

        public Vector2f Normalize(Vector2i point)
        {
            return (point - min) / size.ToVector2f();
        }

        public bool Contains(Vector2i point)
        {
            return point >= min && point <= max;
        }

        public bool Overlaps(Rect2i that)
        {
            bool overlapX = this.xMin < that.xMax && that.xMin < this.xMax;
            bool overlapY = this.yMin < that.yMax && that.yMin < this.yMax;
            return overlapX && overlapY;
        }

        public Rect2i CombineWith(Rect2i b)
        {
            var bMin = b.min;
            var bMax = b.max;
            return Rect2i
                .MinMaxRect(
                    Math.Min(this.min.x, bMin.x),
                    Math.Min(this.min.y, bMin.y),
                    Math.Max(this.max.x, bMax.x),
                    Math.Max(this.max.y, bMax.y));
        }

        public Rect2i IntersectWith(Rect2i b)
        {
            var bMin = b.min;
            var bMax = b.max;
            int minX = Math.Max(this.min.x, bMin.x);
            int minY = Math.Max(this.min.y, bMin.y);
            int maxX = Math.Min(this.max.x, bMax.x);
            int maxY = Math.Min(this.max.y, bMax.y);

            maxX = Math.Max(minX, maxX);
            maxY = Math.Max(minY, maxY);
            return Rect2i.MinMaxRect(minX, minY, maxX, maxY);
        }

        public Rect2i CombineWith(Vector2i b)
        {
            if (Contains(b))
                return this;

            var bMin = b;
            var bMax = b;
            return Rect2i
                .MinMaxRect(
                    Math.Min(this.min.x, bMin.x),
                    Math.Min(this.min.y, bMin.y),
                    Math.Max(this.max.x, bMax.x),
                    Math.Max(this.max.y, bMax.y));
        }

        public Rect2f ToRect()
        {
            return new Rect2f(min.x, min.y, max.x - min.x, max.y - min.y);
        }

        public static bool operator ==(Rect2i a, Rect2i b)
        {
            return a.min == b.min && a.max == b.max;
        }

        public static bool operator !=(Rect2i a, Rect2i b)
        {
            return a.min != b.min || a.max != b.max;
        }

        public bool Equals(Rect2i other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Rect2i))
                return false;

            return this.Equals((Rect2i)obj);
        }

        public override int GetHashCode()
        {
            return min.GetHashCode() + 7 * max.GetHashCode();
        }

        public override string ToString()
        {
            return $"min:{min} max:{max}";
        }
    }
}