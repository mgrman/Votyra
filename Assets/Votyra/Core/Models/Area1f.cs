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
            Min = Math.Min(min, max);
            Max = Math.Max(min, max);
        }

        public float Center => (Max + Min) / 2;

        public float Size => Max - Min;

        public static bool operator ==(Area1f a, Area1f b) => a.Min == b.Min && a.Max == b.Max;

        public static bool operator !=(Area1f a, Area1f b) => a.Min != b.Min || a.Max != b.Max;

        public Area1f UnionWith(Area1f range) => new Area1f(Math.Min(Min, range.Min), Math.Min(Max, range.Max));

        public Area1f? UnionWith(Area1f? range)
        {
            if (range == null)
                return this;

            return UnionWith(range.Value);
        }

        public Area1f UnionWith(float value)
        {
            if (value < Min)
                return new Area1f(value, Max);
            if (value > Max)
                return new Area1f(Min, value);
            return this;
        }

        public bool Equals(Area1f other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Area1f))
                return false;

            return Equals((Area1f) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Min.GetHashCode() + Max.GetHashCode() * 7;
            }
        }

        public override string ToString() => $"Area1i: min={Min} max={Max} size={Size}";
    }
}