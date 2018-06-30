using System;

namespace Votyra.Core.Models
{
    public struct Vector4f : IEquatable<Vector4f>
    {
        public readonly float X;
        public readonly float Y;
        public readonly float Z;
        public readonly float W;

        public Vector4f(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public static bool operator ==(Vector4f a, Vector4f b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;
        }

        public static bool operator !=(Vector4f a, Vector4f b)
        {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z || a.W != b.W;
        }

        public bool Equals(Vector4f other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector4f))
                return false;

            return this.Equals((Vector4f)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.X.GetHashCode() ^ this.Y.GetHashCode() << 2 ^ this.Z.GetHashCode() >> 2 ^ this.W.GetHashCode() >> 1;
            }
        }

        public override string ToString()
        {
            return string.Format("({0} , {1}, {2}, {3})", X, Y, Z, W);
        }
    }
}