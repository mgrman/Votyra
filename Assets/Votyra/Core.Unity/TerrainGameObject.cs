using System;
using System.Threading.Tasks;
using UniRx.Async;
using UnityEngine;

namespace Votyra.Core.Unity
{
    public class TerrainGameObject : ITerrainGameObject
    {
        private readonly Func<GameObject> _factory;

        public TerrainGameObject(Func<GameObject> factory)
        {
            _factory = factory;
        }

        public bool IsInitialized => GameObject != null;

        public void Initialize()
        {
            if (IsInitialized)
            {
                throw new InvalidOperationException();
            }

            GameObject = _factory();
            MeshFilter = GameObject.GetComponent<MeshFilter>();
            MeshCollider = GameObject.GetComponent<MeshCollider>();
            BoxCollider = GameObject.GetComponent<BoxCollider>();
        }

        public async Task InitializeAsync()
        {
            if (IsInitialized)
            {
                throw new InvalidOperationException();
            }

            await UniTask.SwitchToMainThread();
            Initialize();
        }

        public void SetActive(bool isActive)
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException();
            }

            GameObject.SetActive(isActive);
        }

        public async Task SetActiveAsync(bool isActive)
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException();
            }

            await UniTask.SwitchToMainThread();
            SetActive(isActive);
        }

        public GameObject GameObject { get; private set; }

        public MeshFilter MeshFilter { get; private set; }

        public MeshCollider MeshCollider { get; private set; }

        public BoxCollider BoxCollider { get; private set; }
    }
}
