using System;
using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Logging;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct Triangle3
    {
        private static readonly Lazy<IThreadSafeLogger> _logger = new Lazy<IThreadSafeLogger>(() => LoggerFactoryExtensions.factory.Create<Triangle3>());
        public static IThreadSafeLogger Logger => _logger.Value;

        public readonly Vector3f a;
        public readonly Vector3f b;
        public readonly Vector3f c;

        public Triangle3(Vector3f a, Vector3f b, Vector3f c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }

        public IEnumerable<Vector3f> Points
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

        public float DotWithObserver(Vector3f observer)
        {
            var center = (a + b + c) / 3f;
            var normal = Vector3f.Cross(b - a, c - a);
            return Vector3f.Dot(observer - center, normal);
        }

        public bool IsCCW(Vector3f observer)
        {
            var dot = DotWithObserver(observer);
            if (dot == 0f)
            {
                _logger.Value.LogError("Wrong observer!");
            }
            return dot >= 0;
        }

        public Triangle3 EnsureCCW(Vector3f observer)
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
        private static readonly Vector3f CenterZeroCell = new Vector3f(0.5f, 0.5f, 0.5f);

        public static IEnumerable<Triangle3> EnsureCCW(this IEnumerable<Triangle3> triangles, SampledData3b data)
        {
            var observer = data.GetPointsWithValue(false)
                .Select(o => o.ToVector3f())
                .Average();
            return EnsureCCW(triangles, data, observer);
        }

        public static IEnumerable<Triangle3> EnsureCCW(this IEnumerable<Triangle3> triangles, SampledData3b data, Vector3f observer)
        {
            observer = observer == CenterZeroCell ? CenterZeroCell : (CenterZeroCell + (observer - CenterZeroCell) * 10);

            return triangles.Select(t =>
            {
                if (t.DotWithObserver(observer) == 0f)
                {
                    Triangle3.Logger.LogError($"Zero dot:\r\n{data.ToCubeString()}");
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