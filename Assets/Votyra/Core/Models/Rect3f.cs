using System;
using System.Collections.Generic;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct Rect3f : IEquatable<Rect3f>
    {
        public readonly Vector3f center;
        public readonly Vector3f size;

        public static Rect3f zero { get; } = new Rect3f();

        public Rect3f(Vector3f center, Vector3f size)
        {
            this.center = center;
            this.size = size;
        }

        public static Rect3f FromMinMax(Vector3f min, Vector3f max)
        {
            var size = max - min;
            return new Rect3f(min + (size / 2), size);
        }

        public Vector3f extents => size / 2;

        public Vector3f min => center - extents;

        public Vector3f max => center + extents;

        public bool Contains(Vector3f point)
        {
            return point >= min && point <= max;
        }

        public Rect3f Encapsulate(Vector3f point)
        {
            return Rect3f.FromMinMax(Vector3f.Min(this.min, point), Vector3f.Max(this.max, point));
        }

        public Rect3f Encapsulate(Rect3f bounds)
        {
            return Rect3f.FromMinMax(Vector3f.Min(this.min, bounds.min), Vector3f.Max(this.max, bounds.max));
        }

        public Rect3f Expand(float amount)
        {
            return new Rect3f(this.center, this.size + amount);
        }

        public Rect3f Expand(Vector3f amount)
        {
            return new Rect3f(this.center, this.size + amount);
        }

        public Rect3i RoundToContain()
        {
            return Rect3i.MinMaxRect(MathUtils.FloorToInt(this.min.x), MathUtils.FloorToInt(this.min.y), MathUtils.FloorToInt(this.min.z), MathUtils.CeilToInt(this.max.x), MathUtils.CeilToInt(this.max.y), MathUtils.CeilToInt(this.max.z));
        }

        public Rect3f GetBounds(Vector2i cellCount, Vector2i cell)
        {
            var rect = Rect2f.MinMaxRect(this.min.x, this.min.y, this.size.x, this.size.y);

            var step = rect.size.DivideBy(cellCount);
            var pos = rect.min + (step * cell);

            var center = new Vector3f(pos.x + (step.x / 2), pos.y + (step.y / 2), this.center.z);
            var size = new Vector3f(step.x, step.y, this.size.z);

            return new Rect3f(center, size);
        }

        public List<Vector3f> GetCorners(bool includePosition = true)
        {
            var halfSize = this.size / 2;
            var result = new List<Vector3f>();
            for (int x = -1; x <= 1; x += 2)
                for (int y = -1; y <= 1; y += 2)
                    for (int z = -1; z <= 1; z += 2)
                    {
                        result.Add((includePosition ? this.center : Vector3f.Zero) + new Vector3f(halfSize.x * x, halfSize.y * y, halfSize.z * z));
                    }
            return result;
        }

        public static bool operator ==(Rect3f a, Rect3f b)
        {
            return a.center == b.center && a.size == b.size;
        }

        public static bool operator !=(Rect3f a, Rect3f b)
        {
            return a.center != b.center || a.size != b.size;
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