using System;

namespace Votyra.Core.Models
{
    public struct Range1hf : IEquatable<Range1hf>
    {
        public static readonly Range1hf Default = new Range1hf(Height1f.Default, Height1f.Default);

        public readonly Height1f Min;
        public readonly Height1f Max;

        public Range1hf(Height1f min, Height1f max)
        {
            this.Min = Height1f.Min(min, max);
            this.Max = Height1f.Max(min, max);
        }

        // public Height Center => (Max + Min) / 2.0f;

        // public int FlooredCenter => (Max + Min) / 2;

        public Height1f.Difference Size => Max - Min;

        // public void ForeachPointExlusive(Action<Height> action)
        // {
        //     for (Height i = this.Min; i < this.Max; i++)
        //     {
        //         action(i);
        //     }
        // }

        public static bool operator ==(Range1hf a, Range1hf b)
        {
            return a.Min == b.Min && a.Max == b.Max;
        }

        // public static Range1i operator -(Range1i a, Range1i b)
        // {
        //     return new Range1i(a.Min - b.Min, a.Max - b.Max);
        // }
        public static bool operator !=(Range1hf a, Range1hf b)
        {
            return a.Min != b.Min || a.Max != b.Max;
        }

        public Range1hf UnionWith(Range1hf range)
        {
            return new Range1hf(Height1f.Min(this.Min, range.Min), Height1f.Min(this.Max, range.Max));
        }

        public Range1hf? UnionWith(Range1hf? range)
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
        public bool Equals(Range1hf other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Range1hf))
                return false;

            return this.Equals((Range1hf)obj);
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