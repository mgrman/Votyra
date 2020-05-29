using System;

namespace Votyra.Core.Models
{
    public struct Ray2f : IEquatable<Ray2f>
    {
        public readonly Vector2f Origin;
        public readonly Vector2f Direction;

        public Ray2f XX() => new Ray2f(this.Origin.XX(), this.Direction.XX());

        public Ray2f XY() => new Ray2f(this.Origin.XY(), this.Direction.XY());

        public Ray2f YX() => new Ray2f(this.Origin.YX(), this.Direction.YX());

        public Ray2f YY() => new Ray2f(this.Origin.YY(), this.Direction.YY());

        public Vector2f ToAt1 => this.Origin + this.Direction;

        public Ray2f(Vector2f origin, Vector2f direction)
        {
            this.Origin = origin;
            this.Direction = direction.Normalized();
        }

        public Vector2f GetPoint(float distance) => this.Origin + (this.Direction * distance);

        public bool Equals(Ray2f other) => this.Origin.Equals(other.Origin) && this.Direction.Equals(other.Direction);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Ray2f other && this.Equals(other);
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
