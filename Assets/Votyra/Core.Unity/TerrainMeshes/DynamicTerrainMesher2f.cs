using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public static class DynamicTerrainMesher2f
    {
        public static void GetResultingMesh(ITerrainMesh mesh, Vector2i group, Vector2i cellInGroupCount, IImage2f image, IMask2e mask)
        {
            var groupPosition = cellInGroupCount * group;

            var samples = image.SampleArea(Range2i.FromMinAndSize(groupPosition, cellInGroupCount + Vector2i.One));

            for (var ix = 0; ix < cellInGroupCount.X; ix++)
            {
                for (var iy = 0; iy < cellInGroupCount.Y; iy++)
                {
                    var cell = new Vector2i(ix, iy) + groupPosition;
                    var x0y0 = new Vector3f(cell.X + 0, cell.Y + 0, samples[ix + 0, iy + 0]);
                    var x0y1 = new Vector3f(cell.X + 0, cell.Y + 1, samples[ix + 0, iy + 1]);
                    var x1y0 = new Vector3f(cell.X + 1, cell.Y + 0, samples[ix + 1, iy + 0]);
                    var x1y1 = new Vector3f(cell.X + 1, cell.Y + 1, samples[ix + 1, iy + 1]);
                    mesh.AddQuad(x0y0, x0y1, x1y0, x1y1);
                }
            }

            samples.Dispose();
        }
    }
}