using System;

namespace Votyra.Core.Models
{
    public struct Line4f : IEquatable<Line4f>
    {
        public readonly Vector4f From;
        public readonly Vector4f To;

        public Line4f(Vector4f from, Vector4f to)
        {
            this.From = from;
            this.To = to;
        }

        public bool Equals(Line4f other) => this.From.Equals(other.From) && this.To.Equals(other.To);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Line4f other && this.Equals(other);
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
