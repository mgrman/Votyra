using System;
using System.Threading.Tasks;
using UnityEngine;
using Votyra.Core.Pooling;
using Votyra.Core.Utils;

namespace Votyra.Core.Unity
{
    public class TerrainGameObject : ITerrainGameObject
    {
        private Func<GameObject> _factory;
        public TerrainGameObject(Func<GameObject> factory)
        {
            _factory = factory;
        }

        public void InitializeOnMainThread()
        {
            if (GameObject != null)
            {
                return;
            }
            GameObject = _factory();
            MeshFilter = GameObject.GetComponent<MeshFilter>();
            MeshCollider = GameObject.GetComponent<MeshCollider>();
            BoxCollider = GameObject.GetComponent<BoxCollider>();
        }

        public GameObject GameObject { get; private set; }
        public MeshFilter MeshFilter { get; private set; }
        public MeshCollider MeshCollider { get; private set; }
        public BoxCollider BoxCollider { get; private set; }

    }
}