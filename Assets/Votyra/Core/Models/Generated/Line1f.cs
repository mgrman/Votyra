using System;

namespace Votyra.Core.Models
{
    public struct Line1f : IEquatable<Line1f>
    {
        public readonly float From;
        public readonly float To;

        public Line1f(float from, float to)
        {
            From = from;
            To = to;
        }

        public bool Equals(Line1f other) => From.Equals(other.From) && To.Equals(other.To);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is Line1f other && Equals(other);
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