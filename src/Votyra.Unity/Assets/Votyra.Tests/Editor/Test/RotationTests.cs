using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Moq;
using NUnit.Framework;
using UnityEngine;
using Votyra.Images;
using Votyra.ImageSamplers;
using Votyra.Models;
using Votyra.TerrainGenerators.TerrainMeshers;
using Votyra.TerrainGenerators.TerrainMeshers.TerrainMeshes;
using Votyra.Unity.Assets.Votyra.Pooling;
using static Votyra.Tests.TerrainMesher3bUtils;

namespace Votyra.Tests
{
    public class RotationTests
    {
        [Test]
        public void x0y0z0()
        {
            var initialCubeString = @"
              0-----0
             /|    /|
            1-+---0 |
            | 0---+-0
            |/    |/
            1-----1
            ";
            var expectedCubeString = @"
              0-----0
             /|    /|
            1-+---0 |
            | 0---+-0
            |/    |/
            1-----1
            ";
            var initialCube = SampledData3b.ParseCube(initialCubeString);
            var expectedCube = SampledData3b.ParseCube(expectedCubeString);

            var rotatedCube = initialCube.GetRotated(new Vector3i(0, 0, 0), false);

            Assert.AreEqual(expectedCube, rotatedCube);
        }

        [Test]
        public void xy0z1()
        {
            var initialCubeString = @"
              0-----0
             /|    /|
            1-+---0 |
            | 0---+-0
            |/    |/
            1-----1
            ";
            var expectedCubeString = @"
              1-----0
             /|    /|
            0-+---0 |
            | 1---+-0
            |/    |/
            1-----0
            ";
            var initialCube = SampledData3b.ParseCube(initialCubeString);
            var expectedCube = SampledData3b.ParseCube(expectedCubeString);

            var rotatedCube = initialCube.GetRotated(new Vector3i(0, 0, 1), false);

            Assert.AreEqual(expectedCube, rotatedCube);
        }

        [Test]
        public void xy0z2()
        {
            var initialCubeString = @"
              0-----0
             /|    /|
            1-+---0 |
            | 0---+-0
            |/    |/
            1-----1
            ";
            var expectedCubeString = @"
              0-----1
             /|    /|
            0-+---0 |
            | 1---+-1
            |/    |/
            0-----0
            ";
            var initialCube = SampledData3b.ParseCube(initialCubeString);
            var expectedCube = SampledData3b.ParseCube(expectedCubeString);

            var rotatedCube = initialCube.GetRotated(new Vector3i(0, 0, 2), false);

            Assert.AreEqual(expectedCube, rotatedCube);
        }

        [Test]
        public void xy0z3()
        {
            var initialCubeString = @"
              0-----0
             /|    /|
            1-+---0 |
            | 0---+-0
            |/    |/
            1-----1
            ";
            var expectedCubeString = @"
              0-----0
             /|    /|
            0-+---1 |
            | 0---+-1
            |/    |/
            0-----1
            ";
            var initialCube = SampledData3b.ParseCube(initialCubeString);
            var expectedCube = SampledData3b.ParseCube(expectedCubeString);

            var rotatedCube = initialCube.GetRotated(new Vector3i(0, 0, 3), false);

            Assert.AreEqual(expectedCube, rotatedCube);
        }

        [Test]
        public void xy0z0Inv()
        {
            var initialCubeString = @"
              0-----0
             /|    /|
            1-+---0 |
            | 0---+-0
            |/    |/
            1-----1
            ";
            var expectedCubeString = @"
              0-----0
             /|    /|
            1-+---1 |
            | 0---+-0
            |/    |/
            1-----0
            ";
            var initialCube = SampledData3b.ParseCube(initialCubeString);
            var expectedCube = SampledData3b.ParseCube(expectedCubeString);

            var rotatedCube = initialCube.GetRotated(new Vector3i(0, 0, 0), true);

            Assert.AreEqual(expectedCube, rotatedCube);
        }
    }
}