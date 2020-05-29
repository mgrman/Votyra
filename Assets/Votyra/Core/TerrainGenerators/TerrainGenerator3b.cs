using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Profiling;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators
{
    public class TerrainGenerator3b : ITerrainGenerator3b
    {
        private readonly Vector3i _cellInGroupCount;
        private readonly ITerrainMesher3b _mesher;
        private readonly IProfiler _profiler;

        public TerrainGenerator3b(ITerrainMesher3b mesher, ITerrainConfig terrainConfig, IProfiler profiler)
        {
            this._mesher = mesher;
            this._profiler = profiler;
            this._cellInGroupCount = terrainConfig.CellInGroupCount;
        }

        public void Generate(Vector3i group, IImage3b image, IGeneralMesh pooledTerrainMesh)
        {
            using (this._profiler.Start("init"))
            {
                this._mesher.Initialize(image);
            }

            using (this._profiler.Start("Other"))
            {
                this._mesher.InitializeGroup(group, pooledTerrainMesh);
            }

            var tempQualifier = this._cellInGroupCount.ToRange3i();
            var min = tempQualifier.Min;
            for (var ix = 0; ix < tempQualifier.Size.X; ix++)
            {
                for (var iy = 0; iy < tempQualifier.Size.Y; iy++)
                {
                    for (var iz = 0; iz < tempQualifier.Size.Z; iz++)
                    {
                        var cellInGroup = new Vector3i(ix, iy, iz) + min;
                        using (this._profiler.Start("TerrainMesher.AddCell()"))
                        {
                            this._mesher.AddCell(cellInGroup);
                        }
                    }
                }
            }
        }
    }
}
