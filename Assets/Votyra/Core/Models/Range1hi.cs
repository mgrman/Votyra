using System;

namespace Votyra.Core.Models
{
    public struct Range1hi : IEquatable<Range1hi>
    {
        public static readonly Range1hi Default = new Range1hi(Height1i.Default, Height1i.Default);

        public readonly Height1i Min;
        public readonly Height1i Max;

        public Range1hi(Height1i min, Height1i max)
        {
            this.Min = Height1i.Min(min, max);
            this.Max = Height1i.Max(min, max);
        }

        public Range1hf ToRange1hf()
        {
            return new Range1hf(Min.ToHeight1f(),Max.ToHeight1f());
        }

        // public Height Center => (Max + Min) / 2.0f;

        // public int FlooredCenter => (Max + Min) / 2;

        public Height1i.Difference Size => Max - Min;

        // public void ForeachPointExlusive(Action<Height> action)
        // {
        //     for (Height i = this.Min; i < this.Max; i++)
        //     {
        //         action(i);
        //     }
        // }

        public static bool operator ==(Range1hi a, Range1hi b)
        {
            return a.Min == b.Min && a.Max == b.Max;
        }

        // public static Range1i operator -(Range1i a, Range1i b)
        // {
        //     return new Range1i(a.Min - b.Min, a.Max - b.Max);
        // }
        public static bool operator !=(Range1hi a, Range1hi b)
        {
            return a.Min != b.Min || a.Max != b.Max;
        }

        public Range1hi UnionWith(Range1hi range)
        {
            return new Range1hi(Height1i.Min(this.Min, range.Min), Height1i.Min(this.Max, range.Max));
        }

        public Range1hi UnionWith(Height1i value)
        {
            if (value < Min)
            {
                return new Range1hi(value, Max);
            }
            else if (value > Max)
            {
                return new Range1hi(Min, value);
            }
            else
            {
                return this;
            }
        }

        public Range1hi? UnionWith(Range1hi? range)
        {
            if (range == null)
            {
                return this;
            }
            return UnionWith(range.Value);
        }

        // public static Range1i operator +(Range1i a, Range1i b)
        // {
        //     return new Range1i(a.Min + b.Min, a.Max + b.Max);
        // }
        public bool Equals(Range1hi other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Range1hi))
                return false;

            return this.Equals((Range1hi)obj);
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