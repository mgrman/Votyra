using System;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Core;
using Votyra.Core.MeshUpdaters;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Unity;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class TerrainMeshManager : IDisposable
    {
        private readonly Dictionary<Vector2i, ITerrainGameObject> _pooledGameObjects = new Dictionary<Vector2i, ITerrainGameObject>();
        
        private ITerrainGeneratorManager2i _manager;
        private ITerrainGameObjectPool _gameObjectPool;

        public TerrainMeshManager(ITerrainGeneratorManager2i manager, ITerrainConfig config, ITerrainGameObjectPool gameObjectPool, [Inject(Id = "root")] GameObject root)
        {
            _manager = manager;
            _gameObjectPool = gameObjectPool;
            _manager.NewTerrain += NewTerrain;
            _manager.ChangedTerrain += ChangedTerrain;
            _manager.RemovedTerrain += RemovedTerrain;
        }

        private void ChangedTerrain(Vector2i arg1, ITerrainMesh2f mesh)
        {
            var pooledGameObject = _pooledGameObjects[arg1];

            if (!pooledGameObject.IsInitialized)
            {
                pooledGameObject.Initialize();
            }

            pooledGameObject.SetActive(true);

            mesh.SetUnityMesh(pooledGameObject);
        }

        private void RemovedTerrain(Vector2i arg1)
        {
            var pooledGameObject = _pooledGameObjects[arg1];
            _gameObjectPool.ReturnRaw(pooledGameObject);
        }

        private void NewTerrain(Vector2i arg1, ITerrainMesh2f arg2)
        {
            _pooledGameObjects[arg1] = _gameObjectPool.GetRaw();
        }

        public void Dispose()
        {
            _manager.NewTerrain += NewTerrain;
            _manager.ChangedTerrain += ChangedTerrain;
            _manager.RemovedTerrain += RemovedTerrain;
        }
    }
}