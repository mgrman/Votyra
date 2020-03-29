using System;

namespace Votyra.Core.Models
{
    public struct Ray1f : IEquatable<Ray1f>
    {
        public readonly float Origin;
        public readonly float Direction;

        public float ToAt1 => Origin + Direction;

        public Ray1f(float origin, float direction)
        {
            Origin = origin;
            Direction = direction.Normalized();
        }

        public float GetPoint(float distance) => Origin + Direction * distance;

        public bool Equals(Ray1f other) => Origin.Equals(other.Origin) && Direction.Equals(other.Direction);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Ray1f other && Equals(other);
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
