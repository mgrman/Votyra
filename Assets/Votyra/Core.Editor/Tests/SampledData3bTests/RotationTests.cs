using NUnit.Framework;
using Votyra.Core.Models;

namespace Votyra.Cubical.Tests.Editor.SampledData3bTests
{
    public class RotationTests
    {
        [Test,]
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

        [Test,]
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

        [Test,]
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

        [Test,]
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

        [Test,]
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
