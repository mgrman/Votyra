using System;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct Vector4f : IEquatable<Vector4f>
    {
        public readonly float x;
        public readonly float y;
        public readonly float z;
        public readonly float w;

        public Vector4f(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }


        public static bool operator ==(Vector4f a, Vector4f b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w;
        }

        public static bool operator !=(Vector4f a, Vector4f b)
        {
            return a.x != b.x || a.y != b.y || a.z != b.z || a.w != b.w;
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
            return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2 ^ this.w.GetHashCode() >> 1;
        }

        public override string ToString()
        {
            return string.Format("({0} , {1}, {2}, {3})", x, y, z, w);
        }
    }
}