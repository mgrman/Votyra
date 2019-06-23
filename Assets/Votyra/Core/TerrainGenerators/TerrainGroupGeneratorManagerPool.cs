using System;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainGenerators.TerrainMeshers;

namespace Votyra.Core
{
    public class TerrainGroupGeneratorManagerPool : Pool<ITerrainGroupGeneratorManager2i>, ITerrainGroupGeneratorManagerPool
    {
        public TerrainGroupGeneratorManagerPool(ITerrainConfig terrainConfig, IInterpolationConfig interpolationConfig, ITerrainMesh2iPool terrainMeshPool, ITerrainMesher2f terrainMesher)
            : base(factory(terrainConfig, interpolationConfig, terrainMeshPool, terrainMesher))
        {
        }

        private static Func<ITerrainGroupGeneratorManager2i> factory(ITerrainConfig terrainConfig, IInterpolationConfig interpolationConfig, ITerrainMesh2iPool terrainMeshPool, ITerrainMesher2f terrainMesher)
        {
            var cellInGroupCount = terrainConfig.CellInGroupCount.XY();

            Func<ITerrainGroupGeneratorManager2i> managerFactory= () => new TerrainGroupGeneratorManager2i(cellInGroupCount, terrainMeshPool.GetRaw(), terrainMesher.GetResultingMesh);

            return managerFactory;
        }
    }
}