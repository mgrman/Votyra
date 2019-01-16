using System;

namespace Votyra.Core.Models
{
    /// <summary>
    ///     Max is inclusive
    /// </summary>
    public struct Area2i : IEquatable<Area2i>
    {
        public static readonly Area2i All = new Area2i(Vector2i.FromSame(int.MinValue / 2), Vector2i.FromSame(int.MaxValue / 2 - 1));

        public static readonly Area2i Zero = new Area2i();

        public readonly Vector2i Min;

        public readonly Vector2i Max;

        private Area2i(Vector2i min, Vector2i max)
        {
            Min = min;
            Max = max;
            if (Size.AnyNegative)
                throw new InvalidOperationException($"{nameof(Area2i)} '{this}' cannot have a size be zero or negative!");
            if (Size.AnyZero)
                Max = Min;
        }

        public Vector2i Size => Max - Min + Vector2i.One;

        public static Area2i FromCenterAndExtents(Vector2i center, Vector2i extents)
        {
            if (extents.AnyNegative)
                throw new InvalidOperationException($"When creating {nameof(Area2i)} from center '{center}' and extents '{extents}', extents cannot have a negative coordinate!");
            return new Area2i(center - extents, center + extents);
        }

        public static Area2i FromMinAndSize(Vector2i min, Vector2i size)
        {
            if (size.AnyNegative)
                throw new InvalidOperationException($"When creating {nameof(Area2i)} using min '{min}' and size '{size}', size cannot have a negative coordinate!");
            return new Area2i(min, min + size);
        }

        public static Area2i FromMinAndMax(Vector2i min, Vector2i max) => new Area2i(min, max);

        public static bool operator ==(Area2i a, Area2i b) => a.Min == b.Min && a.Max == b.Max;

        public static bool operator !=(Area2i a, Area2i b) => a.Min != b.Min || a.Max != b.Max;

        public Area2i ExtendBothDirections(int distance) => FromMinAndMax(Min - distance, Max + distance);

        public Range2i ToRange2i() => Range2i.FromMinAndMax(Min, Max + Vector2i.One);

        // public Area2i CeilTo2()
        // {
        //     return Area2i.FromMinAndSize(Min, Size + Size % 2);
        // }
        public Area2f ToArea2f() => Area2f.FromMinAndMax(Min.ToVector2f(), Max.ToVector2f());

        public bool Contains(Vector2i point) => point >= Min && point <= Max;

        public bool Overlaps(Area2i that)
        {
            if (Size == Vector2i.Zero || that.Size == Vector2i.Zero)
                return false;

            return Min <= that.Max && that.Min <= Max;
        }

        public Area2i CombineWith(Area2i that)
        {
            if (Size == Vector2i.Zero)
                return that;

            if (that.Size == Vector2i.Zero)
                return this;

            var min = Vector2i.Min(Min, that.Min);
            var max = Vector2i.Max(Max, that.Max);
            return FromMinAndMax(min, max);
        }

        public Area2i CombineWith(Vector2i point)
        {
            if (Contains(point))
                return this;

            var min = Vector2i.Min(Min, point);
            var max = Vector2i.Max(Max, point);

            return FromMinAndMax(min, max);
        }

        public Area2i IntersectWith(Area2i that)
        {
            if (Size == Vector2i.Zero || that.Size == Vector2i.Zero)
                return Zero;

            var min = Vector2i.Max(Min, that.Min);
            var max = Vector2i.Max(Vector2i.Min(Max, that.Max), min);

            return FromMinAndMax(min, max);
        }

        public Area2i UnionWith(Area2i? that) => this;

        public Area2i UnionWith(Area2i that)
        {
            if (Size == Vector2i.Zero || that.Size == Vector2i.Zero)
                return Zero;

            var min = Vector2i.Min(Min, that.Min);
            var max = Vector2i.Max(Max, that.Max);

            return FromMinAndMax(min, max);
        }

        public bool Equals(Area2i other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Area2i))
                return false;

            return Equals((Area2i) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Min.GetHashCode() + 7 * Max.GetHashCode();
            }
        }

        public override string ToString() => $"Area2i: min={Min} max={Max} size={Size}";
    }
}