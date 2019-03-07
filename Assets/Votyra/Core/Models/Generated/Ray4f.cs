using System;

namespace Votyra.Core.Models
{
    public struct Ray4f : IEquatable<Ray4f>
    {
        public readonly Vector4f Origin;
        public readonly Vector4f Direction;

        public Ray2f XX() => new Ray2f(Origin.XX(), Direction.XX());

        public Ray2f XY() => new Ray2f(Origin.XY(), Direction.XY());

        public Ray2f XZ() => new Ray2f(Origin.XZ(), Direction.XZ());

        public Ray2f XW() => new Ray2f(Origin.XW(), Direction.XW());

        public Ray2f YX() => new Ray2f(Origin.YX(), Direction.YX());

        public Ray2f YY() => new Ray2f(Origin.YY(), Direction.YY());

        public Ray2f YZ() => new Ray2f(Origin.YZ(), Direction.YZ());

        public Ray2f YW() => new Ray2f(Origin.YW(), Direction.YW());

        public Ray2f ZX() => new Ray2f(Origin.ZX(), Direction.ZX());

        public Ray2f ZY() => new Ray2f(Origin.ZY(), Direction.ZY());

        public Ray2f ZZ() => new Ray2f(Origin.ZZ(), Direction.ZZ());

        public Ray2f ZW() => new Ray2f(Origin.ZW(), Direction.ZW());

        public Ray2f WX() => new Ray2f(Origin.WX(), Direction.WX());

        public Ray2f WY() => new Ray2f(Origin.WY(), Direction.WY());

        public Ray2f WZ() => new Ray2f(Origin.WZ(), Direction.WZ());

        public Ray2f WW() => new Ray2f(Origin.WW(), Direction.WW());

        public Vector4f ToAt1 => Origin + Direction;

        public Ray4f(Vector4f origin, Vector4f direction)
        {
            Origin = origin;
            Direction = direction.Normalized();
        }

        public Vector4f GetPoint(float distance) => Origin + Direction * distance;

        public bool Equals(Ray4f other) => Origin.Equals(other.Origin) && Direction.Equals(other.Direction);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is Ray4f other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Origin.GetHashCode() * 397) ^ Direction.GetHashCode();
            }
        }

        public override string ToString() => $"origin:{Origin} dir:{Direction}";
    }
}