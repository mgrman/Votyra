using System;
using UnityEngine;

namespace Votyra.Core.Models
{
    public struct Height : IEquatable<Height>, IComparable<Height>
    {
        public static readonly Height Default = new Height();
        public static readonly Height Hole = new Height(HoleValue);
        private const int HoleValue = int.MinValue;
        private readonly int Value;

        public Height(int value)
        {
            Value = value;
        }

        public Height Above => IsHole ? Hole : new Height(Value + 1);
        public Height Below => IsHole ? Hole : new Height(Value - 1);
        public bool IsHole => Value == HoleValue;
        public bool IsNotHole => Value != HoleValue;
        public Height Abs => IsHole ? Hole : new Height(Math.Abs(Value));
        public int Sign => IsHole ? 0 : Math.Sign(Value);

        public static Height Max(Height a, Height b)
        {
            return (a.IsNotHole && b.IsNotHole) ? Math.Max(a.Value, b.Value).CreateHeight() : Height.Hole;
        }

        public static Height MaxHoleless(Height a, Height b)
        {
            return (a.IsNotHole && b.IsNotHole) ? Math.Max(a.Value, b.Value).CreateHeight() : a.DefaultIfHole(b);
        }

        public static Height Min(Height a, Height b)
        {
            return (a.IsNotHole && b.IsNotHole) ? Math.Min(a.Value, b.Value).CreateHeight() : Height.Hole;
        }

        public static Height MinHoleless(Height a, Height b)
        {
            return (a.IsNotHole && b.IsNotHole) ? Math.Min(a.Value, b.Value).CreateHeight() : a.DefaultIfHole(b);
        }

        public static Difference operator -(Height a, Height b)
        {
            if (a.IsNotHole && b.IsNotHole)
            {
                return new Difference(a.Value - b.Value);
            }
            else
            {
                return Difference.Hole;
            }
        }

        public static Height operator -(Height a)
        {
            if (a.IsNotHole)
            {
                return new Height(-a.Value);
            }
            else
            {
                return Height.Hole;
            }
        }

        public static bool operator !=(Height a, Height b)
        {
            return a.Value != b.Value;
        }

        // }
        public static Height operator *(Height a, int b)
        {
            if (a.IsNotHole)
            {
                return new Height(a.Value * b);
            }
            else
            {
                return Height.Hole;
            }
        }

        public static bool operator <(Height a, Height b)
        {
            if (a.IsNotHole && b.IsNotHole)
            {
                return a.Value < b.Value;
            }
            else
            {
                return false;
            }
        }

        public static bool operator <=(Height a, Height b)
        {
            return a == b || a < b;
        }

        public static bool operator ==(Height a, Height b)
        {
            return a.Value == b.Value;
        }

        public static bool operator >(Height a, Height b)
        {
            if (a.IsNotHole && b.IsNotHole)
            {
                return a.Value > b.Value;
            }
            else
            {
                return false;
            }
        }

        public static bool operator >=(Height a, Height b)
        {
            return a == b || a > b;
        }

        public static Range1h? Range(Height min, Height max)
        {
            if (min.IsNotHole && max.IsNotHole)
            {
                return new Range1h(min, max);
            }
            else
            {
                return null;
            }
        }

        public int CompareTo(Height other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public Height DefaultIfHole(Height b)
        {
            return this.IsNotHole ? this : b;
        }

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
            return (this.IsNotHole && height.IsNotHole && min.IsNotHole) ? Math.Max(this.Value - height.Value, min.Value).CreateHeight() : this;
        }

        public override string ToString()
        {
            if (IsHole)
                return " H";
            else
                return Value.ToString().PadLeft(2);
        }

        public Vector3f? ToVector3f(Vector2f vec)
        {
            if (IsHole)
                return null;
            else
                return new Vector3f(vec.X, vec.Y, Value);
        }

        public Vector3f ToVector3f(Vector2f vec, float holeValue)
        {
            return new Vector3f(vec.X, vec.Y, IsNotHole ? Value : holeValue);
        }

        public static Height Lerp(Height a, Height b, float param)
        {
            if (a.IsHole || b.IsHole)
            {
                return Height.Hole;
            }

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
            public static readonly Difference Hole = new Difference(HoleValue);
            public static readonly Difference Infinite = new Difference(InfiniteValue);
            public static readonly Difference Zero = new Difference();
            private const int HoleValue = int.MinValue;
            private const int InfiniteValue = int.MaxValue;
            private readonly int Value;

            public Difference(int value)
            {
                Value = value;
            }

            public bool IsHole => Value == HoleValue;
            public bool IsInfinite => Value == InfiniteValue;
            public bool IsNotHole => Value != HoleValue;
            public bool IsNotInfinite => Value != InfiniteValue;
            public Difference Abs => IsHole ? Hole : IsHole || IsInfinite || Value > 0 ? this : new Difference(Math.Abs(Value));

            public static Difference AbsoluteDif(Height a, Height b)
            {
                if (a.IsHole && b.IsHole)
                    return Zero;
                else if (a.IsHole || b.IsHole)
                    return Difference.Infinite;
                else
                    return new Difference(Math.Abs(a.Value - b.Value));
            }

            public static Height operator -(Height a, Difference b)
            {
                if (b.IsInfinite)
                {
                    throw new NotImplementedException("Cannot add infinite difference to height!");
                }
                else if (a.IsNotHole && b.IsNotHole)
                {
                    return new Height(a.Value - b.Value);
                }
                else
                {
                    return a;
                }
            }

            public static Difference operator *(Difference a, int b)
            {
                if (a.IsInfinite || a.IsHole)
                {
                    return a;
                }
                else
                {
                    return new Difference(a.Value * b);
                }
            }

            public static Difference operator *(Difference a, float b)
            {
                if (a.IsInfinite || a.IsHole)
                {
                    return a;
                }
                else
                {
                    return new Difference((int)(a.Value * b));
                }
            }

            public static Height operator +(Height a, Difference b)
            {
                if (b.IsInfinite)
                {
                    throw new NotImplementedException("Cannot add infinite difference to height!");
                }
                else if (a.IsNotHole && b.IsNotHole)
                {
                    return new Height(a.Value + b.Value);
                }
                else
                {
                    return a;
                }
            }

            public static Difference operator +(Difference a, Difference b)
            {
                if (a.IsInfinite || b.IsInfinite)
                {
                    return Difference.Infinite;
                }
                else if (a.IsNotHole && b.IsNotHole)
                {
                    return new Difference(a.Value + b.Value);
                }
                else
                {
                    return a.DefaultIfHole(b);
                }
            }

            public static bool operator <(Difference a, Difference b)
            {
                if (a.IsNotInfinite && b.IsInfinite)
                {
                    return true;
                }
                else if (a.IsNotHole && b.IsNotHole)
                {
                    return a.Value < b.Value;
                }
                else
                {
                    return false;
                }
            }

            public static bool operator >(Difference a, Difference b)
            {
                if (a.IsInfinite && b.IsNotInfinite)
                {
                    return true;
                }
                else if (a.IsNotHole && b.IsNotHole)
                {
                    return a.Value > b.Value;
                }
                else
                {
                    return false;
                }
            }

            public Difference DefaultIfHole(Difference b)
            {
                return this.IsNotHole ? this : b;
            }

            public override string ToString()
            {
                if (IsInfinite)
                    return " I";
                else if (IsHole)
                    return " H";
                else
                    return Value.ToString().PadLeft(2);
            }

            public Vector3f? ToVector3f(Vector2f vec)
            {
                if (IsHole)
                    return null;
                else
                    return new Vector3f(vec.X, vec.Y, Value);
            }

            public Vector3f ToVector3f(Vector2f vec, float holeValue)
            {
                return new Vector3f(vec.X, vec.Y, IsHole ? Value : holeValue);
            }
        }
    }

    public static class HeightUtils
    {
        public static Height CreateHeight(this int val)
        {
            return new Height(val);
        }

        public static Height CreateHeight(this int? val)
        {
            return val.HasValue ? new Height(val.Value) : Height.Hole;
        }

        public static Height.Difference CreateHeightDifference(this int val)
        {
            return new Height.Difference(val);
        }

        public static Height.Difference CreateHeightDifference(this int? val)
        {
            return val.HasValue ? new Height.Difference(val.Value) : Height.Difference.Hole;
        }

        public static Vector3f ToVector3f(this Vector2f vec, Height z, float holeValue)
        {
            return z.ToVector3f(vec, holeValue);
        }

        public static Vector3f? ToVector3f(this Vector2f vec, Height z)
        {
            return z.ToVector3f(vec);
        }

        public static Vector3f ToVector3f(this Vector2f vec, Height.Difference z, float holeValue)
        {
            return z.ToVector3f(vec, holeValue);
        }

        public static Vector3f? ToVector3f(this Vector2f vec, Height.Difference z)
        {
            return z.ToVector3f(vec);
        }
    }
}