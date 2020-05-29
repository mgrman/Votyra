using System;

namespace Votyra.Core.Models
{
    public struct Line3f : IEquatable<Line3f>
    {
        public readonly Vector3f From;
        public readonly Vector3f To;

        public Line3f(Vector3f from, Vector3f to)
        {
            this.From = from;
            this.To = to;
        }

        public bool Equals(Line3f other) => this.From.Equals(other.From) && this.To.Equals(other.To);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Line3f other && this.Equals(other);
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
