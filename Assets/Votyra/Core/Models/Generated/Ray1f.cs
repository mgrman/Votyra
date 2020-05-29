using System;

namespace Votyra.Core.Models
{
    public struct Ray1f : IEquatable<Ray1f>
    {
        public readonly float Origin;
        public readonly float Direction;

        public float ToAt1 => this.Origin + this.Direction;

        public Ray1f(float origin, float direction)
        {
            this.Origin = origin;
            this.Direction = direction.Normalized();
        }

        public float GetPoint(float distance) => this.Origin + (this.Direction * distance);

        public bool Equals(Ray1f other) => this.Origin.Equals(other.Origin) && this.Direction.Equals(other.Direction);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Ray1f other && this.Equals(other);
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
