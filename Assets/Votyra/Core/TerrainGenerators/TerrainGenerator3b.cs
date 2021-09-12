using System;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Profiling;
using Votyra.Core.TerrainGenerators.TerrainMeshers;

namespace Votyra.Core.TerrainGenerators
{
    public class TerrainGenerator3b : ITerrainGenerator3b
    {
        private readonly Vector3i _cellInGroupCount;
        
        private readonly ITerrainMesher3b _mesher;
        
        private readonly IProfiler _profiler;

        public TerrainGenerator3b(ITerrainMesher3b mesher, ITerrainConfig terrainConfig, IProfiler profiler)
        {
            _mesher = mesher;
            _profiler = profiler;
            _cellInGroupCount = terrainConfig.CellInGroupCount;
        }

        public void Generate(Vector3i group, IImage3b image,IPooledTerrainMesh pooledTerrainMesh)
        {
            using (_profiler.Start("init"))
            {
                _mesher.Initialize(image);
            }

            using (_profiler.Start("Other"))
            {
                _mesher.InitializeGroup(group,pooledTerrainMesh);
            }

            Range3i tempQualifier = _cellInGroupCount.ToRange3i();
            var min = tempQualifier.Min;
            for (var ix = 0; ix < tempQualifier.Size.X; ix++)
            {
                for (var iy = 0; iy < tempQualifier.Size.Y; iy++)
                {
                    for (var iz = 0; iz < tempQualifier.Size.Z; iz++)
                    {
                        var
                        cellInGroup=new Vector3i(ix, iy, iz)+min;
                        using (_profiler.Start("TerrainMesher.AddCell()"))
                        {
                            _mesher.AddCell(cellInGroup);
                        }
                    }
                }
            }
        }
    }
}