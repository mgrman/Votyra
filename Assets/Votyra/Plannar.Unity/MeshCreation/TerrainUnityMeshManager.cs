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

        private IUnityTerrainGeneratorManager2i _manager;
        private ITerrainGameObjectPool _gameObjectPool;
        private readonly ILayerConfig _layerConfig;

        public TerrainUnityMeshManager(IUnityTerrainGeneratorManager2i manager, ITerrainGameObjectPool gameObjectPool, [Inject(Id = "root")]
            GameObject root, [InjectOptional]ILayerConfig layerConfig)
        {
            _manager = manager;
            _gameObjectPool = gameObjectPool;
            _layerConfig = layerConfig;
            _manager.NewTerrain += NewTerrain;
            _manager.ChangedTerrain += ChangedTerrain;
            _manager.RemovedTerrain += RemovedTerrain;
        }

        private void ChangedTerrain(Vector2i group, ITerrainMesh2f mesh)
        {
#if UNITY_EDITOR
            if (!_unityMeshes.ContainsKey(group))
            {
                Debug.LogError($"Changed {group} before NewTerrain was called!");
                return;
            }
#endif
            Debug.Log("Changed " + group);
            var pooledGameObject = _unityMeshes[group];

            mesh.SetUnityMesh(pooledGameObject);
        }

        private void RemovedTerrain(Vector2i group, ITerrainMesh2f mesh)
        {
#if UNITY_EDITOR
            if (!_unityMeshes.ContainsKey(group))
            {
                Debug.LogError($"RemovedTerrain {group} called without NewTerrain first!");
                return;
            }
#endif
            Debug.Log("Removed " + group);
            var pooledGameObject = _unityMeshes[group];
            _gameObjectPool.ReturnRaw(pooledGameObject);

            _unityMeshes.Remove(group);
        }

        private void NewTerrain(Vector2i group, ITerrainMesh2f mesh)
        {
#if UNITY_EDITOR
            if (_unityMeshes.ContainsKey(group))
            {
                Debug.LogError($"NewTerrain {group} called multiple times without RemovedTerrain after each time!");
                return;
            }
#endif

            Debug.Log("New     " + group);
            var pooledGameObject = _gameObjectPool.GetRaw();
            if (!pooledGameObject.IsInitialized)
                pooledGameObject.Initialize();

            pooledGameObject.GameObject.name = $"Group_{group}_{_layerConfig?.Layer}_{this.GetHashCode()}";
            pooledGameObject.SetActive(true);
            mesh.SetUnityMesh(pooledGameObject);

            _unityMeshes[group] = pooledGameObject;
        }

        public void Dispose()
        {
            _manager.NewTerrain -= NewTerrain;
            _manager.ChangedTerrain -= ChangedTerrain;
            _manager.RemovedTerrain -= RemovedTerrain;
        }
    }
}
