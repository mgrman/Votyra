using System;
using System.Threading;
using Votyra.Core.Images;
using Votyra.Core.MeshUpdaters;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Unity;

namespace Votyra.Core
{
    public class TerrainGroupGeneratorManagerPool : Pool<ITerrainGroupGeneratorManager2i>, ITerrainGroupGeneratorManagerPool
    {
        public TerrainGroupGeneratorManagerPool(ITerrainConfig terrainConfig, IInterpolationConfig interpolationConfig, ITerrainGameObjectPool gameObjectPool, ITerrainMeshPool terrainMeshPool, ITerrainMesher2f terrainMesher)
            : base(factory(terrainConfig, interpolationConfig, gameObjectPool, terrainMeshPool, terrainMesher))
        {
        }

        private static Func<ITerrainGroupGeneratorManager2i> factory(ITerrainConfig terrainConfig, IInterpolationConfig interpolationConfig, ITerrainGameObjectPool gameObjectPool, ITerrainMeshPool terrainMeshPool, ITerrainMesher2f terrainMesher)
        {
            var cellInGroupCount = terrainConfig.CellInGroupCount.XY();

            var triangleCount = (uint) (cellInGroupCount.AreaSum() * 2 * interpolationConfig.MeshSubdivision * interpolationConfig.MeshSubdivision);

            Func<ITerrainGroupGeneratorManager2i> managerFactory;
            if (terrainConfig.Async)
                managerFactory = () => new AsyncTerrainGroupGeneratorManager2i(cellInGroupCount, gameObjectPool.GetRaw(), terrainMeshPool.GetRaw(triangleCount), terrainMesher.GetResultingMesh);
            else
                managerFactory = () => new SyncTerrainGroupGeneratorManager2i(cellInGroupCount, gameObjectPool.GetRaw(), terrainMeshPool.GetRaw(triangleCount), terrainMesher.GetResultingMesh);

            return managerFactory;
        }
    }
}