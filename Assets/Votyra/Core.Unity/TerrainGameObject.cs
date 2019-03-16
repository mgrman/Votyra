using System;
using System.Threading.Tasks;
using UnityEngine;
using Votyra.Core.Pooling;
using Votyra.Core.Utils;

namespace Votyra.Core.Unity
{
    public class TerrainGameObject : ITerrainGameObject
    {
        public TerrainGameObject(GameObject go)
        {
            GameObject = go;
            MeshFilter = go.GetComponent<MeshFilter>();
            MeshCollider = go.GetComponent<MeshCollider>();
            BoxCollider = go.GetComponent<BoxCollider>();
        }

        public GameObject GameObject { get; }
        public MeshFilter MeshFilter { get; }
        public MeshCollider MeshCollider { get; }
        public BoxCollider BoxCollider { get; }

        public void Return()
        {
            MainThreadUtils.RunOnMainThreadAsync(() =>
            {
                GameObject.SetActive(false);
            });
            OnReturn?.Invoke(this);
        }

        public event Action<ITerrainGameObject> OnReturn;
    }
}