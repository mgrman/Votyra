using System;
using System.Collections.Generic;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    /// <summary>
    /// Max is exclusive
    /// </summary>
    public struct Rect3i : IEquatable<Rect3i>
    {
        public static readonly Rect3i All = new Rect3i(Vector3i.FromSame(int.MinValue / 2), Vector3i.FromSame(int.MaxValue));

        public static readonly Rect3i zero = new Rect3i();

        public readonly Vector3i min;

        public readonly Vector3i max;

        public Rect3i(Vector3i min, Vector3i size)
        {
            if (size.AnyNegative)
            {
                throw new InvalidOperationException($"When creating {nameof(Rect3i)} using min '{min}' and size '{size}', size cannot have a negative coordinate!");
            }
            this.min = min;
            this.max = min + size;
        }

        public Vector3i size => max - min;

        public static Rect3i CenterAndExtents(Vector3i center, Vector3i extents)
        {
            if (extents.AnyNegative)
            {
                throw new InvalidOperationException($"When creating {nameof(Rect3i)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            }
            return new Rect3i(center - extents + 1, extents + extents - 1);
        }

        public static Rect3i MinMaxRect(Vector3i min, Vector3i max)
        {
            return new Rect3i(min, max - min);
        }

        public void ForeachPointExlusive(Action<Vector3i> action)
        {
            for (int ix = this.min.x; ix < this.max.x; ix++)
            {
                for (int iy = this.min.y; iy < this.max.y; iy++)
                {
                    for (int iz = this.min.z; iz < this.max.z; iz++)
                    {
                        var i = new Vector3i(ix, iy, iz);
                        action(i);
                    }
                }
            }
        }

        public bool Overlaps(Rect3i that)
        {
            if (this.size == Vector3i.Zero || that.size == Vector3i.Zero)
                return false;

            bool overlapX = this.min.x <= that.max.x && that.min.x <= this.max.x;
            bool overlapY = this.min.y <= that.max.y && that.min.y <= this.max.y;
            bool overlapZ = this.min.z <= that.max.z && that.min.z <= this.max.z;
            return overlapX && overlapY && overlapZ;
        }

        public Rect3i CombineWith(Rect3i that)
        {
            if (this.size == Vector3i.Zero)
                return that;

            if (that.size == Vector3i.Zero)
                return this;

            var bMin = that.min;
            var bMax = that.max;

            Vector3i min = new Vector3i(Math.Min(this.min.x, bMin.x),
                         Math.Min(this.min.y, bMin.y),
                         Math.Min(this.min.z, bMin.z));
            Vector3i max = new Vector3i(Math.Max(this.max.x, bMax.x),
                         Math.Max(this.max.y, bMax.y),
                         Math.Max(this.max.z, bMax.z));
            return Rect3i.MinMaxRect(min, max);
        }

        public Rect3f ToBounds()
        {
            return Rect3f.FromMinMax(min.ToVector3f(), max.ToVector3f());
        }

        public Rect3i IntersectWith(Rect3i that)
        {
            if (this.size == Vector3i.Zero || that.size == Vector3i.Zero)
                return Rect3i.zero;

            int minX = Math.Max(this.min.x, that.min.x);
            int minY = Math.Max(this.min.y, that.min.y);
            int minZ = Math.Max(this.min.z, that.min.z);
            int maxX = Math.Max(Math.Min(this.max.x, that.max.x), minX);
            int maxY = Math.Max(Math.Min(this.max.y, that.max.y), minY);
            int maxZ = Math.Max(Math.Min(this.max.z, that.max.z), minZ);

            Vector3i min = new Vector3i(minX, minY, minZ);
            Vector3i max = new Vector3i(maxX, maxY, maxZ);
            return Rect3i.MinMaxRect(min, max);
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
            return $"min:{min} max:{max} size:{size}";
        }
    }
}