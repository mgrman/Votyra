using System;
using UnityEngine;

namespace Votyra.Core.Models
{
    public struct Height : IEquatable<Height>, IComparable<Height>
    {
        public static readonly Height Default = new Height();
        public static readonly Height MinValue = new Height(int.MinValue);
        public static readonly Height MaxValue = new Height(int.MaxValue);
        private readonly int Value;

        public Height(int value)
        {
            Value = value;
        }

        public Height Above => new Height(Value + 1);
        public Height Below => new Height(Value - 1);
        public Height Abs => new Height(Math.Abs(Value));
        public int Sign => Math.Sign(Value);
        public static Height Max(Height a, Height b) => Math.Max(a.Value, b.Value).CreateHeight();
        public static Height Min(Height a, Height b) => Math.Min(a.Value, b.Value).CreateHeight();
        public static Difference operator -(Height a, Height b) => new Difference(a.Value - b.Value);
        public static Height operator -(Height a) => new Height(-a.Value);

        public static bool operator !=(Height a, Height b) => a.Value != b.Value;

        // }
        public static Height operator *(Height a, int b) => new Height(a.Value * b);

        public static bool operator <(Height a, Height b) => a.Value < b.Value;

        public static bool operator <=(Height a, Height b) => a == b || a < b;

        public static bool operator ==(Height a, Height b) => a.Value == b.Value;

        public static bool operator >(Height a, Height b) => a.Value > b.Value;

        public static bool operator >=(Height a, Height b) => a == b || a > b;

        public static Range1h Range(Height min, Height max) => new Range1h(min, max);

        public int CompareTo(Height other) => this.Value.CompareTo(other.Value);

        public bool Equals(Height other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Height))
                return false;

            return this.Equals((Height)obj);
        }

        public override int GetHashCode()
        {
            return Value;
        }

        // public static Height operator +(Height a, Height b)
        // {
        public Height LowerClip(Height height, Height min)
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

        public static Height Lerp(Height a, Height b, float param)
        {
            var offsetF = (a.Value - b.Value) * param;
            int offsetI = 0;
            if (offsetF > 0.1f)
                offsetI = Mathf.Max(1, Mathf.RoundToInt(offsetF));
            else if (offsetF < -0.1f)
                offsetI = Mathf.Min(-1, Mathf.RoundToInt(offsetF));
            return (b.Value + offsetI).CreateHeight();
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

            public static Difference AbsoluteDif(Height a, Height b) => new Difference(Math.Abs(a.Value - b.Value));

            public static Height operator -(Height a, Difference b) => new Height(a.Value - b.Value);

            public static Difference operator *(Difference a, int b) => new Difference(a.Value * b);

            public static Difference operator *(Difference a, float b) => new Difference((int)(a.Value * b));

            public static Height operator +(Height a, Difference b) => new Height(a.Value + b.Value);

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
        public static Height CreateHeight(this int val)
        {
            return new Height(val);
        }

        public static Height.Difference CreateHeightDifference(this int val)
        {
            return new Height.Difference(val);
        }

        public static Vector3f ToVector3f(this Vector2f vec, Height z)
        {
            return z.ToVector3f(vec);
        }

        public static Vector3f ToVector3f(this Vector2f vec, Height.Difference z)
        {
            return z.ToVector3f(vec);
        }
    }
}