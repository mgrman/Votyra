using System;

namespace Votyra.Core.Models
{
    public struct Line6f : IEquatable<Line6f>
    {
        public readonly Vector6f From;
        public readonly Vector6f To;

        public Line6f(Vector6f from, Vector6f to)
        {
            From = from;
            To = to;
        }

        public bool Equals(Line6f other) => From.Equals(other.From) && To.Equals(other.To);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is Line6f other && Equals(other);
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