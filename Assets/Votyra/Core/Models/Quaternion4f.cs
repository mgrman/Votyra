using System;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct Quaternion4f : IEquatable<Quaternion4f>
    {
        public readonly float x;
        public readonly float y;
        public readonly float z;
        public readonly float w;

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
            var rollOver2 = z * 0.5f;
            var sinRollOver2 = (float) Math.Sin(rollOver2);
            var cosRollOver2 = (float) Math.Cos(rollOver2);
            var pitchOver2 = y * 0.5f;
            var sinPitchOver2 = (float) Math.Sin(pitchOver2);
            var cosPitchOver2 = (float) Math.Cos(pitchOver2);
            var yawOver2 = x * 0.5f;
            var sinYawOver2 = (float) Math.Sin(yawOver2);
            var cosYawOver2 = (float) Math.Cos(yawOver2);

            var qw = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
            var qx = cosYawOver2 * sinPitchOver2 * cosRollOver2 + sinYawOver2 * cosPitchOver2 * sinRollOver2;
            var qy = sinYawOver2 * cosPitchOver2 * cosRollOver2 - cosYawOver2 * sinPitchOver2 * sinRollOver2;
            var qz = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;

            return new Quaternion4f(qx, qy, qz, qw);
        }

        public static bool operator ==(Quaternion4f a, Quaternion4f b) => a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w;

        public static bool operator !=(Quaternion4f a, Quaternion4f b) => a.x != b.x || a.y != b.y || a.z != b.z || a.w != b.w;

        public bool Equals(Quaternion4f other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Quaternion4f))
                return false;

            return Equals((Quaternion4f) obj);
        }

        public override int GetHashCode() => x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2) ^ (w.GetHashCode() >> 1);

        public override string ToString() => string.Format("({0} , {1}, {2}, {3})", x, y, z, w);
    }
}