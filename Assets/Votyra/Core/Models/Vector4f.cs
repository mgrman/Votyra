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
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public static bool operator ==(Vector4f a, Vector4f b) => a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;

        public static bool operator !=(Vector4f a, Vector4f b) => a.X != b.X || a.Y != b.Y || a.Z != b.Z || a.W != b.W;

        public bool Equals(Vector4f other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Vector4f))
                return false;

            return Equals((Vector4f) obj);
        }

        public override int GetHashCode() => X.GetHashCode() ^ (Y.GetHashCode() << 2) ^ (Z.GetHashCode() >> 2) ^ (W.GetHashCode() >> 1);

        public override string ToString() => string.Format("({0} , {1}, {2}, {3})", X, Y, Z, W);
    }
}