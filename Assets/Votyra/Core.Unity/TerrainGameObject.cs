using System;
using System.Threading.Tasks;
using UniRx.Async;
using UnityEngine;

namespace Votyra.Core.Unity
{
    public class TerrainGameObject : ITerrainGameObject
    {
        private readonly Func<GameObject> factory;

        public TerrainGameObject(Func<GameObject> factory)
        {
            this.factory = factory;
        }

        public bool IsInitialized => this.GameObject != null;

        public void Initialize()
        {
            if (this.IsInitialized)
            {
                throw new InvalidOperationException();
            }

            this.GameObject = this.factory();
            this.MeshFilter = this.GameObject.GetComponent<MeshFilter>();
            this.MeshCollider = this.GameObject.GetComponent<MeshCollider>();
            this.BoxCollider = this.GameObject.GetComponent<BoxCollider>();
        }

        public async Task InitializeAsync()
        {
            if (this.IsInitialized)
            {
                throw new InvalidOperationException();
            }

            await UniTask.SwitchToMainThread();
            this.Initialize();
        }

        public void SetActive(bool isActive)
        {
            if (!this.IsInitialized)
            {
                throw new InvalidOperationException();
            }

            this.GameObject.SetActive(isActive);
        }

        public async Task SetActiveAsync(bool isActive)
        {
            if (!this.IsInitialized)
            {
                throw new InvalidOperationException();
            }

            await UniTask.SwitchToMainThread();
            this.SetActive(isActive);
        }

        public GameObject GameObject { get; private set; }

        public MeshFilter MeshFilter { get; private set; }

        public MeshCollider MeshCollider { get; private set; }

        public BoxCollider BoxCollider { get; private set; }
    }
}
