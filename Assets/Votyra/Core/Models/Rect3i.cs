using System;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct Rect3i : IEquatable<Rect3i>
    {
        public static readonly Rect3i All = new Rect3i(int.MinValue / 2, int.MinValue / 2, int.MinValue / 2, int.MaxValue, int.MaxValue, int.MaxValue);

        public readonly Vector3i min;
        public readonly Vector3i max;

        public Rect3i(int minX, int minY, int minZ, int sizeX, int sizeY, int sizeZ)
        {
            this.min = new Vector3i(minX, minY, minZ);
            this.max = new Vector3i(minX + sizeX, minY + sizeY, minZ + sizeZ);
        }

        public Rect3i(Vector3i min, Vector3i size)
        {
            this.min = min;
            this.max = min + size;
        }

        public Vector3i extents => (max - min) / 2;

        public Vector3i center => min + extents;

        public Vector3i size => max - min;

        public static Rect3i zero { get; } = new Rect3i();

        public int zMax => max.z;
        public int yMax => max.y;
        public int xMax => max.x;

        public int zMin => min.z;
        public int yMin => min.y;
        public int xMin => min.x;

        public int height => max.y - min.y;

        public int width => max.x - min.x;

        public static Rect3i CenterAndExtents(Vector3i center, Vector3i extents)
        {
            return new Rect3i(center - extents, Vector3i.One + extents + extents);
        }

        public static Rect3i MinMaxRect(int xmin, int ymin, int zmin, int xmax, int ymax, int zmax)
        {
            return new Rect3i(new Vector3i(xmin, ymin, zmin), new Vector3i(xmax - xmin, ymax - ymin, zmax - zmin));
        }

        public Vector3i Denormalize(Vector3f normalizedRectCoordinates)
        {
            return min + (size * normalizedRectCoordinates).ToVector3i();
        }

        public Vector3f Normalize(Vector3i point)
        {
            return (point - min) / size.ToVector3f();
        }

        public bool Contains(Vector3i point)
        {
            return point >= min && point <= max;
        }

        public bool Overlaps(Rect3i that)
        {
            bool overlapX = this.xMin < that.xMax && that.xMin < this.xMax;
            bool overlapY = this.yMin < that.yMax && that.yMin < this.yMax;
            return overlapX && overlapY;
        }

        public Rect3i CombineWith(Rect3i b)
        {
            var bMin = b.min;
            var bMax = b.max;
            return Rect3i
                     .MinMaxRect(
                         Math.Min(this.min.x, bMin.x),
                         Math.Min(this.min.y, bMin.y),
                         Math.Min(this.min.z, bMin.z),
                         Math.Max(this.max.x, bMax.x),
                         Math.Max(this.max.y, bMax.y),
                         Math.Max(this.max.z, bMax.z));
        }

        public Rect3f ToBounds()
        {
            return new Rect3f(center.ToVector3f(), size.ToVector3f());
        }

        public Rect3i IntersectWith(Rect3i b)
        {
            var bMin = b.min;
            var bMax = b.max;
            int minX = Math.Max(this.min.x, bMin.x);
            int minY = Math.Max(this.min.y, bMin.y);
            int minZ = Math.Max(this.min.z, bMin.z);
            int maxX = Math.Min(this.max.x, bMax.x);
            int maxY = Math.Min(this.max.y, bMax.y);
            int maxZ = Math.Min(this.max.z, bMax.z);

            maxX = Math.Max(minX, maxX);
            maxY = Math.Max(minY, maxY);
            maxZ = Math.Max(minZ, maxZ);
            return Rect3i.MinMaxRect(minX, minY, minZ, maxX, maxY, maxZ);
        }


        public static bool operator ==(Rect3i a, Rect3i b)
        {
            return a.min == b.min && a.max == b.max;
        }

        public static bool operator !=(Rect3i a, Rect3i b)
        {
            return a.min != b.min || a.max != b.max;
        }

        public bool Equals(Rect3i other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Rect3i))
                return false;

            return this.Equals((Rect3i)obj);
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