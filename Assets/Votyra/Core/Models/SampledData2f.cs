using System;
using System.Collections.Generic;
using System.Linq;

namespace Votyra.Core.Models
{
    public struct SampledData2f : IEquatable<SampledData2f>
    {
        public readonly float x0y0;
        public readonly float x0y1;
        public readonly float x1y0;
        public readonly float x1y1;

        public SampledData2f(float x0y0, float x0y1, float x1y0, float x1y1)
        {
            this.x0y0 = x0y0;
            this.x0y1 = x0y1;
            this.x1y0 = x1y0;
            this.x1y1 = x1y1;
        }

        public Area1f? Range
        {
            get
            {
                var min = this.Min;
                var max = this.Max;
                return Area1f.FromMinAndMax(min, max);
            }
        }

        public float Max => Math.Max(this.x0y0, Math.Max(this.x0y1, Math.Max(this.x1y0, this.x1y1)));

        public float Min => Math.Min(this.x0y0, Math.Min(this.x0y1, Math.Min(this.x1y0, this.x1y1)));

        public static SampledData2f operator -(SampledData2f a) => new SampledData2f(-a.x0y0, -a.x0y1, -a.x1y0, -a.x1y1);

        public static SampledData2f operator -(SampledData2f a, float b) => new SampledData2f(a.x0y0 - b, a.x0y1 - b, a.x1y0 - b, a.x1y1 - b);

        public static bool operator !=(SampledData2f a, SampledData2f b) => (a.x0y0 != b.x0y0) && (a.x0y1 != b.x0y1) && (a.x1y0 != b.x1y0) && (a.x1y1 != b.x1y1);

        // public static SampledData2i operator -(SampledData2i a, int b)
        // {
        //     return new SampledData2i(a.x0y0 - b, a.x0y1 - b, a.x1y0 - b, a.x1y1 - b);
        // }
        public static SampledData2f operator *(SampledData2f a, int b) => new SampledData2f(a.x0y0 * b, a.x0y1 * b, a.x1y0 * b, a.x1y1 * b);

        public static SampledData2f operator +(SampledData2f a, float b) => new SampledData2f(a.x0y0 + b, a.x0y1 + b, a.x1y0 + b, a.x1y1 + b);

        public static bool operator ==(SampledData2f a, SampledData2f b) => (a.x0y0 == b.x0y0) && (a.x0y1 == b.x0y1) && (a.x1y0 == b.x1y0) && (a.x1y1 == b.x1y1);

        public SampledData2f ClipMin(float clipValue) => new SampledData2f(Math.Max(this.x0y0, clipValue), Math.Max(this.x0y1, clipValue), Math.Max(this.x1y0, clipValue), Math.Max(this.x1y1, clipValue));

        public override bool Equals(object obj)
        {
            if (obj is SampledData2f)
            {
                var that = (SampledData2f)obj;
                return this.Equals(that);
            }

            return false;
        }

        public bool Equals(SampledData2f that) => this == that;

        public override int GetHashCode()
        {
            unchecked
            {
                return this.x0y0.GetHashCode() + (this.x0y1.GetHashCode() * 7) + (this.x1y0.GetHashCode() * 17) + (this.x1y1.GetHashCode() * 31);
            }
        }

        public float GetIndexedValueCW(int index)
        {
            switch (index % 4)
            {
                case 0:
                    return this.x0y0;

                case 1:
                    return this.x0y1;

                case 2:
                    return this.x1y1;

                case 3:
                    return this.x1y0;

                default:
                    throw new InvalidOperationException();
            }
        }

        public SampledData2f GetRotated(int offset) => new SampledData2f(this.GetIndexedValueCW(0 + offset), this.GetIndexedValueCW(1 + offset), this.GetIndexedValueCW(3 + offset), this.GetIndexedValueCW(2 + offset));

        public override string ToString() => string.Format("x0y0:{0} , x0y1:{1} , x1y0:{2} , x1y1:{3}", this.x0y0, this.x0y1, this.x1y0, this.x1y1);

        public static float Dif(SampledData2f a, SampledData2f b) => Math.Abs(a.x0y0 - b.x0y0) + Math.Abs(a.x0y1 - b.x0y1) + Math.Abs(a.x1y0 - b.x1y0) + Math.Abs(a.x1y1 - b.x1y1);

        public static IEnumerable<SampledData2f> GenerateAllValuesWithHoles(Area1f range, float step)
        {
            var stepCount = (range.Size / step).RoundToVector1i() + 1;
            step = range.Size / (stepCount - 1);

            var results = new List<SampledData2f>();
            foreach (var x0y0 in GenerateValues(range, stepCount, step))
            {
                foreach (var x0y1 in GenerateValues(range, stepCount, step))
                {
                    foreach (var x1y0 in GenerateValues(range, stepCount, step))
                    {
                        foreach (var x1y1 in GenerateValues(range, stepCount, step))
                        {
                            results.Add(new SampledData2f(x0y0, x0y1, x1y0, x1y1));
                        }
                    }
                }
            }

            return results.Distinct();
        }

        public static IEnumerable<float> GenerateValues(Area1f range, int stepCount, float step)
        {
            for (var i = 0; i < stepCount; i++)
            {
                yield return range.Min + (i * step);
            }
        }
    }

    public static class SampledData2fExtensions
    {
        public static SampledData2i ToSampledData2i(this SampledData2f data) => new SampledData2i((int)data.x0y0, (int)data.x0y1, (int)data.x1y0, (int)data.x1y1);
    }
}
