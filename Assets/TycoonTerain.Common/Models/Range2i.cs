using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public struct Range2i : IEquatable<Range2i>
{
    public readonly int min;
    public readonly int max;
    public Range2i(int min, int max)
    {
        this.min = Mathf.Min(min, max);
        this.max = Mathf.Max(min, max);
    }
    public Range2i(Range2 range)
        : this(Mathf.FloorToInt(range.min), Mathf.FloorToInt(range.max))
    {
    }

    public float Center
    {
        get
        {
            return (max + min) / 2.0f;
        }
    }

    public int FlooredCenter
    {
        get
        {
            return (max + min) / 2;
        }
    }

    public int Size
    {
        get
        {
            return max - min;
        }
    }

    public Range2i UnionWith(Range2i range)
    {
        return new Range2i(Math.Min(this.min, range.min), Math.Min(this.max, range.max));
    }

    public static bool operator ==(Range2i a, Range2i b)
    {
        return a.min == b.min && a.max == b.max;
    }

    public static bool operator !=(Range2i a, Range2i b)
    {
        return a.min != b.min || a.max != b.max;
    }

    public bool Equals(Range2i other)
    {
        return this == other;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Range2i))
            return false;

        return this.Equals((Range2i)obj);
    }

    public override int GetHashCode()
    {
        return min.GetHashCode() + max.GetHashCode() * 7;
    }

    public override string ToString()
    {
        return string.Format("({0} , {1})", min, max);
    }
}
