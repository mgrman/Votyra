using System;
using System.Collections.Generic;
using Votyra.Models;
using Votyra.Models.ObjectPool;
using Votyra.TerrainTileGenerators.TerrainGroups;

namespace Votyra.Unity.Assets.Votyra.Pooling
{
    public class PooledTerrainGroup : TerrainGroup, IDisposable
    {
        private readonly Vector2i _key;
        public static readonly ConcurentObjectDictionaryPool<PooledTerrainGroup, Vector2i> Pool = new ConcurentObjectDictionaryPool<PooledTerrainGroup, Vector2i>(100, cellInGroupCount => new PooledTerrainGroup(cellInGroupCount));

        private PooledTerrainGroup(Vector2i cellInGroupCount)
            : base(cellInGroupCount)
        {
            _key = cellInGroupCount;
        }

        public static PooledTerrainGroup CreateDirty(Vector2i cellInGroupCount)
        {
            var obj = Pool.GetObject(cellInGroupCount);
            return obj;
        }

        public void Dispose()
        {
            Pool.ReturnObject(this, _key);
        }
    }
}