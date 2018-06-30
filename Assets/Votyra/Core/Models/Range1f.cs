using System;

namespace Votyra.Core.Models
{
    public struct Range1f : IEquatable<Range1f>
    {
        public static readonly Range1f Zero = new Range1f(0, 0);
        public readonly float Min;
        public readonly float Max;

        public Range1f(float min, float max)
        {
            this.Min = Math.Min(min, max);
            this.Max = Math.Max(min, max);
        }

        public float Center
        {
            get
            {
                return (Max + Min) / 2;
            }
        }

        public float Size
        {
            get
            {
                return Max - Min;
            }
        }

        public Range1f UnionWith(Range1f range)
        {
            return new Range1f(Math.Min(this.Min, range.Min), Math.Min(this.Max, range.Max));
        }

        public static bool operator ==(Range1f a, Range1f b)
        {
            return a.Min == b.Min && a.Max == b.Max;
        }

        public static bool operator !=(Range1f a, Range1f b)
        {
            return a.Min != b.Min || a.Max != b.Max;
        }

        public bool Equals(Range1f other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Range1f))
                return false;

            return this.Equals((Range1f)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Min.GetHashCode() + Max.GetHashCode() * 7;
            }
        }

        public override string ToString()
        {
            return string.Format("({0} , {1})", Min, Max);
        }
    }
}