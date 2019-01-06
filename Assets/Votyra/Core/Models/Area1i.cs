using System;

namespace Votyra.Core.Models
{
    public struct Area1i : IEquatable<Area1i>
    {
        public static readonly Area1i Zero = new Area1i(0, 0);
        public static readonly Area1i PlusMinusOne = new Area1i(-1, 1);

        public readonly int Min;
        public readonly int Max;

        public Area1i(int min, int max)
        {
            Min = Math.Min(min, max);
            Max = Math.Max(min, max);
        }

        public Area1i(Area1f range)
            : this((int) Math.Floor(range.Min), (int) Math.Floor(range.Max))
        {
        }

        public float Center => (Max + Min) / 2.0f;

        public int FlooredCenter => (Max + Min) / 2;

        public int Size => Max - Min + 1;

        public static Area1i operator +(Area1i a, Area1i b) => new Area1i(a.Min + b.Min, a.Max + b.Max);

        public static Area1i operator -(Area1i a, Area1i b) => new Area1i(a.Min - b.Min, a.Max - b.Max);

        public static bool operator ==(Area1i a, Area1i b) => a.Min == b.Min && a.Max == b.Max;

        public static bool operator !=(Area1i a, Area1i b) => a.Min != b.Min || a.Max != b.Max;

        public void ForeachPointInclusive(Action<int> action)
        {
            for (var i = Min; i <= Max; i++)
            {
                action(i);
            }
        }

        public Area1i UnionWith(Area1i range) => new Area1i(Math.Min(Min, range.Min), Math.Min(Max, range.Max));

        public Area1i? UnionWith(Area1i? range)
        {
            if (range == null)
                return this;
            return UnionWith(range.Value);
        }

        public bool Equals(Area1i other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Area1i))
                return false;

            return Equals((Area1i) obj);
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