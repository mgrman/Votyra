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
    public class NormalTests
    {
        [Test]
        public void XDir()
        {
            var initialCubeString = @"
              1-----0
             /|    /|
            1-+---0 |
            | 1---+-0
            |/    |/
            1-----0
            ";
            var initialCube = SampledData3b.ParseCube(initialCubeString);

            var expectedNormal = new Vector3(1, 0, 0);

            Assert.AreEqual(expectedNormal, initialCube.Normal);
        }

        [Test]
        public void YDir()
        {
            var initialCubeString = @"
              0-----0
             /|    /|
            1-+---1 |
            | 0---+-0
            |/    |/
            1-----1
            ";
            var initialCube = SampledData3b.ParseCube(initialCubeString);

            var expectedNormal = new Vector3(0, 1, 0);

            Assert.AreEqual(expectedNormal, initialCube.Normal);
        }

        [Test]
        public void ZDir()
        {
            var initialCubeString = @"
              0-----0
             /|    /|
            0-+---0 |
            | 1---+-1
            |/    |/
            1-----1
            ";
            var initialCube = SampledData3b.ParseCube(initialCubeString);

            var expectedNormal = new Vector3(0, 0, 1);

            Assert.AreEqual(expectedNormal, initialCube.Normal);
        }

        [Test]
        public void ZDir2()
        {
            var initialCubeString = @"
              0-----0
             /|    /|
            0-+---0 |
            | 2---+-2
            |/    |/
            2-----2
            ";
            var initialCube = SampledData3b.ParseCube(initialCubeString);

            var expectedNormal = new Vector3(0, 0, 1);

            Assert.AreEqual(expectedNormal, initialCube.Normal);
        }

        [Test]
        public void ZDir3()
        {
            var initialCubeString = @"
              1-----1
             /|    /|
            1-+---1 |
            | 2---+-2
            |/    |/
            2-----2
            ";
            var initialCube = SampledData3b.ParseCube(initialCubeString);

            var expectedNormal = new Vector3(0, 0, 1);

            Assert.AreEqual(expectedNormal, initialCube.Normal);
        }
    }
}