using System;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.Editor.Tests
{
    [TestFixture]
    public class LiangBarskyClipperTest
    {
        [Test]
        public void Test1()
        {
            Line2f result;
            Side side;
            var fromI = 1;
            var toI = 100000;
            var countI = toI - fromI;
            var areas = Enumerable.Range(fromI, countI)
                .Select(i => Area2f.FromMinAndMax(new Vector2f(i, i), new Vector2f(i * 2, i * 2)))
                .ToArray();
            var lines = Enumerable.Range(fromI, countI)
                .Select(i => new Ray2f(new Vector2f(i, i + 0.5f), new Vector2f(1, 0)))
                .ToArray();
            var results = Enumerable.Range(fromI, countI)
                .Select(i => new Vector2f(i * 2f, i + 0.5f))
                .ToArray();

            var startLiangBarskyClipper = DateTime.Now;
            for (int i = 0; i < countI; i++)
            {
                var success = LiangBarskyClipper.Compute(areas[i], lines[i], out result, out side);

                UnityEngine.Assertions.Assert.AreEqual(Side.X1, side, $"Problem in Side of LiangBarskyClipper test {i}");
                UnityEngine.Assertions.Assert.AreApproximatelyEqual(lines[i]
                        .Origin.X,
                    result.From.X,
                    0.0001f,
                    $"Problem in LiangBarskyClipper test {i}");
                UnityEngine.Assertions.Assert.AreApproximatelyEqual(lines[i]
                        .Origin.Y,
                    result.From.Y,
                    0.0001f,
                    $"Problem in LiangBarskyClipper test {i}");
                UnityEngine.Assertions.Assert.AreApproximatelyEqual(results[i]
                        .X,
                    result.To.X,
                    0.0001f,
                    $"Problem in LiangBarskyClipper test {i}");
                UnityEngine.Assertions.Assert.AreApproximatelyEqual(results[i]
                        .Y,
                    result.To.Y,
                    0.0001f,
                    $"Problem in LiangBarskyClipper test {i}");
            }

            var endLiangBarskyClipper = DateTime.Now;
            var timeLiangBarskyClipper = (endLiangBarskyClipper - startLiangBarskyClipper).TotalMilliseconds;
            Debug.Log($"timeLiangBarskyClipper:{timeLiangBarskyClipper}");

            var startLineSegmentClipper = DateTime.Now;
            for (int i = 0; i < countI; i++)
            {
                var success = LineSegmentClipper.Compute(areas[i], lines[i], Side.X1, out result, out side);

                UnityEngine.Assertions.Assert.AreEqual(Side.X1,side,$"Problem in Side of LineSegmentClipper test {i}");
                UnityEngine.Assertions.Assert.AreApproximatelyEqual(lines[i]
                        .Origin.X,
                    result.From.X,
                    0.0001f,
                    $"Problem in LineSegmentClipper test {i}");
                UnityEngine.Assertions.Assert.AreApproximatelyEqual(lines[i]
                        .Origin.Y,
                    result.From.Y,
                    0.0001f,
                    $"Problem in LineSegmentClipper test {i}");
                UnityEngine.Assertions.Assert.AreApproximatelyEqual(results[i]
                        .X,
                    result.To.X,
                    0.0001f,
                    $"Problem in LineSegmentClipper test {i}");
                UnityEngine.Assertions.Assert.AreApproximatelyEqual(results[i]
                        .Y,
                    result.To.Y,
                    0.0001f,
                    $"Problem in LineSegmentClipper test {i}");
            }

            var endLineSegmentClipper = DateTime.Now;
            var timeLineSegmentClipper = (endLineSegmentClipper - startLineSegmentClipper).TotalMilliseconds;
            Debug.Log($"timeLineSegmentClipper:{timeLineSegmentClipper}");
        }

        [Test]
        public void Test2()
        {
            Line2f result;
            Side side;

            var fromI = 1;
            var toI = 100000;
            var countI = toI - fromI;
            var areas = Enumerable.Range(fromI, countI)
                .Select(i => Area2f.FromMinAndMax(new Vector2f(i, i), new Vector2f(i * 2, i * 2)))
                .ToArray();
            var lines = Enumerable.Range(fromI, countI)
                .Select(i => new Ray2f(new Vector2f(i, i + 0.5f), new Vector2f(1, 0)))
                .ToArray();

            var startLiangBarskyClipper = DateTime.Now;
            for (int i = 0; i < countI; i++)
            {
                var success = LiangBarskyClipper.Compute(areas[i], lines[i], out result, out side);
            }

            var endLiangBarskyClipper = DateTime.Now;
            var timeLiangBarskyClipper = (endLiangBarskyClipper - startLiangBarskyClipper).TotalMilliseconds;
            Debug.Log($"timeLiangBarskyClipper:{timeLiangBarskyClipper}");

            var startLineSegmentClipper = DateTime.Now;
            for (int i = 0; i < countI; i++)
            {
                var success = LineSegmentClipper.Compute(areas[i], lines[i], Side.X1, out result, out side);
            }

            var endLineSegmentClipper = DateTime.Now;
            var timeLineSegmentClipper = (endLineSegmentClipper - startLineSegmentClipper).TotalMilliseconds;
            Debug.Log($"timeLineSegmentClipper:{timeLineSegmentClipper}");
        }
    }
}