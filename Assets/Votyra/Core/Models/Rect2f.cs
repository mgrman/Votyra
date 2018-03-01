using System;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct Rect2f : IEquatable<Rect2f>
    {
        public float x => center.x - size.x / 2;
        public float y => center.y - size.y / 2;
        public readonly Vector2f center;
        public readonly Vector2f size;

        public Rect2f(float minX, float minY, float sizeX, float sizeY)
        {
            this.center = new Vector2f(minX + sizeX / 2, minY + sizeY / 2);
            this.size = new Vector2f(sizeX, sizeY);
        }

        public Rect2f(Vector2f min, Vector2f size)
        {
            this.center = min + size / 2;
            this.size = size;
        }

        public Vector2f extents => size / 2;

        public Vector2f max => center + extents;

        public Vector2f min => center - extents;

        public static Rect2f zero { get; } = new Rect2f();

        public float yMax => center.y + size.y / 2f;

        public float xMax => center.x + size.x / 2f;

        public float yMin => center.y - size.y / 2f;

        public float xMin => center.x - size.x / 2f;

        public float height => size.y;

        public float width => size.x;

        public static Rect2f MinMaxRect(float xmin, float ymin, float xmax, float ymax)
        {
            float width = xmax - xmin;
            float height = ymax - ymin;

            return new Rect2f(new Vector2f(xmin, ymin), new Vector2f(width, height));
        }

        public Vector2f Denormalize(Vector2f normalizedRectCoordinates)
        {
            return min + size * normalizedRectCoordinates;
        }

        public Vector2f Normalize(Vector2f point)
        {
            return (point - min) / size;
        }

        public bool Contains(Vector2f point)
        {
            return point >= min && point <= max;
        }

        public static bool operator ==(Rect2f a, Rect2f b)
        {
            return a.center == b.center && a.size == b.size;
        }

        public static bool operator !=(Rect2f a, Rect2f b)
        {
            return a.center != b.center || a.size != b.size;
        }

        public static readonly Rect2f All = new Rect2f(float.MinValue / 2, float.MinValue / 2, float.MaxValue, float.MaxValue);

        public Rect2f GetRectangle(Vector2i cellCount, Vector2i cell)
        {
            var step = this.size.DivideBy(cellCount);
            var pos = this.min + step * cell;

            return new Rect2f(pos, step);
        }

        public Rect2f FromCenterAndSize(Vector2f center, Vector2f size)
        {
            return new Rect2f(center - size / 2, size);
        }

        public Rect2f FromCenterAndSize(float centerX, float centerY, float sizeX, float sizeY)
        {
            return new Rect2f(new Vector2f(centerX - sizeX / 2, centerY - sizeY / 2), new Vector2f(sizeX, sizeY));
        }

        public Rect2f FromMinAndSize(float minX, float minY, float sizeX, float sizeY)
        {
            return new Rect2f(new Vector2f(minX, minY), new Vector2f(sizeX, sizeY));
        }

        public Rect2i RoundToInt()
        {
            return new Rect2i(MathUtils.RoundToInt(this.x), MathUtils.RoundToInt(this.y), MathUtils.RoundToInt(this.width), MathUtils.RoundToInt(this.height));
        }

        public Rect2i RoundToContain()
        {
            return Rect2i.MinMaxRect(MathUtils.FloorToInt(this.xMin), MathUtils.FloorToInt(this.yMin), MathUtils.CeilToInt(this.xMax), MathUtils.CeilToInt(this.yMax));
        }

        public Rect2f CombineWith(Rect2f that)
        {
            var aMin = this.min;
            var aMax = this.max;
            var bMin = that.min;
            var bMax = that.max;
            return Rect2f
                     .MinMaxRect(
                         Math.Min(aMin.x, bMin.x),
                         Math.Min(aMin.y, bMin.y),
                         Math.Max(aMax.x, bMax.x),
                         Math.Max(aMax.y, bMax.y));
        }

        public bool Equals(Rect2f other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Rect2f))
                return false;

            return this.Equals((Rect2f)obj);
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