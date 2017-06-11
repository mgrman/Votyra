using System;

namespace Votyra.Models
{
    public struct Range2 : IEquatable<Range2>
    {
        public readonly float min;
        public readonly float max;

        public Range2(float min, float max)
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

        public Range2 UnionWith(Range2 range)
        {
            return new Range2(Math.Min(this.min, range.min), Math.Min(this.max, range.max));
        }

        public static bool operator ==(Range2 a, Range2 b)
        {
            return a.min == b.min && a.max == b.max;
        }

        public static bool operator !=(Range2 a, Range2 b)
        {
            return a.min != b.min || a.max != b.max;
        }

        public bool Equals(Range2 other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Range2))
                return false;

            return this.Equals((Range2)obj);
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