// using System;
// using UnityEngine;

// namespace Votyra.Core.Models
// {
//     public struct HeightHoleable : IEquatable<HeightHoleable>, IComparable<HeightHoleable>
//     {
//         public static readonly HeightHoleable Default = new HeightHoleable();
//         public static readonly HeightHoleable Hole = new HeightHoleable(HoleValue);
//         private const int HoleValue = int.MinValue;
//         private readonly int Value;

//         public HeightHoleable(int value)
//         {
//             Value = value;
//         }

//         public HeightHoleable Above => IsHole ? Hole : new HeightHoleable(Value + 1);
//         public HeightHoleable Below => IsHole ? Hole : new HeightHoleable(Value - 1);
//         public bool IsHole => Value == HoleValue;
//         public bool IsNotHole => Value != HoleValue;
//         public HeightHoleable Abs => IsHole ? Hole : new HeightHoleable(Math.Abs(Value));
//         public int Sign => IsHole ? 0 : Math.Sign(Value);

//         public static HeightHoleable Max(HeightHoleable a, HeightHoleable b)
//         {
//             return (a.IsNotHole && b.IsNotHole) ? Math.Max(a.Value, b.Value).CreateHeight() : HeightHoleable.Hole;
//         }

//         public static HeightHoleable MaxHoleless(HeightHoleable a, HeightHoleable b)
//         {
//             return (a.IsNotHole && b.IsNotHole) ? Math.Max(a.Value, b.Value).CreateHeight() : a.DefaultIfHole(b);
//         }

//         public static HeightHoleable Min(HeightHoleable a, HeightHoleable b)
//         {
//             return (a.IsNotHole && b.IsNotHole) ? Math.Min(a.Value, b.Value).CreateHeight() : HeightHoleable.Hole;
//         }

//         public static HeightHoleable MinHoleless(HeightHoleable a, HeightHoleable b)
//         {
//             return (a.IsNotHole && b.IsNotHole) ? Math.Min(a.Value, b.Value).CreateHeight() : a.DefaultIfHole(b);
//         }

//         public static Difference operator -(HeightHoleable a, HeightHoleable b)
//         {
//             if (a.IsNotHole && b.IsNotHole)
//             {
//                 return new Difference(a.Value - b.Value);
//             }
//             else
//             {
//                 return Difference.Hole;
//             }
//         }

//         public static HeightHoleable operator -(HeightHoleable a)
//         {
//             if (a.IsNotHole)
//             {
//                 return new HeightHoleable(-a.Value);
//             }
//             else
//             {
//                 return HeightHoleable.Hole;
//             }
//         }

//         public static bool operator !=(HeightHoleable a, HeightHoleable b)
//         {
//             return a.Value != b.Value;
//         }

//         // }
//         public static HeightHoleable operator *(HeightHoleable a, int b)
//         {
//             if (a.IsNotHole)
//             {
//                 return new HeightHoleable(a.Value * b);
//             }
//             else
//             {
//                 return HeightHoleable.Hole;
//             }
//         }

//         public static bool operator <(HeightHoleable a, HeightHoleable b)
//         {
//             if (a.IsNotHole && b.IsNotHole)
//             {
//                 return a.Value < b.Value;
//             }
//             else
//             {
//                 return false;
//             }
//         }

//         public static bool operator <=(HeightHoleable a, HeightHoleable b)
//         {
//             return a == b || a < b;
//         }

//         public static bool operator ==(HeightHoleable a, HeightHoleable b)
//         {
//             return a.Value == b.Value;
//         }

//         public static bool operator >(HeightHoleable a, HeightHoleable b)
//         {
//             if (a.IsNotHole && b.IsNotHole)
//             {
//                 return a.Value > b.Value;
//             }
//             else
//             {
//                 return false;
//             }
//         }

//         public static bool operator >=(HeightHoleable a, HeightHoleable b)
//         {
//             return a == b || a > b;
//         }

//         public static Range1h? Range(HeightHoleable min, HeightHoleable max)
//         {
//             if (min.IsNotHole && max.IsNotHole)
//             {
//                 return new Range1h(min, max);
//             }
//             else
//             {
//                 return null;
//             }
//         }

//         public int CompareTo(HeightHoleable other)
//         {
//             return this.Value.CompareTo(other.Value);
//         }

//         public HeightHoleable DefaultIfHole(HeightHoleable b)
//         {
//             return this.IsNotHole ? this : b;
//         }

//         public bool Equals(HeightHoleable other)
//         {
//             return this == other;
//         }

//         public override bool Equals(object obj)
//         {
//             if (!(obj is HeightHoleable))
//                 return false;

//             return this.Equals((HeightHoleable)obj);
//         }

//         public override int GetHashCode()
//         {
//             return Value;
//         }

//         // public static HeightHoleable operator +(HeightHoleable a, HeightHoleable b)
//         // {
//         public HeightHoleable LowerClip(HeightHoleable height, HeightHoleable min)
//         {
//             return (this.IsNotHole && height.IsNotHole && min.IsNotHole) ? Math.Max(this.Value - height.Value, min.Value).CreateHeight() : this;
//         }

//         public override string ToString()
//         {
//             if (IsHole)
//                 return " H";
//             else
//                 return Value.ToString().PadLeft(2);
//         }

//         public Vector3f? ToVector3f(Vector2f vec)
//         {
//             if (IsHole)
//                 return null;
//             else
//                 return new Vector3f(vec.X, vec.Y, Value);
//         }

//         public Vector3f ToVector3f(Vector2f vec, float holeValue)
//         {
//             return new Vector3f(vec.X, vec.Y, IsNotHole ? Value : holeValue);
//         }

//         public static HeightHoleable Lerp(HeightHoleable a, HeightHoleable b, float param)
//         {
//             if (a.IsHole || b.IsHole)
//             {
//                 return HeightHoleable.Hole;
//             }

//             var offsetF = (a.Value - b.Value) * param;
//             int offsetI = 0;
//             if (offsetF > 0.1f)
//                 offsetI = Mathf.Max(1, Mathf.RoundToInt(offsetF));
//             else if (offsetF < -0.1f)
//                 offsetI = Mathf.Min(-1, Mathf.RoundToInt(offsetF));
//             return (b.Value + offsetI).CreateHeight();
//         }

//         public struct Difference
//         {
//             public static readonly Difference Hole = new Difference(HoleValue);
//             public static readonly Difference Infinite = new Difference(InfiniteValue);
//             public static readonly Difference Zero = new Difference();
//             private const int HoleValue = int.MinValue;
//             private const int InfiniteValue = int.MaxValue;
//             private readonly int Value;

//             public Difference(int value)
//             {
//                 Value = value;
//             }

//             public bool IsHole => Value == HoleValue;
//             public bool IsInfinite => Value == InfiniteValue;
//             public bool IsNotHole => Value != HoleValue;
//             public bool IsNotInfinite => Value != InfiniteValue;
//             public Difference Abs => IsHole ? Hole : IsHole || IsInfinite || Value > 0 ? this : new Difference(Math.Abs(Value));

//             public static Difference AbsoluteDif(HeightHoleable a, HeightHoleable b)
//             {
//                 if (a.IsHole && b.IsHole)
//                     return Zero;
//                 else if (a.IsHole || b.IsHole)
//                     return Difference.Infinite;
//                 else
//                     return new Difference(Math.Abs(a.Value - b.Value));
//             }

//             public static HeightHoleable operator -(HeightHoleable a, Difference b)
//             {
//                 if (b.IsInfinite)
//                 {
//                     throw new NotImplementedException("Cannot add infinite difference to height!");
//                 }
//                 else if (a.IsNotHole && b.IsNotHole)
//                 {
//                     return new HeightHoleable(a.Value - b.Value);
//                 }
//                 else
//                 {
//                     return a;
//                 }
//             }

//             public static Difference operator *(Difference a, int b)
//             {
//                 if (a.IsInfinite || a.IsHole)
//                 {
//                     return a;
//                 }
//                 else
//                 {
//                     return new Difference(a.Value * b);
//                 }
//             }

//             public static Difference operator *(Difference a, float b)
//             {
//                 if (a.IsInfinite || a.IsHole)
//                 {
//                     return a;
//                 }
//                 else
//                 {
//                     return new Difference((int)(a.Value * b));
//                 }
//             }

//             public static HeightHoleable operator +(HeightHoleable a, Difference b)
//             {
//                 if (b.IsInfinite)
//                 {
//                     throw new NotImplementedException("Cannot add infinite difference to height!");
//                 }
//                 else if (a.IsNotHole && b.IsNotHole)
//                 {
//                     return new HeightHoleable(a.Value + b.Value);
//                 }
//                 else
//                 {
//                     return a;
//                 }
//             }

//             public static Difference operator +(Difference a, Difference b)
//             {
//                 if (a.IsInfinite || b.IsInfinite)
//                 {
//                     return Difference.Infinite;
//                 }
//                 else if (a.IsNotHole && b.IsNotHole)
//                 {
//                     return new Difference(a.Value + b.Value);
//                 }
//                 else
//                 {
//                     return a.DefaultIfHole(b);
//                 }
//             }

//             public static bool operator <(Difference a, Difference b)
//             {
//                 if (a.IsNotInfinite && b.IsInfinite)
//                 {
//                     return true;
//                 }
//                 else if (a.IsNotHole && b.IsNotHole)
//                 {
//                     return a.Value < b.Value;
//                 }
//                 else
//                 {
//                     return false;
//                 }
//             }

//             public static bool operator >(Difference a, Difference b)
//             {
//                 if (a.IsInfinite && b.IsNotInfinite)
//                 {
//                     return true;
//                 }
//                 else if (a.IsNotHole && b.IsNotHole)
//                 {
//                     return a.Value > b.Value;
//                 }
//                 else
//                 {
//                     return false;
//                 }
//             }

//             public Difference DefaultIfHole(Difference b)
//             {
//                 return this.IsNotHole ? this : b;
//             }

//             public override string ToString()
//             {
//                 if (IsInfinite)
//                     return " I";
//                 else if (IsHole)
//                     return " H";
//                 else
//                     return Value.ToString().PadLeft(2);
//             }

//             public Vector3f? ToVector3f(Vector2f vec)
//             {
//                 if (IsHole)
//                     return null;
//                 else
//                     return new Vector3f(vec.X, vec.Y, Value);
//             }

//             public Vector3f ToVector3f(Vector2f vec, float holeValue)
//             {
//                 return new Vector3f(vec.X, vec.Y, IsHole ? Value : holeValue);
//             }
//         }
//     }

//     public static class HeightUtils
//     {
//         public static HeightHoleable CreateHeight(this int val)
//         {
//             return new HeightHoleable(val);
//         }

//         public static HeightHoleable CreateHeight(this int? val)
//         {
//             return val.HasValue ? new HeightHoleable(val.Value) : HeightHoleable.Hole;
//         }

//         public static HeightHoleable.Difference CreateHeightDifference(this int val)
//         {
//             return new HeightHoleable.Difference(val);
//         }

//         public static HeightHoleable.Difference CreateHeightDifference(this int? val)
//         {
//             return val.HasValue ? new HeightHoleable.Difference(val.Value) : HeightHoleable.Difference.Hole;
//         }

//         public static Vector3f ToVector3f(this Vector2f vec, HeightHoleable z, float holeValue)
//         {
//             return z.ToVector3f(vec, holeValue);
//         }

//         public static Vector3f? ToVector3f(this Vector2f vec, HeightHoleable z)
//         {
//             return z.ToVector3f(vec);
//         }

//         public static Vector3f ToVector3f(this Vector2f vec, HeightHoleable.Difference z, float holeValue)
//         {
//             return z.ToVector3f(vec, holeValue);
//         }

//         public static Vector3f? ToVector3f(this Vector2f vec, HeightHoleable.Difference z)
//         {
//             return z.ToVector3f(vec);
//         }
//     }
// }