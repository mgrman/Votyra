using System;
using Newtonsoft.Json;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct Ray2f : IEquatable<Ray2f>
    {
        public readonly Vector2f Origin;
        public readonly Vector2f Direction;

        public Vector2f ToAt1 => Origin + Direction;
        
        public Ray2f(Vector2f origin, Vector2f direction)
        {
            Origin = origin;
            Direction = direction.Normalized;
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