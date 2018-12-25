using System;

namespace Votyra.Core.Models
{
    public struct Area1f : IEquatable<Area1f>
    {
        public static readonly Area1f Zero = new Area1f(0, 0);
        public readonly float Min;
        public readonly float Max;

        public Area1f(float min, float max)
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

        public static bool operator ==(Area1f a, Area1f b)
        {
            return a.Min == b.Min && a.Max == b.Max;
        }

        public static bool operator !=(Area1f a, Area1f b)
        {
            return a.Min != b.Min || a.Max != b.Max;
        }

        public Area1f UnionWith(Area1f range)
        {
            return new Area1f(Math.Min(this.Min, range.Min), Math.Min(this.Max, range.Max));
        }

        public bool Equals(Area1f other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Area1f))
                return false;

            return this.Equals((Area1f)obj);
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
            return $"Area1i: min={Min} max={Max} size={Size}";
        }
    }
}