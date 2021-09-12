using System;
using System.Threading;
using UnityEngine;
using Votyra.Core.ImageSamplers;
using Votyra.Core.MeshUpdaters;
using Votyra.Core.Models;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.TerrainMeshes;
using Zenject;

namespace Votyra.Core
{
    public class AsyncTerrainGroupGeneratorManagerFactory2i : ITerrainGroupGeneratorManagerFactory2i, IDisposable
    {
        public AsyncTerrainGroupGeneratorManagerFactory2i(ITerrainMeshFactory terrainMeshFactory, ITerrainConfig terrainConfig, ITerrainMesher terrainMesher, ITerrainMeshUpdater terrainMeshUpdater,Func<GameObject> gameObjectFactory,[InjectOptional]  ITerrainVertexPostProcessor vertexPostProcessor,[InjectOptional]  ITerrainUVPostProcessor uvPostProcessor)
        {
            _terrainMeshFactory = terrainMeshFactory;
            _terrainMesher = terrainMesher;
            _terrainMeshUpdater = terrainMeshUpdater;
            _gameObjectFactory = gameObjectFactory;
            _cellInGroupCount = terrainConfig.CellInGroupCount;
        }

        private readonly ITerrainMeshFactory _terrainMeshFactory;
        private readonly ITerrainMesher _terrainMesher;
        private readonly ITerrainMeshUpdater _terrainMeshUpdater;
        private readonly Func<GameObject> _gameObjectFactory;

        private readonly CancellationTokenSource _onDestroyCts = new CancellationTokenSource();
        private readonly Vector2i _cellInGroupCount;

        public ITerrainGroupGeneratorManager2i CreateGroupManager(Vector2i newGroup)
        {
            return new AsyncTerrainGroupGeneratorManager2i(_cellInGroupCount, _gameObjectFactory, newGroup,
                _onDestroyCts.Token, _terrainMeshFactory.CreatePooledTerrainMesh(), _terrainMesher, _terrainMeshUpdater);
        }


        public void Dispose()
        {
            _onDestroyCts.Cancel();
        }
    }
}