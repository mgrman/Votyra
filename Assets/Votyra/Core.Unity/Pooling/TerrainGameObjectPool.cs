using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Models.ObjectPool;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.TerrainMeshes;
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
            return () => new TerrainGameObject(gameObjectFactory());
        }
    }
}