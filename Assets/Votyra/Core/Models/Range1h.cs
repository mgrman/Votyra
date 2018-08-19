using System;

namespace Votyra.Core.Models
{
    public struct Range1h : IEquatable<Range1h>
    {
        public static readonly Range1h Default = new Range1h(Height.Default, Height.Default);

        public readonly Height Min;
        public readonly Height Max;

        public Range1h(Height min, Height max)
        {
            this.Min = Height.Min(min, max);
            this.Max = Height.Max(min, max);
        }

        // public Height Center => (Max + Min) / 2.0f;

        // public int FlooredCenter => (Max + Min) / 2;

        public Height.Difference Size => Max - Min;

        // public void ForeachPointExlusive(Action<Height> action)
        // {
        //     for (Height i = this.Min; i < this.Max; i++)
        //     {
        //         action(i);
        //     }
        // }

        public Range1h UnionWith(Range1h range)
        {
            return new Range1h(Height.Min(this.Min, range.Min), Height.Min(this.Max, range.Max));
        }

        public Range1h? UnionWith(Range1h? range)
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

        // public static Range1i operator -(Range1i a, Range1i b)
        // {
        //     return new Range1i(a.Min - b.Min, a.Max - b.Max);
        // }

        public static bool operator ==(Range1h a, Range1h b)
        {
            return a.Min == b.Min && a.Max == b.Max;
        }

        public static bool operator !=(Range1h a, Range1h b)
        {
            return a.Min != b.Min || a.Max != b.Max;
        }

        public bool Equals(Range1h other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Range1h))
                return false;

            return this.Equals((Range1h)obj);
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