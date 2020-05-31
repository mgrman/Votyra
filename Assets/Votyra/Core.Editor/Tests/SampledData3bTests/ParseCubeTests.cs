using System.Linq;
using NUnit.Framework;
using Votyra.Core.Models;

namespace Votyra.Cubical.Tests.Editor.SampledData3bTests
{
    public class ParseCubeTests
    {
        [Test]
        public void TestCase_FromStringToString()
        {
            var cubeString = @"
              0-----0
             /|    /|
            0-+---0 |
            | 0---+-0
            |/    |/
            1-----0
            ";

            var cube = SampledData3b.ParseCube(cubeString);

            Assert.AreEqual(CleanString(cubeString), CleanString(cube.ToCubeString()));
        }

        private static string CleanString(string cubeString)
        {
            var cleanLines = cubeString.Split('\n')
                .Select(o => o.Trim());

            return string.Join("\n", cleanLines);
        }

        [Test]
        public void TestCase_x0y0z0()
        {
            var cubeString = @"
              0-----0
             /|    /|
            0-+---0 |
            | 0---+-0
            |/    |/
            1-----0
            ";

            var cube = SampledData3b.ParseCube(cubeString);

            Assert.IsTrue(cube.DataX0Y0Z0);
            Assert.IsFalse(cube.DataX0Y0Z1);
            Assert.IsFalse(cube.DataX0Y1Z0);
            Assert.IsFalse(cube.DataX0Y1Z1);
            Assert.IsFalse(cube.DataX1Y0Z0);
            Assert.IsFalse(cube.DataX1Y0Z1);
            Assert.IsFalse(cube.DataX1Y1Z0);
            Assert.IsFalse(cube.DataX1Y1Z1);
        }

        [Test]
        public void TestCase_x0y0z1()
        {
            var cubeString = @"
              0-----0
             /|    /|
            1-+---0 |
            | 0---+-0
            |/    |/
            0-----0
            ";

            var cube = SampledData3b.ParseCube(cubeString);

            Assert.IsFalse(cube.DataX0Y0Z0);
            Assert.IsTrue(cube.DataX0Y0Z1);
            Assert.IsFalse(cube.DataX0Y1Z0);
            Assert.IsFalse(cube.DataX0Y1Z1);
            Assert.IsFalse(cube.DataX1Y0Z0);
            Assert.IsFalse(cube.DataX1Y0Z1);
            Assert.IsFalse(cube.DataX1Y1Z0);
            Assert.IsFalse(cube.DataX1Y1Z1);
        }

        [Test]
        public void TestCase_x0y1z0()
        {
            var cubeString = @"
              0-----0
             /|    /|
            0-+---0 |
            | 1---+-0
            |/    |/
            0-----0
            ";

            var cube = SampledData3b.ParseCube(cubeString);

            Assert.IsFalse(cube.DataX0Y0Z0);
            Assert.IsFalse(cube.DataX0Y0Z1);
            Assert.IsTrue(cube.DataX0Y1Z0);
            Assert.IsFalse(cube.DataX0Y1Z1);
            Assert.IsFalse(cube.DataX1Y0Z0);
            Assert.IsFalse(cube.DataX1Y0Z1);
            Assert.IsFalse(cube.DataX1Y1Z0);
            Assert.IsFalse(cube.DataX1Y1Z1);
        }

        [Test]
        public void TestCase_x0y1z1()
        {
            var cubeString = @"
              1-----0
             /|    /|
            0-+---0 |
            | 0---+-0
            |/    |/
            0-----0
            ";

            var cube = SampledData3b.ParseCube(cubeString);

            Assert.IsFalse(cube.DataX0Y0Z0);
            Assert.IsFalse(cube.DataX0Y0Z1);
            Assert.IsFalse(cube.DataX0Y1Z0);
            Assert.IsTrue(cube.DataX0Y1Z1);
            Assert.IsFalse(cube.DataX1Y0Z0);
            Assert.IsFalse(cube.DataX1Y0Z1);
            Assert.IsFalse(cube.DataX1Y1Z0);
            Assert.IsFalse(cube.DataX1Y1Z1);
        }

        [Test]
        public void TestCase_x1y0z0()
        {
            var cubeString = @"
              0-----0
             /|    /|
            0-+---0 |
            | 0---+-0
            |/    |/
            0-----1
            ";

            var cube = SampledData3b.ParseCube(cubeString);

            Assert.IsFalse(cube.DataX0Y0Z0);
            Assert.IsFalse(cube.DataX0Y0Z1);
            Assert.IsFalse(cube.DataX0Y1Z0);
            Assert.IsFalse(cube.DataX0Y1Z1);
            Assert.IsTrue(cube.DataX1Y0Z0);
            Assert.IsFalse(cube.DataX1Y0Z1);
            Assert.IsFalse(cube.DataX1Y1Z0);
            Assert.IsFalse(cube.DataX1Y1Z1);
        }

        [Test]
        public void TestCase_x1y0z1()
        {
            var cubeString = @"
              0-----0
             /|    /|
            0-+---1 |
            | 0---+-0
            |/    |/
            0-----0
            ";

            var cube = SampledData3b.ParseCube(cubeString);

            Assert.IsFalse(cube.DataX0Y0Z0);
            Assert.IsFalse(cube.DataX0Y0Z1);
            Assert.IsFalse(cube.DataX0Y1Z0);
            Assert.IsFalse(cube.DataX0Y1Z1);
            Assert.IsFalse(cube.DataX1Y0Z0);
            Assert.IsTrue(cube.DataX1Y0Z1);
            Assert.IsFalse(cube.DataX1Y1Z0);
            Assert.IsFalse(cube.DataX1Y1Z1);
        }

        [Test]
        public void TestCase_x1y1z0()
        {
            var cubeString = @"
              0-----0
             /|    /|
            0-+---0 |
            | 0---+-1
            |/    |/
            0-----0
            ";

            var cube = SampledData3b.ParseCube(cubeString);

            Assert.IsFalse(cube.DataX0Y0Z0);
            Assert.IsFalse(cube.DataX0Y0Z1);
            Assert.IsFalse(cube.DataX0Y1Z0);
            Assert.IsFalse(cube.DataX0Y1Z1);
            Assert.IsFalse(cube.DataX1Y0Z0);
            Assert.IsFalse(cube.DataX1Y0Z1);
            Assert.IsTrue(cube.DataX1Y1Z0);
            Assert.IsFalse(cube.DataX1Y1Z1);
        }

        [Test]
        public void TestCase_x1y1z1()
        {
            var cubeString = @"
              0-----1
             /|    /|
            0-+---0 |
            | 0---+-0
            |/    |/
            0-----0
            ";

            var cube = SampledData3b.ParseCube(cubeString);

            Assert.IsFalse(cube.DataX0Y0Z0);
            Assert.IsFalse(cube.DataX0Y0Z1);
            Assert.IsFalse(cube.DataX0Y1Z0);
            Assert.IsFalse(cube.DataX0Y1Z1);
            Assert.IsFalse(cube.DataX1Y0Z0);
            Assert.IsFalse(cube.DataX1Y0Z1);
            Assert.IsFalse(cube.DataX1Y1Z0);
            Assert.IsTrue(cube.DataX1Y1Z1);
        }
    }
}
