using System;

namespace Votyra.Core.Models
{
    public struct Line1f : IEquatable<Line1f>
    {
        public readonly float From;
        public readonly float To;

        public Line1f(float from, float to)
        {
            this.From = from;
            this.To = to;
        }

        public bool Equals(Line1f other) => this.From.Equals(other.From) && this.To.Equals(other.To);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Line1f other && this.Equals(other);
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
