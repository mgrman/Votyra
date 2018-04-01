using System;

namespace Votyra.Core.Models
{
    public struct Range2i : IEquatable<Range2i>
    {
        public static readonly Range2i Zero = new Range2i(0, 0);
        public readonly int Min;
        public readonly int Max;

        public Range2i(int min, int max)
        {
            this.Min = Math.Min(min, max);
            this.Max = Math.Max(min, max);
        }

        public Range2i(Range2f range)
            : this((int)Math.Floor(range.min), (int)Math.Floor(range.max))
        {
        }

        public float Center => (Max + Min) / 2.0f;

        public int FlooredCenter => (Max + Min) / 2;

        public int Size => Max - Min;

        public Range2i UnionWith(Range2i range)
        {
            return new Range2i(Math.Min(this.Min, range.Min), Math.Min(this.Max, range.Max));
        }

        public static Range2i operator +(Range2i a, Range2i b)
        {
            return new Range2i(a.Min + b.Min, a.Max + b.Max);
        }

        public static Range2i operator -(Range2i a, Range2i b)
        {
            return new Range2i(a.Min - b.Min, a.Max - b.Max);
        }

        public static bool operator ==(Range2i a, Range2i b)
        {
            return a.Min == b.Min && a.Max == b.Max;
        }

        public static bool operator !=(Range2i a, Range2i b)
        {
            return a.Min != b.Min || a.Max != b.Max;
        }

        public bool Equals(Range2i other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Range2i))
                return false;

            return this.Equals((Range2i)obj);
        }

        public override int GetHashCode()
        {
            return Min.GetHashCode() + Max.GetHashCode() * 7;
        }

        public override string ToString()
        {
            return string.Format("({0} , {1})", Min, Max);
        }
    }
}