using System;

namespace Votyra.Core.Models
{
    public struct Range2f : IEquatable<Range2f>
    {
        public static readonly Range2f Zero = new Range2f(0, 0);
        public readonly float min;
        public readonly float max;

        public Range2f(float min, float max)
        {
            this.min = Math.Min(min, max);
            this.max = Math.Max(min, max);
        }

        public float Center
        {
            get
            {
                return (max + min) / 2;
            }
        }

        public float Size
        {
            get
            {
                return max - min;
            }
        }

        public Range2f UnionWith(Range2f range)
        {
            return new Range2f(Math.Min(this.min, range.min), Math.Min(this.max, range.max));
        }

        public static bool operator ==(Range2f a, Range2f b)
        {
            return a.min == b.min && a.max == b.max;
        }

        public static bool operator !=(Range2f a, Range2f b)
        {
            return a.min != b.min || a.max != b.max;
        }

        public bool Equals(Range2f other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Range2f))
                return false;

            return this.Equals((Range2f)obj);
        }

        public override int GetHashCode()
        {
            return min.GetHashCode() + max.GetHashCode() * 7;
        }

        public override string ToString()
        {
            return string.Format("({0} , {1})", min, max);
        }
    }
}