using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public class TerrainMesher2f : ITerrainMesher2f
    {
        private readonly Vector2i cellInGroupCount;

        public TerrainMesher2f(ITerrainConfig terrainConfig)
        {
            this.cellInGroupCount = terrainConfig.CellInGroupCount.XY();
        }

        public void GetResultingMesh(ITerrainMesh2f mesh, Vector2i group, IImage2f image)
        {
            var range = Area3f.FromMinAndSize((group * this.cellInGroupCount).ToVector3f(image.RangeZ.Min), this.cellInGroupCount.ToVector3f(image.RangeZ.Size));
            mesh.Reset(range);

            var groupPosition = this.cellInGroupCount * group;
            for (var ix = 0; ix < this.cellInGroupCount.X; ix++)
            {
                for (var iy = 0; iy < this.cellInGroupCount.Y; iy++)
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
