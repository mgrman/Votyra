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

namespace Votyra.Tests.TerrainMesher3bTests
{
    public class WallTests
    {


        [Test]
        public void WallX0Test()
        {
            var cube = @"
              1-----0
             /|    /|
            1-+---0 |
            | 1---+-0
            |/    |/
            1-----0
            ";
            var triangles = Evaluate(cube);

            AssertContainsQuad(triangles, cube, x0y0z0, x0y1z0, x0y0z1, x0y1z1);
        }

        [Test]
        public void WallX1Test()
        {
            var cube = @"
              0-----1
             /|    /|
            0-+---1 |
            | 0---+-1
            |/    |/
            0-----1
            ";
            var triangles = Evaluate(cube);

            AssertContainsQuad(triangles, cube, x1y0z0, x1y1z0, x1y0z1, x1y1z1);
        }

        [Test]
        public void WallY0Test()
        {
            var cube = @"
              0-----0
             /|    /|
            1-+---1 |
            | 0---+-0
            |/    |/
            1-----1
            ";
            var triangles = Evaluate(cube);

            AssertContainsQuad(triangles, cube, x0y0z0, x0y0z1, x1y0z0, x1y0z1);
        }

        [Test]
        public void WallY1Test()
        {
            var cube = @"
              1-----1
             /|    /|
            0-+---0 |
            | 1---+-1
            |/    |/
            0-----0
            ";
            var triangles = Evaluate(cube);

            AssertContainsQuad(triangles, cube, x0y1z0, x0y1z1, x1y1z0, x1y1z1);
        }

        [Test]
        public void WallZ0Test()
        {
            var cube = @"
              0-----0
             /|    /|
            0-+---0 |
            | 1---+-1
            |/    |/
            1-----1
            ";
            var triangles = Evaluate(cube);

            AssertContainsQuad(triangles, cube, x0y0z0, x0y1z0, x1y0z0, x1y1z0);
        }

        [Test]
        public void WallZ1Test()
        {
            var cube = @"
              1-----1
             /|    /|
            1-+---1 |
            | 0---+-0
            |/    |/
            0-----0
            ";
            var triangles = Evaluate(cube);

            AssertContainsQuad(triangles, cube, x0y0z1, x0y1z1, x1y0z1, x1y1z1);
        }
    }
}