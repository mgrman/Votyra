using System;
using UnityEngine;
using Votyra.Core.Unity;

namespace Votyra.Core.Pooling
{
    public class TerrainGameObjectPool : Pool<ITerrainGameObject>, ITerrainGameObjectPool
    {
        public TerrainGameObjectPool(Func<GameObject> gameObjectFactory)
            : base(CreateMeshFunc(gameObjectFactory))
        {
        }

        private static Func<ITerrainGameObject> CreateMeshFunc(Func<GameObject> gameObjectFactory)
        {
            return () => new TerrainGameObject(gameObjectFactory);
        }
    }
}
