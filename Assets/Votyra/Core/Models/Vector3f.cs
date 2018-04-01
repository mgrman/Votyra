using System;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct Vector3f : IEquatable<Vector3f>
    {
        public static readonly Vector3f Zero = new Vector3f();

        public static readonly Vector3f One = FromSame(1);

        public readonly float X;

        public readonly float Y;

        public readonly float Z;

        public Vector3f Normalized => this / Magnitude;

        public float Magnitude => (float)Math.Sqrt(SqrMagnitude);

        public float SqrMagnitude => X * X + Y * Y + Z * Z;

        public bool Positive => this.X > 0 && this.Y > 0 && this.Z > 0;

        public bool ZeroOrPositive => this.X >= 0 && this.Y >= 0 && this.Z >= 0;

        public float VolumeSum => X * Y * Z;

        public Vector2f XY => new Vector2f(X, Y);

        public bool AnyNegative => this.X < 0 || this.Y < 0 || this.Z < 0;

        public Vector3f(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public static Vector3f FromSame(float value)
        {
            return new Vector3f(value, value, value);
        }

        public Vector3i FloorToVector3i()
        {
            return new Vector3i(X.FloorToInt(), Y.FloorToInt(), Z.FloorToInt());
        }

        public Vector3i RoundToVector3i()
        {
            return new Vector3i(X.RoundToInt(), Y.RoundToInt(), Z.RoundToInt());
        }

        public Vector3i CeilToVector3i()
        {
            return new Vector3i(X.CeilToInt(), Y.CeilToInt(), Z.CeilToInt());
        }

        public static float Dot(Vector3f lhs, Vector3f rhs)
        {
            return (float)((double)lhs.X * (double)rhs.X + (double)lhs.Y * (double)rhs.Y + (double)lhs.Z * (double)rhs.Z);
        }

        public static Vector3f Cross(Vector3f lhs, Vector3f rhs)
        {
            return new Vector3f((float)((double)lhs.Y * (double)rhs.Z - (double)lhs.Z * (double)rhs.Y), (float)((double)lhs.Z * (double)rhs.X - (double)lhs.X * (double)rhs.Z), (float)((double)lhs.X * (double)rhs.Y - (double)lhs.Y * (double)rhs.X));
        }

        public static bool operator >(Vector3f a, Vector3f b)
        {
            return a.X > b.X && a.Y > b.Y && a.Z > b.Z;
        }

        public static bool operator >=(Vector3f a, Vector3f b)
        {
            return a.X >= b.X && a.Y >= b.Y && a.Z >= b.Z;
        }

        public static bool operator <(Vector3f a, Vector3f b)
        {
            return a.X < b.X && a.Y < b.Y && a.Z < b.Z;
        }

        public static bool operator <=(Vector3f a, Vector3f b)
        {
            return a.X <= b.X && a.Y <= b.Y && a.Z <= b.Z;
        }

        public static Vector3f operator +(Vector3f a, float b)
        {
            return new Vector3f(a.X + b, a.Y + b, a.Z + b);
        }

        public static Vector3f operator -(Vector3f a, float b)
        {
            return new Vector3f(a.X - b, a.Y - b, a.Z - b);
        }

        public static Vector3f operator -(Vector3f a)
        {
            return new Vector3f(-a.X, -a.Y, -a.Z);
        }

        public static Vector3f operator +(Vector3f a, Vector3f b)
        {
            return new Vector3f(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3f operator -(Vector3f a, Vector3f b)
        {
            return new Vector3f(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector3f operator *(Vector3f a, Vector3f b)
        {
            return new Vector3f(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static Vector3f operator *(Vector3f a, float b)
        {
            return new Vector3f(a.X * b, a.Y * b, a.Z * b);
        }

        public static Vector3f operator /(Vector3f a, float b)
        {
            return new Vector3f(a.X / b, a.Y / b, a.Z / b);
        }

        public static Vector3f operator /(Vector3f a, Vector3f b)
        {
            return new Vector3f(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        public static Vector3f Max(Vector3f a, Vector3f b)
        {
            return new Vector3f(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));
        }

        public static Vector3f Min(Vector3f a, Vector3f b)
        {
            return new Vector3f(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
        }

        public static bool operator ==(Vector3f a, Vector3f b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        public static bool operator !=(Vector3f a, Vector3f b)
        {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
        }

        public bool Equals(Vector3f other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector3f))
                return false;

            return this.Equals((Vector3f)obj);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode() * 7 + Z.GetHashCode() * 13;
        }

        public override string ToString()
        {
            return string.Format("({0} , {1}, {2})", X, Y, Z);
        }
    }
}