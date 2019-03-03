using System;

namespace Votyra.Core.Models
{
    /// <summary>
    ///     Max is inclusive
    /// </summary>
    
    
    
    
    
    
    
        
        
    
    
    public partial struct Area6i : IEquatable<Area6i>
    {
        public static readonly Area6i All = new Area6i(Vector6iUtils.FromSame(int.MinValue / 2), Vector6iUtils.FromSame(int.MaxValue / 2 - 1));

        public static readonly Area6i Zero = new Area6i();

        public readonly Vector6i Min;

        public readonly Vector6i Max;

        private Area6i(Vector6i min, Vector6i max)
        {
            Min = min;
            Max = max;
            if (Size.AnyNegative())
                throw new InvalidOperationException($"{nameof(Area6i)} '{this}' cannot have a size be zero or negative!");
            if (Size.AnyZero())
                Max = Min;
        }

        public Vector6i Size => Max - Min + Vector6i.One;

        public static Area6i FromCenterAndExtents(Vector6i center, Vector6i extents)
        {
            if (extents.AnyNegative())
                throw new InvalidOperationException($"When creating {nameof(Area6i)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            return new Area6i(center - extents, center + extents);
        }

        public static Area6i FromMinAndSize(Vector6i min, Vector6i size)
        {
            if (size.AnyNegative())
                throw new InvalidOperationException($"When creating {nameof(Area6i)} using min '{min}' and size '{size}', size cannot have a negative coordinate!");
            return new Area6i(min, min + size);
        }

        public static Area6i FromMinAndMax(Vector6i min, Vector6i max) => new Area6i(min, max);

        public static bool operator ==(Area6i a, Area6i b) => a.Min == b.Min && a.Max == b.Max;

        public static bool operator !=(Area6i a, Area6i b) => a.Min != b.Min || a.Max != b.Max;

        public Area6i ExtendBothDirections(int distance) => FromMinAndMax(Min - distance, Max + distance);

        public Range6i ToRange6i() => Range6i.FromMinAndMax(Min, Max + Vector6i.One);

        public Area6f ToArea6f() => Area6f.FromMinAndMax(Min.ToVector6f(), Max.ToVector6f());

        public bool Contains(Vector6i point) => point >= Min && point <= Max;

        public bool Overlaps(Area6i that)
        {
            if (Size == Vector6i.Zero || that.Size == Vector6i.Zero)
                return false;

            return Min <= that.Max && that.Min <= Max;
        }

        public Area6i CombineWith(Area6i that)
        {
            if (Size == Vector6i.Zero)
                return that;

            if (that.Size == Vector6i.Zero)
                return this;

            var min = Vector6iUtils.Min(Min, that.Min);
            var max = Vector6iUtils.Max(Max, that.Max);
            return FromMinAndMax(min, max);
        }

        public Area6i CombineWith(Vector6i point)
        {
            if (Contains(point))
                return this;

            var min = Vector6iUtils.Min(Min, point);
            var max = Vector6iUtils.Max(Max, point);

            return FromMinAndMax(min, max);
        }

        public Area6i IntersectWith(Area6i that)
        {
            if (Size == Vector6i.Zero || that.Size == Vector6i.Zero)
                return Zero;

            var min = Vector6iUtils.Max(Min, that.Min);
            var max = Vector6iUtils.Max(Vector6iUtils.Min(Max, that.Max), min);

            return FromMinAndMax(min, max);
        }

        public Area6i UnionWith(Area6i? that) 
        {
            if (that == null)
                return this;

            return UnionWith(that.Value);
        }

        public Area6i UnionWith(Area6i that)
        {
            if (Size == Vector6i.Zero || that.Size == Vector6i.Zero)
                return Zero;

            var min = Vector6iUtils.Min(Min, that.Min);
            var max = Vector6iUtils.Max(Max, that.Max);

            return FromMinAndMax(min, max);
        }

        public bool Equals(Area6i other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Area6i))
                return false;

            return Equals((Area6i) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Min.GetHashCode() + 7 * Max.GetHashCode();
            }
        }

        public override string ToString() => $"Area6i: min={Min} max={Max} size={Size}";
    }
}