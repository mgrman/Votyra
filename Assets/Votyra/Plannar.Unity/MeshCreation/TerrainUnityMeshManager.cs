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
    public class TerrainUnityMeshManager : IDisposable
    {
        private readonly Dictionary<Vector2i, ITerrainGameObject> _unityMeshes = new Dictionary<Vector2i, ITerrainGameObject>();
        private readonly Dictionary<Vector2i, ITerrainMesh2f> _coreMeshes = new Dictionary<Vector2i, ITerrainMesh2f>();

        private IUnityTerrainGeneratorManager2i _manager;
        private ITerrainGameObjectPool _gameObjectPool;

        public TerrainUnityMeshManager(IUnityTerrainGeneratorManager2i manager, ITerrainGameObjectPool gameObjectPool, [Inject(Id = "root")] GameObject root)
        {
            _manager = manager;
            _gameObjectPool = gameObjectPool;
            _manager.NewTerrain += NewTerrain;
            _manager.ChangedTerrain += ChangedTerrain;
            _manager.RemovedTerrain += RemovedTerrain;
        }

        private void ChangedTerrain(Vector2i group)
        {
            var pooledGameObject = _unityMeshes[group];
            var mesh = _coreMeshes[group];

            mesh.SetUnityMesh(pooledGameObject);
        }

        private void RemovedTerrain(Vector2i group)
        {
            var pooledGameObject = _unityMeshes[group];
            _gameObjectPool.ReturnRaw(pooledGameObject);

            _unityMeshes.Remove(group);
            _coreMeshes.Remove(group);
        }

        private void NewTerrain(Vector2i group, ITerrainMesh2f mesh)
        {
            var pooledGameObject = _gameObjectPool.GetRaw();
            if (!pooledGameObject.IsInitialized)
                pooledGameObject.Initialize();
            pooledGameObject.SetActive(true);
            mesh.SetUnityMesh(pooledGameObject);

            _unityMeshes[group] = pooledGameObject;
            _coreMeshes[group] = mesh;
        }

        public void Dispose()
        {
            _manager.NewTerrain -= NewTerrain;
            _manager.ChangedTerrain -= ChangedTerrain;
            _manager.RemovedTerrain -= RemovedTerrain;
        }
    }
}