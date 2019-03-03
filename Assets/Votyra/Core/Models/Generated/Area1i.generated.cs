using System;

namespace Votyra.Core.Models
{
    /// <summary>
    ///     Max is inclusive
    /// </summary>
    
    
    
    
    
    
    
        
        
    
    
    public partial struct Area1i : IEquatable<Area1i>
    {
        public static readonly Area1i All = new Area1i(Vector1iUtils.FromSame(int.MinValue / 2), Vector1iUtils.FromSame(int.MaxValue / 2 - 1));

        public static readonly Area1i Zero = new Area1i();

        public readonly int Min;

        public readonly int Max;

        private Area1i(int min, int max)
        {
            Min = min;
            Max = max;
            if (Size.AnyNegative())
                throw new InvalidOperationException($"{nameof(Area1i)} '{this}' cannot have a size be zero or negative!");
            if (Size.AnyZero())
                Max = Min;
        }

        public int Size => Max - Min + Vector1i.One;

        public static Area1i FromCenterAndExtents(int center, int extents)
        {
            if (extents.AnyNegative())
                throw new InvalidOperationException($"When creating {nameof(Area1i)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            return new Area1i(center - extents, center + extents);
        }

        public static Area1i FromMinAndSize(int min, int size)
        {
            if (size.AnyNegative())
                throw new InvalidOperationException($"When creating {nameof(Area1i)} using min '{min}' and size '{size}', size cannot have a negative coordinate!");
            return new Area1i(min, min + size);
        }

        public static Area1i FromMinAndMax(int min, int max) => new Area1i(min, max);

        public static bool operator ==(Area1i a, Area1i b) => a.Min == b.Min && a.Max == b.Max;

        public static bool operator !=(Area1i a, Area1i b) => a.Min != b.Min || a.Max != b.Max;

        public Area1i ExtendBothDirections(int distance) => FromMinAndMax(Min - distance, Max + distance);

        public Range1i ToRange1i() => Range1i.FromMinAndMax(Min, Max + Vector1i.One);

        public Area1f ToArea1f() => Area1f.FromMinAndMax(Min.ToVector1f(), Max.ToVector1f());

        public bool Contains(int point) => point >= Min && point <= Max;

        public bool Overlaps(Area1i that)
        {
            if (Size == Vector1i.Zero || that.Size == Vector1i.Zero)
                return false;

            return Min <= that.Max && that.Min <= Max;
        }

        public Area1i CombineWith(Area1i that)
        {
            if (Size == Vector1i.Zero)
                return that;

            if (that.Size == Vector1i.Zero)
                return this;

            var min = Vector1iUtils.Min(Min, that.Min);
            var max = Vector1iUtils.Max(Max, that.Max);
            return FromMinAndMax(min, max);
        }

        public Area1i CombineWith(int point)
        {
            if (Contains(point))
                return this;

            var min = Vector1iUtils.Min(Min, point);
            var max = Vector1iUtils.Max(Max, point);

            return FromMinAndMax(min, max);
        }

        public Area1i IntersectWith(Area1i that)
        {
            if (Size == Vector1i.Zero || that.Size == Vector1i.Zero)
                return Zero;

            var min = Vector1iUtils.Max(Min, that.Min);
            var max = Vector1iUtils.Max(Vector1iUtils.Min(Max, that.Max), min);

            return FromMinAndMax(min, max);
        }

        public Area1i UnionWith(Area1i? that) 
        {
            if (that == null)
                return this;

            return UnionWith(that.Value);
        }

        public Area1i UnionWith(Area1i that)
        {
            if (Size == Vector1i.Zero || that.Size == Vector1i.Zero)
                return Zero;

            var min = Vector1iUtils.Min(Min, that.Min);
            var max = Vector1iUtils.Max(Max, that.Max);

            return FromMinAndMax(min, max);
        }

        public bool Equals(Area1i other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Area1i))
                return false;

            return Equals((Area1i) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Min.GetHashCode() + 7 * Max.GetHashCode();
            }
        }

        public override string ToString() => $"Area1i: min={Min} max={Max} size={Size}";
    }
}