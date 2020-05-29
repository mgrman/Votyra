using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public class TerrainMesher2f : ITerrainMesher2f
    {
        private readonly Vector2i _cellInGroupCount;

        public TerrainMesher2f(ITerrainConfig terrainConfig)
        {
            _cellInGroupCount = terrainConfig.CellInGroupCount.XY();
        }

        public void GetResultingMesh(ITerrainMesh2f mesh, Vector2i group, IImage2f image)
        {
            var range = Area3f.FromMinAndSize((group * _cellInGroupCount).ToVector3f(image.RangeZ.Min), _cellInGroupCount.ToVector3f(image.RangeZ.Size));
            mesh.Reset(range);

            var groupPosition = _cellInGroupCount * group;
            for (var ix = 0; ix < _cellInGroupCount.X; ix++)
            {
                for (var iy = 0; iy < _cellInGroupCount.Y; iy++)
                {
                    var cellInGroup = new Vector2i(ix, iy);
                    var cell = cellInGroup + groupPosition;
                    var data = image.SampleCell(cell);
                    mesh.AddCell(cellInGroup, Vector2i.Zero, data);
                }
            }
        }
    }
}
