using NUnit.Framework;
using static Votyra.Cubical.Tests.Editor.TerrainMesher3bTests.TerrainMesher3bUtils;

namespace Votyra.Cubical.Tests.Editor.TerrainMesher3bTests
{
    public class EmptyTests
    {
        [Test,]
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

        [Test,]
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
