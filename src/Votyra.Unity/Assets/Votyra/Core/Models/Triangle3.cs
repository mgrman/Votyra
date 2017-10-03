using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Votyra.Utils;

namespace Votyra.Models
{
    public struct Triangle3
    {
        public readonly Vector3 a;
        public readonly Vector3 b;
        public readonly Vector3 c;

        public Triangle3(Vector3 a, Vector3 b, Vector3 c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }


        public IEnumerable<Vector3> Points
        {
            get
            {

                yield return a;
                yield return b;
                yield return c;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Triangle3)
            {
                var that = (Triangle3)obj;
                return this.a == that.a && this.b == that.b && this.c == that.c;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return a.GetHashCode() + b.GetHashCode() * 3 + c.GetHashCode() * 7;
        }

        public override string ToString()
        {
            return $"{a},{b},{c}";
        }

        public float DotWithObserver(Vector3 observer)
        {
            var center = (a + b + c) / 3f;
            var normal = Vector3.Cross(b - a, c - a);
            return Vector3.Dot(observer - center, normal);
        }
        public bool IsCCW(Vector3 observer)
        {
            var dot = DotWithObserver(observer);
            if (dot == 0f)
            {
                Debug.LogError("Wrong observer!");
            }
            return dot >= 0;
        }

        public Triangle3 EnsureCCW(Vector3 observer)
        {
            if (IsCCW(observer))
            {
                return this;
            }
            else
            {
                return GetReversedOrder();
            }
        }
        public Triangle3 GetReversedOrder()
        {

            return new Triangle3(a, c, b);

        }

        public static readonly IEqualityComparer<Triangle3> OrderInvariantComparer = new TriangleInvariantComparer();

        private class TriangleInvariantComparer : IEqualityComparer<Triangle3>
        {
            public TriangleInvariantComparer()
            {

            }

            public bool Equals(Triangle3 x, Triangle3 y)
            {
                foreach (var xP in x.Points)
                {
                    if (!y.Points.Any(yP => (xP - yP).sqrMagnitude < 0.1f))
                    {
                        return false;
                    }
                }
                return true;
            }

            public int GetHashCode(Triangle3 obj)
            {
                return 0;
            }
        }
    }
    public static class Triangle3Extensions
    {

        private static readonly Vector3 CenterZeroCell = new Vector3(0.5f, 0.5f, 0.5f);

        public static IEnumerable<Triangle3> EnsureCCW(this IEnumerable<Triangle3> triangles, SampledData3b data)
        {
            var observer = data.GetPointsWithValue(false)
                .Select(o => o.ToVector3())
                .Average();
            return EnsureCCW(triangles, data, observer);
        }

        public static IEnumerable<Triangle3> EnsureCCW(this IEnumerable<Triangle3> triangles, SampledData3b data, Vector3 observer)
        {
            observer = observer == CenterZeroCell ? CenterZeroCell : (CenterZeroCell + (observer - CenterZeroCell) * 10);

            return triangles.Select(t =>
            {
                if (t.DotWithObserver(observer) == 0f)
                {
                    Debug.LogError($"Zero dot:\r\n{data.ToCubeString()}");
                }
                return t.EnsureCCW(observer);
            });
        }

        public static IEnumerable<Triangle3> ChangeOrderIfTrue(this IEnumerable<Triangle3> triangles, bool value)
        {
            if (value)
            {
                return triangles.Select(o => o.GetReversedOrder());
            }
            else
            {
                return triangles;
            }
        }
    }
}