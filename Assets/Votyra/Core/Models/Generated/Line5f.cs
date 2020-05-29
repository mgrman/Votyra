using System;

namespace Votyra.Core.Models
{
    public struct Line5f : IEquatable<Line5f>
    {
        public readonly Vector5f From;
        public readonly Vector5f To;

        public Line5f(Vector5f from, Vector5f to)
        {
            this.From = from;
            this.To = to;
        }

        public bool Equals(Line5f other) => this.From.Equals(other.From) && this.To.Equals(other.To);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Line5f other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.From.GetHashCode() * 397) ^ this.To.GetHashCode();
            }
        }

        public override string ToString() => $"Origin {this.From} to {this.To}";
    }
}
