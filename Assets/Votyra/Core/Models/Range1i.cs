using System;

namespace Votyra.Core.Models
{
    public struct Range1i : IEquatable<Range1i>
    {
        public static readonly Range1i Zero = new Range1i(0, 0);
        public static readonly Range1i PlusMinusOne = new Range1i(-1, 1);

        public readonly int Min;
        public readonly int Max;

        public Range1i(int min, int max)
        {
            Min = Math.Min(min, max);
            Max = Math.Max(min, max);
        }

        public Range1i(Area1f range)
            : this((int) Math.Floor(range.Min), (int) Math.Floor(range.Max))
        {
        }

        public float Center => (Max + Min) / 2.0f;

        public int FlooredCenter => (Max + Min) / 2;

        public int Size => Max - Min;

        public static Range1i operator +(Range1i a, Range1i b) => new Range1i(a.Min + b.Min, a.Max + b.Max);

        public static Range1i operator -(Range1i a, Range1i b) => new Range1i(a.Min - b.Min, a.Max - b.Max);

        public static bool operator ==(Range1i a, Range1i b) => a.Min == b.Min && a.Max == b.Max;

        public static bool operator !=(Range1i a, Range1i b) => a.Min != b.Min || a.Max != b.Max;

        public void ForeachPointExlusive(Action<int> action)
        {
            for (var i = Min; i < Max; i++)
            {
                action(i);
            }
        }

        public Range1i UnionWith(Range1i range) => new Range1i(Math.Min(Min, range.Min), Math.Min(Max, range.Max));

        public Range1i? UnionWith(Range1i? range)
        {
            if (range == null)
                return this;
            return UnionWith(range.Value);
        }

        public bool Equals(Range1i other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Range1i))
                return false;

            return Equals((Range1i) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Min.GetHashCode() + Max.GetHashCode() * 7;
            }
        }

        public override string ToString() => string.Format("({0} , {1})", Min, Max);
    }
}