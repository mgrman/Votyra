using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public static class TerrainMesher2f
    {
        public static void GetResultingMesh(ITerrainMesh mesh, Vector2i group, Vector2i cellInGroupCount, IImage2f image, IMask2e mask)
        {
            var groupPosition = cellInGroupCount * group;
            cellInGroupCount.ToRange2i()
                .ForeachPointExlusive(cellInGroup =>
                {
                    var cell = cellInGroup + groupPosition;
                    var data = image.SampleCell(cell);
                    var maskData = mask.SampleCell(cell);
                    mesh.AddQuad(cell.ToVector2f(), data, maskData);
                });
        }
    }
}