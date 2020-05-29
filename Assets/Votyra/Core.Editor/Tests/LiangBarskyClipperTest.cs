using System;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Votyra.Core.Models;
using Votyra.Core.Utils;
using Assert = UnityEngine.Assertions.Assert;

namespace Votyra.Core.Editor.Tests
{
    [TestFixture,]
    public class IntersectionUtilsTest
    {
        [Test,]
        public void LiangBarskyClipper_SimpleLines_ReturnExpectedResult()
        {
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
            for (var i = 0; i < countI; i++)
            {
                var result = IntersectionUtils.LiangBarskyClipper(areas[i], lines[i]);

                Assert.AreApproximatelyEqual(results[i]
                        .X,
                    result.X,
                    0.0001f,
                    $"Problem in LiangBarskyClipper test {i}");
                Assert.AreApproximatelyEqual(results[i]
                        .Y,
                    result.Y,
                    0.0001f,
                    $"Problem in LiangBarskyClipper test {i}");
            }

            var endLiangBarskyClipper = DateTime.Now;
            var timeLiangBarskyClipper = (endLiangBarskyClipper - startLiangBarskyClipper).TotalMilliseconds;
            Debug.Log($"timeLiangBarskyClipper:{timeLiangBarskyClipper}");
        }
    }
}
