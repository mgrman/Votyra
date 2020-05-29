using System;

namespace Votyra.Core.Models
{
    public struct Line2f : IEquatable<Line2f>
    {
        public readonly Vector2f From;
        public readonly Vector2f To;

        public Line2f(Vector2f from, Vector2f to)
        {
            this.From = from;
            this.To = to;
        }

        public bool Equals(Line2f other) => this.From.Equals(other.From) && this.To.Equals(other.To);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Line2f other && this.Equals(other);
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
