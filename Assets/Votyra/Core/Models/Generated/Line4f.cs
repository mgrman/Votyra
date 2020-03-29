using System;

namespace Votyra.Core.Models
{
    public struct Line4f : IEquatable<Line4f>
    {
        public readonly Vector4f From;
        public readonly Vector4f To;

        public Line4f(Vector4f from, Vector4f to)
        {
            From = from;
            To = to;
        }

        public bool Equals(Line4f other) => From.Equals(other.From) && To.Equals(other.To);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Line4f other && Equals(other);
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
