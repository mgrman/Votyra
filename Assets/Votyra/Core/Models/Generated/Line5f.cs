using System;

namespace Votyra.Core.Models
{
    public struct Line5f : IEquatable<Line5f>
    {
        public readonly Vector5f From;
        public readonly Vector5f To;

        public Line5f(Vector5f from, Vector5f to)
        {
            From = from;
            To = to;
        }

        public bool Equals(Line5f other) => From.Equals(other.From) && To.Equals(other.To);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Line5f other && Equals(other);
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
