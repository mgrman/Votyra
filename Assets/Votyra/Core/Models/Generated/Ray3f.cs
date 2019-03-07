using System;

namespace Votyra.Core.Models
{
    public struct Ray3f : IEquatable<Ray3f>
    {
        public readonly Vector3f Origin;
        public readonly Vector3f Direction;

        public Ray2f XX() => new Ray2f(Origin.XX(), Direction.XX());

        public Ray2f XY() => new Ray2f(Origin.XY(), Direction.XY());

        public Ray2f XZ() => new Ray2f(Origin.XZ(), Direction.XZ());

        public Ray2f YX() => new Ray2f(Origin.YX(), Direction.YX());

        public Ray2f YY() => new Ray2f(Origin.YY(), Direction.YY());

        public Ray2f YZ() => new Ray2f(Origin.YZ(), Direction.YZ());

        public Ray2f ZX() => new Ray2f(Origin.ZX(), Direction.ZX());

        public Ray2f ZY() => new Ray2f(Origin.ZY(), Direction.ZY());

        public Ray2f ZZ() => new Ray2f(Origin.ZZ(), Direction.ZZ());

        public Vector3f ToAt1 => Origin + Direction;

        public Ray3f(Vector3f origin, Vector3f direction)
        {
            Origin = origin;
            Direction = direction.Normalized();
        }

        public Vector3f GetPoint(float distance) => Origin + Direction * distance;

        public bool Equals(Ray3f other) => Origin.Equals(other.Origin) && Direction.Equals(other.Direction);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is Ray3f other && Equals(other);
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