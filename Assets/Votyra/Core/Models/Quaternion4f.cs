using System;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct Quaternion4f : IEquatable<Quaternion4f>
    {
        public readonly float w;
        public readonly float x;
        public readonly float y;
        public readonly float z;

        public Quaternion4f(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static Quaternion4f Euler(float x, float y, float z)
        {
            x *= MathUtils.Deg2Rad;
            y *= MathUtils.Deg2Rad;
            z *= MathUtils.Deg2Rad;
            float rollOver2 = z * 0.5f;
            float sinRollOver2 = (float)Math.Sin((double)rollOver2);
            float cosRollOver2 = (float)Math.Cos((double)rollOver2);
            float pitchOver2 = y * 0.5f;
            float sinPitchOver2 = (float)Math.Sin((double)pitchOver2);
            float cosPitchOver2 = (float)Math.Cos((double)pitchOver2);
            float yawOver2 = x * 0.5f;
            float sinYawOver2 = (float)Math.Sin((double)yawOver2);
            float cosYawOver2 = (float)Math.Cos((double)yawOver2);

            float qw = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
            float qx = cosYawOver2 * sinPitchOver2 * cosRollOver2 + sinYawOver2 * cosPitchOver2 * sinRollOver2;
            float qy = sinYawOver2 * cosPitchOver2 * cosRollOver2 - cosYawOver2 * sinPitchOver2 * sinRollOver2;
            float qz = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;

            return new Quaternion4f(qx, qy, qz, qw);
        }

        public static bool operator !=(Quaternion4f a, Quaternion4f b)
        {
            return a.x != b.x || a.y != b.y || a.z != b.z || a.w != b.w;
        }

        public static bool operator ==(Quaternion4f a, Quaternion4f b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w;
        }

        public bool Equals(Quaternion4f other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Quaternion4f))
                return false;

            return this.Equals((Quaternion4f)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2 ^ this.w.GetHashCode() >> 1;
            }
        }

        public override string ToString()
        {
            return string.Format("({0} , {1}, {2}, {3})", x, y, z, w);
        }
    }
}