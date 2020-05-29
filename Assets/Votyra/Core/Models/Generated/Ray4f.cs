using System;

namespace Votyra.Core.Models
{
    public struct Ray4f : IEquatable<Ray4f>
    {
        public readonly Vector4f Origin;
        public readonly Vector4f Direction;

        public Ray2f XX() => new Ray2f(this.Origin.XX(), this.Direction.XX());

        public Ray2f XY() => new Ray2f(this.Origin.XY(), this.Direction.XY());

        public Ray2f XZ() => new Ray2f(this.Origin.XZ(), this.Direction.XZ());

        public Ray2f XW() => new Ray2f(this.Origin.XW(), this.Direction.XW());

        public Ray2f YX() => new Ray2f(this.Origin.YX(), this.Direction.YX());

        public Ray2f YY() => new Ray2f(this.Origin.YY(), this.Direction.YY());

        public Ray2f YZ() => new Ray2f(this.Origin.YZ(), this.Direction.YZ());

        public Ray2f YW() => new Ray2f(this.Origin.YW(), this.Direction.YW());

        public Ray2f ZX() => new Ray2f(this.Origin.ZX(), this.Direction.ZX());

        public Ray2f ZY() => new Ray2f(this.Origin.ZY(), this.Direction.ZY());

        public Ray2f ZZ() => new Ray2f(this.Origin.ZZ(), this.Direction.ZZ());

        public Ray2f ZW() => new Ray2f(this.Origin.ZW(), this.Direction.ZW());

        public Ray2f WX() => new Ray2f(this.Origin.WX(), this.Direction.WX());

        public Ray2f WY() => new Ray2f(this.Origin.WY(), this.Direction.WY());

        public Ray2f WZ() => new Ray2f(this.Origin.WZ(), this.Direction.WZ());

        public Ray2f WW() => new Ray2f(this.Origin.WW(), this.Direction.WW());

        public Vector4f ToAt1 => this.Origin + this.Direction;

        public Ray4f(Vector4f origin, Vector4f direction)
        {
            this.Origin = origin;
            this.Direction = direction.Normalized();
        }

        public Vector4f GetPoint(float distance) => this.Origin + (this.Direction * distance);

        public bool Equals(Ray4f other) => this.Origin.Equals(other.Origin) && this.Direction.Equals(other.Direction);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Ray4f other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Origin.GetHashCode() * 397) ^ this.Direction.GetHashCode();
            }
        }

        public override string ToString() => $"origin:{this.Origin} dir:{this.Direction}";
    }
}
