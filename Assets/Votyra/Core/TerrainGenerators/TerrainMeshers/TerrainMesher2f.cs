using System;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public class TerrainMesher2f:ITerrainMesher
    {
        private readonly Vector2i cellInGroupCount;

        public TerrainMesher2f(ITerrainConfig terrainConfig)
        {
            cellInGroupCount = terrainConfig.CellInGroupCount.XY;
        }
        
        public void UpdateMesh(ITerrainMesh mesh, Vector2i group, IImage2f image)
        {
            var groupPosition = cellInGroupCount * group;
            for (var ix = 0; ix < cellInGroupCount.X; ix++)
            {
                for (var iy = 0; iy < cellInGroupCount.Y; iy++)
                {
                    var cellInGroup=new Vector2i(ix, iy);
                    var cell = cellInGroup + groupPosition;
                    var data = image.SampleCell(cell);
                    mesh.AddQuad(cell.ToVector2f(), data);
                }
            }
        }
    }
}