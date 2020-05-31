using NUnit.Framework;
using static Votyra.Cubical.Tests.Editor.TerrainMesher3bTests.TerrainMesher3bUtils;

namespace Votyra.Cubical.Tests.Editor.TerrainMesher3bTests
{
    public class SlopeTests
    {
        [Test]
        public void SlopeX0()
        {
            var cube = @"
              1-----0
             /|    /|
            1-+---0 |
            | 1---+-1
            |/    |/
            1-----1
            ";

            var triangles = Evaluate(cube);

            AssertContainsQuad(triangles, cube, Offset_x1y0z0, Offset_x1y1z0, Offset_x0y0z1, Offset_x0y1z1);
        }

        [Test]
        public void SlopeX1()
        {
            var cube = @"
              0-----1
             /|    /|
            0-+---1 |
            | 1---+-1
            |/    |/
            1-----1
            ";

            var triangles = Evaluate(cube);

            AssertContainsQuad(triangles, cube, Offset_x0y0z0, Offset_x0y1z0, Offset_x1y0z1, Offset_x1y1z1);
        }
    }
}
