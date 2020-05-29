using System;

namespace Votyra.Core.Models
{
    public struct Ray3f : IEquatable<Ray3f>
    {
        public readonly Vector3f Origin;
        public readonly Vector3f Direction;

        public Ray2f XX() => new Ray2f(this.Origin.XX(), this.Direction.XX());

        public Ray2f XY() => new Ray2f(this.Origin.XY(), this.Direction.XY());

        public Ray2f XZ() => new Ray2f(this.Origin.XZ(), this.Direction.XZ());

        public Ray2f YX() => new Ray2f(this.Origin.YX(), this.Direction.YX());

        public Ray2f YY() => new Ray2f(this.Origin.YY(), this.Direction.YY());

        public Ray2f YZ() => new Ray2f(this.Origin.YZ(), this.Direction.YZ());

        public Ray2f ZX() => new Ray2f(this.Origin.ZX(), this.Direction.ZX());

        public Ray2f ZY() => new Ray2f(this.Origin.ZY(), this.Direction.ZY());

        public Ray2f ZZ() => new Ray2f(this.Origin.ZZ(), this.Direction.ZZ());

        public Vector3f ToAt1 => this.Origin + this.Direction;

        public Ray3f(Vector3f origin, Vector3f direction)
        {
            this.Origin = origin;
            this.Direction = direction.Normalized();
        }

        public Vector3f GetPoint(float distance) => this.Origin + (this.Direction * distance);

        public bool Equals(Ray3f other) => this.Origin.Equals(other.Origin) && this.Direction.Equals(other.Direction);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Ray3f other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Origin.GetHashCode() * 397) ^ this.Direction.GetHashCode();
            }
        }

        public override string ToString() => $"origin:{this.Origin} dir:{this.Direction}";
    }
}
