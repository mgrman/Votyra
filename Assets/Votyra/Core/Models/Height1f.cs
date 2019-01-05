using System;
using UnityEngine;

namespace Votyra.Core.Models
{
    public struct Height1f : IEquatable<Height1f>, IComparable<Height1f>
    {
        public static readonly Height1f Default = new Height1f();
        public static readonly Height1f MinValue = new Height1f(int.MinValue);
        public static readonly Height1f MaxValue = new Height1f(int.MaxValue);
        private readonly float Value;

        public Height1f(float value)
        {
            Value = value;
        }

        public float RawValue => Value;
        public Height1f Abs => new Height1f(Math.Abs(Value));
        public int Sign => Math.Sign(Value);

        public static Height1f Max(Height1f a, Height1f b) => Math.Max(a.Value, b.Value).CreateHeight1f();

        public static Height1f Min(Height1f a, Height1f b) => Math.Min(a.Value, b.Value).CreateHeight1f();

        public static Difference operator -(Height1f a, Height1f b) => new Difference(a.Value - b.Value);

        public static Height1f operator -(Height1f a) => new Height1f(-a.Value);

        public static bool operator !=(Height1f a, Height1f b) => a.Value != b.Value;

        // }
        public static Height1f operator *(Height1f a, int b) => new Height1f(a.Value * b);

        public static bool operator <(Height1f a, Height1f b) => a.Value < b.Value;

        public static bool operator <=(Height1f a, Height1f b) => a == b || a < b;

        public static bool operator ==(Height1f a, Height1f b) => a.Value == b.Value;

        public static bool operator >(Height1f a, Height1f b) => a.Value > b.Value;

        public static bool operator >=(Height1f a, Height1f b) => a == b || a > b;

        public static Range1hf Range(Height1f min, Height1f max) => new Range1hf(min, max);

        public static Height1f Lerp(Height1f a, Height1f b, float param)
        {
            var offsetF = (a.Value - b.Value) * param;
            // int offsetI = 0;
            // if (offsetF > 0.1f)
            //     offsetI = Mathf.Max(1, Mathf.RoundToInt(offsetF));
            // else if (offsetF < -0.1f)
            //     offsetI = Mathf.Min(-1, Mathf.RoundToInt(offsetF));
            return (b.Value + offsetF).CreateHeight1f();
        }

        public int CompareTo(Height1f other) => this.Value.CompareTo(other.Value);

        public bool Equals(Height1f other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Height1f))
                return false;

            return this.Equals((Height1f)obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        // public static Height1f operator +(Height1f a, Height1f b)
        // {
        public Height1f LowerClip(Height1f Height1f, Height1f min)
        {
            return Math.Max(this.Value - Height1f.Value, min.Value).CreateHeight1f();
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
            private readonly float Value;

            public Difference(float value)
            {
                Value = value;
            }

            public Difference Abs => new Difference(Math.Abs(Value));

            public static Difference AbsoluteDif(Height1f a, Height1f b) => new Difference(Math.Abs(a.Value - b.Value));

            public static Height1f operator -(Height1f a, Difference b) => new Height1f(a.Value - b.Value);

            public static Difference operator *(Difference a, int b) => new Difference(a.Value * b);

            public static Difference operator *(Difference a, float b) => new Difference((int)(a.Value * b));

            public static Height1f operator +(Height1f a, Difference b) => new Height1f(a.Value + b.Value);
            public static Height1f operator +(Difference a, Height1f b) => new Height1f(a.Value + b.Value);

            public static Difference operator +(Difference a, Difference b) => new Difference(a.Value + b.Value);

            public static bool operator <(Difference a, Difference b) => a.Value < b.Value;

            public static bool operator >(Difference a, Difference b) => a.Value > b.Value;

            public override string ToString()
            {
                return Value.ToString().PadLeft(4) + " [Δh]";
            }

            public Vector3f ToVector3f(Vector2i vec) => new Vector3f(vec.X, vec.Y, Value);
            public Vector3f ToVector3f(Vector2f vec) => new Vector3f(vec.X, vec.Y, Value);
        }
    }

    public static class Height1fUtils
    {
        public static Height1f CreateHeight1f(this float val)
        {
            return new Height1f(val);
        }

        public static Height1f.Difference CreateHeight1fDifference(this float val)
        {
            return new Height1f.Difference(val);
        }

        public static Vector3f ToVector3f(this Vector2i vec, Height1f z)
        {
            return z.ToVector3f(vec);
        }

        public static Vector3f ToVector3f(this Vector2f vec, Height1f z)
        {
            return z.ToVector3f(vec);
        }

        public static Vector3f ToVector3f(this Vector2i vec, Height1f.Difference z)
        {
            return z.ToVector3f(vec);
        }

        public static Vector3f ToVector3f(this Vector2f vec, Height1f.Difference z)
        {
            return z.ToVector3f(vec);
        }
    }
}