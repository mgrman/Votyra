using System;
using UnityEngine;

namespace Votyra.Core.Models
{
    public struct Height1i : IEquatable<Height1i>, IComparable<Height1i>
    {
        public static readonly Height1i Default = new Height1i();
        public static readonly Height1i MinValue = new Height1i(int.MinValue);
        public static readonly Height1i MaxValue = new Height1i(int.MaxValue);
        private readonly int Value;

        public Height1i(int value)
        {
            Value = value;
        }

        public Height1f ToHeight1f()
        {
            return new Height1f(Value);
        }

        public Height1i Above => new Height1i(Value + 1);
        public Height1i Below => new Height1i(Value - 1);
        public int RawValue => Value;
        public Height1i Abs => new Height1i(Math.Abs(Value));
        public int Sign => Math.Sign(Value);

        public static Height1i Max(Height1i a, Height1i b) => Math.Max(a.Value, b.Value).CreateHeight();

        public static Height1i Min(Height1i a, Height1i b) => Math.Min(a.Value, b.Value).CreateHeight();

        public static Difference operator -(Height1i a, Height1i b) => new Difference(a.Value - b.Value);

        public static Height1i operator -(Height1i a) => new Height1i(-a.Value);

        public static bool operator !=(Height1i a, Height1i b) => a.Value != b.Value;

        // }
        public static Height1i operator *(Height1i a, int b) => new Height1i(a.Value * b);

        public static bool operator <(Height1i a, Height1i b) => a.Value < b.Value;

        public static bool operator <=(Height1i a, Height1i b) => a == b || a < b;

        public static bool operator ==(Height1i a, Height1i b) => a.Value == b.Value;

        public static bool operator >(Height1i a, Height1i b) => a.Value > b.Value;

        public static bool operator >=(Height1i a, Height1i b) => a == b || a > b;

        public static Range1hi Range(Height1i min, Height1i max) => new Range1hi(min, max);

        public static Height1i Lerp(Height1i a, Height1i b, float param)
        {
            var offsetF = (a.Value - b.Value) * param;
            int offsetI = 0;
            if (offsetF > 0.1f)
                offsetI = Mathf.Max(1, Mathf.RoundToInt(offsetF));
            else if (offsetF < -0.1f)
                offsetI = Mathf.Min(-1, Mathf.RoundToInt(offsetF));
            return (b.Value + offsetI).CreateHeight();
        }

        public int CompareTo(Height1i other) => this.Value.CompareTo(other.Value);

        public bool Equals(Height1i other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Height1i))
                return false;

            return this.Equals((Height1i)obj);
        }

        public override int GetHashCode()
        {
            return Value;
        }

        // public static Height operator +(Height a, Height b)
        // {
        public Height1i LowerClip(Height1i height, Height1i min)
        {
            return Math.Max(this.Value - height.Value, min.Value).CreateHeight();
        }

        public override string ToString()
        {
            return Value.ToString().PadLeft(4) + " [h]";
        }

        public Vector3f ToVector3f(Vector2f vec)
        {
            return new Vector3f(vec.X, vec.Y, Value);
        }

        public Vector3f ToVector3f(Vector2i vec)
        {
            return new Vector3f(vec.X, vec.Y, Value);
        }

        public struct Difference
        {
            public static readonly Difference Zero = new Difference();
            public static readonly Difference MinValue = new Difference(int.MinValue);
            public static readonly Difference MaxValue = new Difference(int.MaxValue);
            private readonly int Value;

            public Difference(int value)
            {
                Value = value;
            }

            public Difference Abs => new Difference(Math.Abs(Value));

            public static Difference AbsoluteDif(Height1i a, Height1i b) => new Difference(Math.Abs(a.Value - b.Value));

            public static Height1i operator -(Height1i a, Difference b) => new Height1i(a.Value - b.Value);

            public static Difference operator *(Difference a, int b) => new Difference(a.Value * b);

            public static Difference operator *(Difference a, float b) => new Difference((int)(a.Value * b));

            public static Height1i operator +(Height1i a, Difference b) => new Height1i(a.Value + b.Value);
            public static Height1i operator +(Difference a, Height1i b) => new Height1i(a.Value + b.Value);

            public static Difference operator +(Difference a, Difference b) => new Difference(a.Value + b.Value);

            public static bool operator <(Difference a, Difference b) => a.Value < b.Value;

            public static bool operator >(Difference a, Difference b) => a.Value > b.Value;

            public override string ToString()
            {
                return Value.ToString().PadLeft(4) + " [Δh]";
            }

            public Vector3f ToVector3f(Vector2f vec) => new Vector3f(vec.X, vec.Y, Value);
        }
    }

    public static class HeightUtils
    {
        public static Height1i CreateHeight(this int val)
        {
            return new Height1i(val);
        }

        public static Height1i.Difference CreateHeightDifference(this int val)
        {
            return new Height1i.Difference(val);
        }

        public static Vector3f ToVector3f(this Vector2i vec, Height1i z)
        {
            return z.ToVector3f(vec);
        }

        public static Vector3f ToVector3f(this Vector2f vec, Height1i z)
        {
            return z.ToVector3f(vec);
        }

        public static Vector3f ToVector3f(this Vector2f vec, Height1i.Difference z)
        {
            return z.ToVector3f(vec);
        }
    }
}