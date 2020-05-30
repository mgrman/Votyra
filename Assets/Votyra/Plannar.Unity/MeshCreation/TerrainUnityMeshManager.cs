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
        private readonly ITerrainGameObjectPool gameObjectPool;
        private readonly ILayerConfig layerConfig;

        private readonly IUnityTerrainGeneratorManager2I manager;
        private readonly Dictionary<Vector2i, ITerrainGameObject> unityMeshes = new Dictionary<Vector2i, ITerrainGameObject>();

        public TerrainUnityMeshManager(IUnityTerrainGeneratorManager2I manager, ITerrainGameObjectPool gameObjectPool, [Inject(Id = "root")]
            GameObject root, [InjectOptional]
            ILayerConfig layerConfig)
        {
            this.manager = manager;
            this.gameObjectPool = gameObjectPool;
            this.layerConfig = layerConfig;
            this.manager.NewTerrain += this.NewTerrain;
            this.manager.ChangedTerrain += this.ChangedTerrain;
            this.manager.RemovedTerrain += this.RemovedTerrain;
        }

        public void Dispose()
        {
            this.manager.NewTerrain -= this.NewTerrain;
            this.manager.ChangedTerrain -= this.ChangedTerrain;
            this.manager.RemovedTerrain -= this.RemovedTerrain;
        }

        private void ChangedTerrain(Vector2i group, ITerrainMesh2F mesh)
        {
#if UNITY_EDITOR
            if (!this.unityMeshes.ContainsKey(group))
            {
                Debug.LogError($"Changed {group} before NewTerrain was called!");
                return;
            }
#endif
            Debug.Log("Changed " + group);
            var pooledGameObject = this.unityMeshes[group];

            mesh.SetUnityMesh(pooledGameObject);
        }

        private void RemovedTerrain(Vector2i group, ITerrainMesh2F mesh)
        {
#if UNITY_EDITOR
            if (!this.unityMeshes.ContainsKey(group))
            {
                Debug.LogError($"RemovedTerrain {group} called without NewTerrain first!");
                return;
            }
#endif
            Debug.Log("Removed " + group);
            var pooledGameObject = this.unityMeshes[group];
            this.gameObjectPool.ReturnRaw(pooledGameObject);

            this.unityMeshes.Remove(group);
        }

        private void NewTerrain(Vector2i group, ITerrainMesh2F mesh)
        {
#if UNITY_EDITOR
            if (this.unityMeshes.ContainsKey(group))
            {
                Debug.LogError($"NewTerrain {group} called multiple times without RemovedTerrain after each time!");
                return;
            }
#endif

            Debug.Log("New     " + group);
            var pooledGameObject = this.gameObjectPool.GetRaw();
            if (!pooledGameObject.IsInitialized)
            {
                pooledGameObject.Initialize();
            }

            pooledGameObject.GameObject.name = $"Group_{group}_{this.layerConfig?.Layer}_{this.GetHashCode()}";
            pooledGameObject.SetActive(true);
            mesh.SetUnityMesh(pooledGameObject);

            this.unityMeshes[group] = pooledGameObject;
        }
    }
}
