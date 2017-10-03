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

namespace Votyra.Cubical.Tests.Editor.TerrainMesher3bTests
{
    public class EmptyTests
    {
        [Test]
        public void EmptyTest()
        {
            var cube = @"
              0-----0
             /|    /|
            0-+---0 |
            | 0---+-0
            |/    |/
            0-----0
            ";
            var triangles = Evaluate(cube);

            Assert.IsEmpty(triangles);
        }

        [Test]
        public void FullTest()
        {
            var cube = @"
              0-----0
             /|    /|
            0-+---0 |
            | 0---+-0
            |/    |/
            0-----0
            ";
            var triangles = Evaluate(cube);

            Assert.IsEmpty(triangles);
        }
    }
}
