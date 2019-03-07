using System;

namespace Votyra.Core.Models
{
    public struct Ray5f : IEquatable<Ray5f>
    {
        public readonly Vector5f Origin;
        public readonly Vector5f Direction;

        public Ray2f X0X0() => new Ray2f(Origin.X0X0(), Direction.X0X0());

        public Ray2f X0X1() => new Ray2f(Origin.X0X1(), Direction.X0X1());

        public Ray2f X0X2() => new Ray2f(Origin.X0X2(), Direction.X0X2());

        public Ray2f X0X3() => new Ray2f(Origin.X0X3(), Direction.X0X3());

        public Ray2f X0X4() => new Ray2f(Origin.X0X4(), Direction.X0X4());

        public Ray2f X1X0() => new Ray2f(Origin.X1X0(), Direction.X1X0());

        public Ray2f X1X1() => new Ray2f(Origin.X1X1(), Direction.X1X1());

        public Ray2f X1X2() => new Ray2f(Origin.X1X2(), Direction.X1X2());

        public Ray2f X1X3() => new Ray2f(Origin.X1X3(), Direction.X1X3());

        public Ray2f X1X4() => new Ray2f(Origin.X1X4(), Direction.X1X4());

        public Ray2f X2X0() => new Ray2f(Origin.X2X0(), Direction.X2X0());

        public Ray2f X2X1() => new Ray2f(Origin.X2X1(), Direction.X2X1());

        public Ray2f X2X2() => new Ray2f(Origin.X2X2(), Direction.X2X2());

        public Ray2f X2X3() => new Ray2f(Origin.X2X3(), Direction.X2X3());

        public Ray2f X2X4() => new Ray2f(Origin.X2X4(), Direction.X2X4());

        public Ray2f X3X0() => new Ray2f(Origin.X3X0(), Direction.X3X0());

        public Ray2f X3X1() => new Ray2f(Origin.X3X1(), Direction.X3X1());

        public Ray2f X3X2() => new Ray2f(Origin.X3X2(), Direction.X3X2());

        public Ray2f X3X3() => new Ray2f(Origin.X3X3(), Direction.X3X3());

        public Ray2f X3X4() => new Ray2f(Origin.X3X4(), Direction.X3X4());

        public Ray2f X4X0() => new Ray2f(Origin.X4X0(), Direction.X4X0());

        public Ray2f X4X1() => new Ray2f(Origin.X4X1(), Direction.X4X1());

        public Ray2f X4X2() => new Ray2f(Origin.X4X2(), Direction.X4X2());

        public Ray2f X4X3() => new Ray2f(Origin.X4X3(), Direction.X4X3());

        public Ray2f X4X4() => new Ray2f(Origin.X4X4(), Direction.X4X4());

        public Vector5f ToAt1 => Origin + Direction;

        public Ray5f(Vector5f origin, Vector5f direction)
        {
            Origin = origin;
            Direction = direction.Normalized();
        }

        public Vector5f GetPoint(float distance) => Origin + Direction * distance;

        public bool Equals(Ray5f other) => Origin.Equals(other.Origin) && Direction.Equals(other.Direction);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is Ray5f other && Equals(other);
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