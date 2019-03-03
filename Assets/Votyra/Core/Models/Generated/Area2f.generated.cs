using System;

namespace Votyra.Core.Models
{
    
    
    
    
    
    
    
        
        
    
    
    public partial struct Area2f : IEquatable<Area2f>
    {
        public static readonly Area2f Zero = new Area2f();
        public static readonly Area2f All = new Area2f(Vector2fUtils.FromSame(float.MinValue / 2), Vector2fUtils.FromSame(float.MaxValue / 2));

        public readonly Vector2f Max;
        public readonly Vector2f Min;

        
        public Vector2f X0Y0 => Min;
        public Vector2f X0Y1 => new Vector2f(Min.X,Max.Y);
        public Vector2f X1Y0 => new Vector2f(Max.X, Min.Y);
        public Vector2f X1Y1 => Max;

        public Line2f X0 => new Line2f(X0Y0, X0Y1);
        public Line2f X1 => new Line2f(X1Y0, X1Y1);
        public Line2f Y0 => new Line2f(X0Y0, X1Y0);
        public Line2f Y1 => new Line2f(X0Y1, X1Y1);

        

        private Area2f(Vector2f min, Vector2f max)
        {
            Min = min;
            Max = max;
        }

        public Vector2f Center => (Max + Min) / 2f;
        public Vector2f Size => Max - Min;
        public Vector2f Extents => Size / 2;

        public float DiagonalLength => Size.Magnitude();

        public static Area2f FromMinAndMax(Vector2f min, Vector2f max) => new Area2f(min, max);

        public static Area2f FromMinAndSize(Vector2f min, Vector2f size) => new Area2f(min, min + size);

        public static Area2f FromCenterAndSize(Vector2f center, Vector2f size) => new Area2f(center - size / 2, size);
      
        public static Area2f FromCenterAndExtents(Vector2f center, Vector2f extents)
        {
            if (extents.AnyNegative())
                throw new InvalidOperationException($"When creating {nameof(Area2f)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            return new Area2f(center - extents, center + extents);
        }

        public static bool operator ==(Area2f a, Area2f b) => a.Center == b.Center && a.Size == b.Size;

        public static bool operator !=(Area2f a, Area2f b) => a.Center != b.Center || a.Size != b.Size;

        public static Area2f operator /(Area2f a, float b) => FromMinAndMax(a.Min / b, a.Max / b);

        
        public static Area2f operator /(Area2f a, Vector2f b) => FromMinAndMax(a.Min / b, a.Max / b);

        public Area2f IntersectWith(Area2f that)
        {
            if (Size == Vector2f.Zero || that.Size == Vector2f.Zero)
                return Zero;

            var min = Vector2fUtils.Max(Min, that.Min);
            var max = Vector2fUtils.Max(Vector2fUtils.Min(Max, that.Max), min);

            return FromMinAndMax(min, max);
        }

        public Area2f Encapsulate(Vector2f point) => FromMinAndMax(Vector2fUtils.Min(Min, point), Vector2fUtils.Max(Max, point));

        public Area2f Encapsulate(Area2f bounds) => FromMinAndMax(Vector2fUtils.Min(Min, bounds.Min), Vector2fUtils.Max(Max, bounds.Max));

        public bool Contains(Vector2f point) => point >= Min && point <= Max;

        public Range2i RoundToInt() => Range2i.FromMinAndMax(Min.RoundToVector2i(), Max.RoundToVector2i());

        public Range2i RoundToContain() => Range2i.FromMinAndMax(Min.FloorToVector2i(), Max.CeilToVector2i());

        public Area2f CombineWith(Area2f that)
        {
            if (Size == Vector2f.Zero)
                return that;

            if (that.Size == Vector2f.Zero)
                return this;

            var min = Vector2fUtils.Min(Min, that.Min);
            var max = Vector2fUtils.Max(Max, that.Max);
            return FromMinAndMax(min, max);
        }

        public Area2f UnionWith(Area2f range) => Area2f.FromMinAndMax(Vector2fUtils.Min(Min, range.Min), Vector2fUtils.Min(Max, range.Max));

        public Area2f? UnionWith(Area2f? range)
        {
            if (range == null)
                return this;

            return UnionWith(range.Value);
        }

        public Area2f UnionWith(Vector2f value)
        {
            if (value < Min)
                return Area2f.FromMinAndMax(value, Max);
            if (value > Max)
                return Area2f.FromMinAndMax(Min, value);
            return this;
        }


        public bool Equals(Area2f other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Area2f))
                return false;

            return Equals((Area2f) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Min.GetHashCode() + 7 * Max.GetHashCode();
            }
        }

        public override string ToString() => $"Area2f: min={Min} max={Max} size={Size}";
    }
}