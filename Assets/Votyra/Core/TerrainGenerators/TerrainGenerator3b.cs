using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Profiling;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators
{
    public class TerrainGenerator3B : ITerrainGenerator3B
    {
        private readonly Vector3i cellInGroupCount;
        private readonly ITerrainMesher3B mesher;
        private readonly IProfiler profiler;

        public TerrainGenerator3B(ITerrainMesher3B mesher, ITerrainConfig terrainConfig, IProfiler profiler)
        {
            this.mesher = mesher;
            this.profiler = profiler;
            this.cellInGroupCount = terrainConfig.CellInGroupCount;
        }

        public void Generate(Vector3i group, IImage3B image, IGeneralMesh pooledTerrainMesh)
        {
            using (this.profiler.Start("init"))
            {
                this.mesher.Initialize(image);
            }

            using (this.profiler.Start("Other"))
            {
                this.mesher.InitializeGroup(group, pooledTerrainMesh);
            }

            var tempQualifier = this.cellInGroupCount.ToRange3i();
            var min = tempQualifier.Min;
            for (var ix = 0; ix < tempQualifier.Size.X; ix++)
            {
                for (var iy = 0; iy < tempQualifier.Size.Y; iy++)
                {
                    for (var iz = 0; iz < tempQualifier.Size.Z; iz++)
                    {
                        var cellInGroup = new Vector3i(ix, iy, iz) + min;
                        using (this.profiler.Start("TerrainMesher.AddCell()"))
                        {
                            this.mesher.AddCell(cellInGroup);
                        }
                    }
                }
            }
        }
    }
}
