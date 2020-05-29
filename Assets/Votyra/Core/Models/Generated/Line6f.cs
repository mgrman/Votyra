using System;

namespace Votyra.Core.Models
{
    public struct Line6f : IEquatable<Line6f>
    {
        public readonly Vector6f From;
        public readonly Vector6f To;

        public Line6f(Vector6f from, Vector6f to)
        {
            this.From = from;
            this.To = to;
        }

        public bool Equals(Line6f other) => this.From.Equals(other.From) && this.To.Equals(other.To);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Line6f other && this.Equals(other);
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
