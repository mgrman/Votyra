using System;
using System.Collections.Generic;
using System.Linq;

namespace Votyra.Core.Models
{
    public struct SampledData2F : IEquatable<SampledData2F>
    {
        public readonly float X0Y0;
        public readonly float X0Y1;
        public readonly float X1Y0;
        public readonly float X1Y1;

        public SampledData2F(float x0Y0, float x0Y1, float x1Y0, float x1Y1)
        {
            this.X0Y0 = x0Y0;
            this.X0Y1 = x0Y1;
            this.X1Y0 = x1Y0;
            this.X1Y1 = x1Y1;
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

        public float Max => Math.Max(this.X0Y0, Math.Max(this.X0Y1, Math.Max(this.X1Y0, this.X1Y1)));

        public float Min => Math.Min(this.X0Y0, Math.Min(this.X0Y1, Math.Min(this.X1Y0, this.X1Y1)));

        public static SampledData2F operator -(SampledData2F a) => new SampledData2F(-a.X0Y0, -a.X0Y1, -a.X1Y0, -a.X1Y1);

        public static SampledData2F operator -(SampledData2F a, float b) => new SampledData2F(a.X0Y0 - b, a.X0Y1 - b, a.X1Y0 - b, a.X1Y1 - b);

        public static bool operator !=(SampledData2F a, SampledData2F b) => (a.X0Y0 != b.X0Y0) && (a.X0Y1 != b.X0Y1) && (a.X1Y0 != b.X1Y0) && (a.X1Y1 != b.X1Y1);

        // public static SampledData2i operator -(SampledData2i a, int b)
        // {
        //     return new SampledData2i(a.x0y0 - b, a.x0y1 - b, a.x1y0 - b, a.x1y1 - b);
        // }
        public static SampledData2F operator *(SampledData2F a, int b) => new SampledData2F(a.X0Y0 * b, a.X0Y1 * b, a.X1Y0 * b, a.X1Y1 * b);

        public static SampledData2F operator +(SampledData2F a, float b) => new SampledData2F(a.X0Y0 + b, a.X0Y1 + b, a.X1Y0 + b, a.X1Y1 + b);

        public static bool operator ==(SampledData2F a, SampledData2F b) => (a.X0Y0 == b.X0Y0) && (a.X0Y1 == b.X0Y1) && (a.X1Y0 == b.X1Y0) && (a.X1Y1 == b.X1Y1);

        public SampledData2F ClipMin(float clipValue) => new SampledData2F(Math.Max(this.X0Y0, clipValue), Math.Max(this.X0Y1, clipValue), Math.Max(this.X1Y0, clipValue), Math.Max(this.X1Y1, clipValue));

        public override bool Equals(object obj)
        {
            if (obj is SampledData2F)
            {
                var that = (SampledData2F)obj;
                return this.Equals(that);
            }

            return false;
        }

        public bool Equals(SampledData2F that) => this == that;

        public override int GetHashCode()
        {
            unchecked
            {
                return this.X0Y0.GetHashCode() + (this.X0Y1.GetHashCode() * 7) + (this.X1Y0.GetHashCode() * 17) + (this.X1Y1.GetHashCode() * 31);
            }
        }

        public float GetIndexedValueCw(int index)
        {
            switch (index % 4)
            {
                case 0:
                    return this.X0Y0;

                case 1:
                    return this.X0Y1;

                case 2:
                    return this.X1Y1;

                case 3:
                    return this.X1Y0;

                default:
                    throw new InvalidOperationException();
            }
        }

        public SampledData2F GetRotated(int offset) => new SampledData2F(this.GetIndexedValueCw(0 + offset), this.GetIndexedValueCw(1 + offset), this.GetIndexedValueCw(3 + offset), this.GetIndexedValueCw(2 + offset));

        public override string ToString() => string.Format("x0y0:{0} , x0y1:{1} , x1y0:{2} , x1y1:{3}", this.X0Y0, this.X0Y1, this.X1Y0, this.X1Y1);

        public static float Dif(SampledData2F a, SampledData2F b) => Math.Abs(a.X0Y0 - b.X0Y0) + Math.Abs(a.X0Y1 - b.X0Y1) + Math.Abs(a.X1Y0 - b.X1Y0) + Math.Abs(a.X1Y1 - b.X1Y1);

        public static IEnumerable<SampledData2F> GenerateAllValuesWithHoles(Area1f range, float step)
        {
            var stepCount = (range.Size / step).RoundToVector1i() + 1;
            step = range.Size / (stepCount - 1);

            var results = new List<SampledData2F>();
            foreach (var x0Y0 in GenerateValues(range, stepCount, step))
            {
                foreach (var x0Y1 in GenerateValues(range, stepCount, step))
                {
                    foreach (var x1Y0 in GenerateValues(range, stepCount, step))
                    {
                        foreach (var x1Y1 in GenerateValues(range, stepCount, step))
                        {
                            results.Add(new SampledData2F(x0Y0, x0Y1, x1Y0, x1Y1));
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

    public static class SampledData2FExtensions
    {
        public static SampledData2I ToSampledData2I(this SampledData2F data) => new SampledData2I((int)data.X0Y0, (int)data.X0Y1, (int)data.X1Y0, (int)data.X1Y1);
    }
}
