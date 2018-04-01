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
        public static readonly Rect3i All = new Rect3i(Vector3i.FromSame(int.MinValue / 2), Vector3i.FromSame(int.MaxValue) / 2);

        public static readonly Rect3i Zero = new Rect3i();

        public readonly Vector3i Min;

        public readonly Vector3i Max;

        public Vector3i Size => Max - Min;

        private Rect3i(Vector3i min, Vector3i max)
        {
            this.Min = min;
            this.Max = max;
        }

        public static Rect3i FromCenterAndExtents(Vector3i center, Vector3i extents)
        {
            if (extents.AnyNegative)
            {
                throw new InvalidOperationException($"When creating {nameof(Rect3i)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            }
            return new Rect3i(center - extents + 1, center + extents);
        }

        public static Rect3i FromMinAndSize(Vector3i min, Vector3i size)
        {
            if (size.AnyNegative)
            {
                throw new InvalidOperationException($"When creating {nameof(Rect3i)} using min '{min}' and size '{size}', size cannot have a negative coordinate!");
            }
            return new Rect3i(min, min + size);
        }

        public static Rect3i FromMinAndMax(Vector3i min, Vector3i max)
        {
            return new Rect3i(min, max);
        }

        public void ForeachPointExlusive(Action<Vector3i> action)
        {
            for (int ix = this.Min.X; ix < this.Max.X; ix++)
            {
                for (int iy = this.Min.Y; iy < this.Max.Y; iy++)
                {
                    for (int iz = this.Min.Z; iz < this.Max.Z; iz++)
                    {
                        var i = new Vector3i(ix, iy, iz);
                        action(i);
                    }
                }
            }
        }

        public bool Overlaps(Rect3i that)
        {
            if (this.Size == Vector3i.Zero || that.Size == Vector3i.Zero)
                return false;

            bool overlapX = this.Min.X <= that.Max.X && that.Min.X <= this.Max.X;
            bool overlapY = this.Min.Y <= that.Max.Y && that.Min.Y <= this.Max.Y;
            bool overlapZ = this.Min.Z <= that.Max.Z && that.Min.Z <= this.Max.Z;
            return overlapX && overlapY && overlapZ;
        }

        public Rect3i CombineWith(Rect3i that)
        {
            if (this.Size == Vector3i.Zero)
                return that;

            if (that.Size == Vector3i.Zero)
                return this;

            var min = Vector3i.Min(this.Min, that.Min);
            var max = Vector3i.Max(this.Max, that.Max);
            return Rect3i.FromMinAndMax(min, max);
        }

        public Rect3f ToBounds()
        {
            return Rect3f.FromMinAndMax(Min.ToVector3f(), Max.ToVector3f());
        }

        public Rect3i IntersectWith(Rect3i that)
        {
            if (this.Size == Vector3i.Zero || that.Size == Vector3i.Zero)
                return Rect3i.Zero;

            var min = Vector3i.Max(this.Min, that.Min);
            var max = Vector3i.Max(Vector3i.Min(this.Max, that.Max), min);

            return Rect3i.FromMinAndMax(min, max);
        }

        public static bool operator ==(Rect3i a, Rect3i b)
        {
            return a.Min == b.Min && a.Max == b.Max;
        }

        public static bool operator !=(Rect3i a, Rect3i b)
        {
            return a.Min != b.Min || a.Max != b.Max;
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
            return Min.GetHashCode() + 7 * Max.GetHashCode();
        }

        public override string ToString()
        {
            return $"min:{Min} max:{Max} size:{Size}";
        }
    }
}