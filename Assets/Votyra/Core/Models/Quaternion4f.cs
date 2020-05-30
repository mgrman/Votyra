using System;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct Quaternion4F : IEquatable<Quaternion4F>
    {
        public readonly float X;
        public readonly float Y;
        public readonly float Z;
        public readonly float W;

        public Quaternion4F(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public static Quaternion4F Euler(float x, float y, float z)
        {
            x *= MathUtils.Deg2Rad;
            y *= MathUtils.Deg2Rad;
            z *= MathUtils.Deg2Rad;
            var rollOver2 = z * 0.5f;
            var sinRollOver2 = (float)Math.Sin(rollOver2);
            var cosRollOver2 = (float)Math.Cos(rollOver2);
            var pitchOver2 = y * 0.5f;
            var sinPitchOver2 = (float)Math.Sin(pitchOver2);
            var cosPitchOver2 = (float)Math.Cos(pitchOver2);
            var yawOver2 = x * 0.5f;
            var sinYawOver2 = (float)Math.Sin(yawOver2);
            var cosYawOver2 = (float)Math.Cos(yawOver2);

            var qw = (cosYawOver2 * cosPitchOver2 * cosRollOver2) + (sinYawOver2 * sinPitchOver2 * sinRollOver2);
            var qx = (cosYawOver2 * sinPitchOver2 * cosRollOver2) + (sinYawOver2 * cosPitchOver2 * sinRollOver2);
            var qy = (sinYawOver2 * cosPitchOver2 * cosRollOver2) - (cosYawOver2 * sinPitchOver2 * sinRollOver2);
            var qz = (cosYawOver2 * cosPitchOver2 * sinRollOver2) - (sinYawOver2 * sinPitchOver2 * cosRollOver2);

            return new Quaternion4F(qx, qy, qz, qw);
        }

        public static bool operator ==(Quaternion4F a, Quaternion4F b) => (a.X == b.X) && (a.Y == b.Y) && (a.Z == b.Z) && (a.W == b.W);

        public static bool operator !=(Quaternion4F a, Quaternion4F b) => (a.X != b.X) || (a.Y != b.Y) || (a.Z != b.Z) || (a.W != b.W);

        public bool Equals(Quaternion4F other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Quaternion4F))
            {
                return false;
            }

            return this.Equals((Quaternion4F)obj);
        }

        public override int GetHashCode() => this.X.GetHashCode() ^ (this.Y.GetHashCode() << 2) ^ (this.Z.GetHashCode() >> 2) ^ (this.W.GetHashCode() >> 1);

        public override string ToString() => string.Format("({0} , {1}, {2}, {3})", this.X, this.Y, this.Z, this.W);
    }
}
