using System;

namespace Votyra.Core.Models
{
    public struct Ray2f : IEquatable<Ray2f>
    {
        public readonly Vector2f Origin;
        public readonly Vector2f Direction;

        public Ray2f XX() => new Ray2f(Origin.XX(), Direction.XX());

        public Ray2f XY() => new Ray2f(Origin.XY(), Direction.XY());

        public Ray2f YX() => new Ray2f(Origin.YX(), Direction.YX());

        public Ray2f YY() => new Ray2f(Origin.YY(), Direction.YY());

        public Vector2f ToAt1 => Origin + Direction;

        public Ray2f(Vector2f origin, Vector2f direction)
        {
            Origin = origin;
            Direction = direction.Normalized();
        }

        public Vector2f GetPoint(float distance) => Origin + Direction * distance;

        public bool Equals(Ray2f other) => Origin.Equals(other.Origin) && Direction.Equals(other.Direction);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is Ray2f other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Origin.GetHashCode() * 397) ^ Direction.GetHashCode();
            }
        }

        public override string ToString() => $"origin:{Origin} dir:{Direction}";
    }
}