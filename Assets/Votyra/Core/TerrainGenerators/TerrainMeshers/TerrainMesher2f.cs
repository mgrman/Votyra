using System;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public class TerrainMesher2f : ITerrainMesher2f
    {
        private Vector2i _cellInGroupCount;


        public TerrainMesher2f(ITerrainConfig terrainConfig)
        {
            _cellInGroupCount = terrainConfig.CellInGroupCount.XY();
        }

        public void GetResultingMesh(ITerrainMesh mesh, Vector2i group, IImage2f image, IMask2e mask)
        {
            var groupPosition = _cellInGroupCount * group;
            for (var ix = 0; ix < _cellInGroupCount.X; ix++)
            {
                for (var iy = 0; iy < _cellInGroupCount.Y; iy++)
                {
                    var cellInGroup = new Vector2i(ix, iy);
                    var cell = cellInGroup + groupPosition;
                    var data = image.SampleCell(cell);
                    var maskData = mask.SampleCell(cell);
                    mesh.AddQuad(cell.ToVector2f(), data, maskData);
                }
            }
        }
    }
}