using System;

namespace Votyra.Core.Models
{
    public struct Range1i : IEquatable<Range1i>
    {
        public static readonly Range1i Zero = new Range1i(0, 0);
        public readonly int Min;
        public readonly int Max;

        public Range1i(int min, int max)
        {
            this.Min = Math.Min(min, max);
            this.Max = Math.Max(min, max);
        }

        public Range1i(Range1f range)
            : this((int)Math.Floor(range.Min), (int)Math.Floor(range.Max))
        {
        }

        public float Center => (Max + Min) / 2.0f;

        public int FlooredCenter => (Max + Min) / 2;

        public int Size => Max - Min;

        public void ForeachPointExlusive(Action<int> action)
        {
            for (int i = this.Min; i < this.Max; i++)
            {
                action(i);
            }
        }

        public Range1i UnionWith(Range1i range)
        {
            return new Range1i(Math.Min(this.Min, range.Min), Math.Min(this.Max, range.Max));
        }

        public static Range1i operator +(Range1i a, Range1i b)
        {
            return new Range1i(a.Min + b.Min, a.Max + b.Max);
        }

        public static Range1i operator -(Range1i a, Range1i b)
        {
            return new Range1i(a.Min - b.Min, a.Max - b.Max);
        }

        public static bool operator ==(Range1i a, Range1i b)
        {
            return a.Min == b.Min && a.Max == b.Max;
        }

        public static bool operator !=(Range1i a, Range1i b)
        {
            return a.Min != b.Min || a.Max != b.Max;
        }

        public bool Equals(Range1i other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Range1i))
                return false;

            return this.Equals((Range1i)obj);
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