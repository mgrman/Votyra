using System;

namespace Votyra.Core.Models
{
    public struct Line2f : IEquatable<Line2f>
    {
        public readonly Vector2f From;
        public readonly Vector2f To;

        public Line2f(Vector2f from, Vector2f to)
        {
            From = from;
            To = to;
        }

        public bool Equals(Line2f other) => From.Equals(other.From) && To.Equals(other.To);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is Line2f other && Equals(other);
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