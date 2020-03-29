using System;

namespace Votyra.Core.Models
{
    public struct Line3f : IEquatable<Line3f>
    {
        public readonly Vector3f From;
        public readonly Vector3f To;

        public Line3f(Vector3f from, Vector3f to)
        {
            From = from;
            To = to;
        }

        public bool Equals(Line3f other) => From.Equals(other.From) && To.Equals(other.To);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Line3f other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (From.GetHashCode() * 397) ^ To.GetHashCode();
            }
        }

        public override string ToString() => $"Origin {From} to {To}";
    }
}
