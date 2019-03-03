using System;

namespace Votyra.Core.Models
{
    /// <summary>
    ///     Max is inclusive
    /// </summary>
    
    
    
    
    
    
    
        
        
    
    
    public partial struct Area3i : IEquatable<Area3i>
    {
        public static readonly Area3i All = new Area3i(Vector3iUtils.FromSame(int.MinValue / 2), Vector3iUtils.FromSame(int.MaxValue / 2 - 1));

        public static readonly Area3i Zero = new Area3i();

        public readonly Vector3i Min;

        public readonly Vector3i Max;

        private Area3i(Vector3i min, Vector3i max)
        {
            Min = min;
            Max = max;
            if (Size.AnyNegative())
                throw new InvalidOperationException($"{nameof(Area3i)} '{this}' cannot have a size be zero or negative!");
            if (Size.AnyZero())
                Max = Min;
        }

        public Vector3i Size => Max - Min + Vector3i.One;

        public static Area3i FromCenterAndExtents(Vector3i center, Vector3i extents)
        {
            if (extents.AnyNegative())
                throw new InvalidOperationException($"When creating {nameof(Area3i)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            return new Area3i(center - extents, center + extents);
        }

        public static Area3i FromMinAndSize(Vector3i min, Vector3i size)
        {
            if (size.AnyNegative())
                throw new InvalidOperationException($"When creating {nameof(Area3i)} using min '{min}' and size '{size}', size cannot have a negative coordinate!");
            return new Area3i(min, min + size);
        }

        public static Area3i FromMinAndMax(Vector3i min, Vector3i max) => new Area3i(min, max);

        public static bool operator ==(Area3i a, Area3i b) => a.Min == b.Min && a.Max == b.Max;

        public static bool operator !=(Area3i a, Area3i b) => a.Min != b.Min || a.Max != b.Max;

        public Area3i ExtendBothDirections(int distance) => FromMinAndMax(Min - distance, Max + distance);

        public Range3i ToRange3i() => Range3i.FromMinAndMax(Min, Max + Vector3i.One);

        public Area3f ToArea3f() => Area3f.FromMinAndMax(Min.ToVector3f(), Max.ToVector3f());

        public bool Contains(Vector3i point) => point >= Min && point <= Max;

        public bool Overlaps(Area3i that)
        {
            if (Size == Vector3i.Zero || that.Size == Vector3i.Zero)
                return false;

            return Min <= that.Max && that.Min <= Max;
        }

        public Area3i CombineWith(Area3i that)
        {
            if (Size == Vector3i.Zero)
                return that;

            if (that.Size == Vector3i.Zero)
                return this;

            var min = Vector3iUtils.Min(Min, that.Min);
            var max = Vector3iUtils.Max(Max, that.Max);
            return FromMinAndMax(min, max);
        }

        public Area3i CombineWith(Vector3i point)
        {
            if (Contains(point))
                return this;

            var min = Vector3iUtils.Min(Min, point);
            var max = Vector3iUtils.Max(Max, point);

            return FromMinAndMax(min, max);
        }

        public Area3i IntersectWith(Area3i that)
        {
            if (Size == Vector3i.Zero || that.Size == Vector3i.Zero)
                return Zero;

            var min = Vector3iUtils.Max(Min, that.Min);
            var max = Vector3iUtils.Max(Vector3iUtils.Min(Max, that.Max), min);

            return FromMinAndMax(min, max);
        }

        public Area3i UnionWith(Area3i? that) 
        {
            if (that == null)
                return this;

            return UnionWith(that.Value);
        }

        public Area3i UnionWith(Area3i that)
        {
            if (Size == Vector3i.Zero || that.Size == Vector3i.Zero)
                return Zero;

            var min = Vector3iUtils.Min(Min, that.Min);
            var max = Vector3iUtils.Max(Max, that.Max);

            return FromMinAndMax(min, max);
        }

        public bool Equals(Area3i other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Area3i))
                return false;

            return Equals((Area3i) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Min.GetHashCode() + 7 * Max.GetHashCode();
            }
        }

        public override string ToString() => $"Area3i: min={Min} max={Max} size={Size}";
    }
}