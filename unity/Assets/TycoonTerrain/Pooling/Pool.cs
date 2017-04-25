using TycoonTerrain.TerrainGenerators;
using System.Collections.Generic;
using System.Linq;
using TycoonTerrain.Common.Models;
using TycoonTerrain.Common.Models.ObjectPool;
using TycoonTerrain.TerrainMeshers.TriangleMesh;

namespace TycoonTerrain.Pooling
{
    public static class Pool
    {
        public static readonly ConcurentObjectPool<List<Vector2i>> Vector2ListPool = new ConcurentObjectPool<List<Vector2i>>(5, () => new List<Vector2i>(0));
        public static readonly ConcurentObjectDictionaryPool<ITriangleMesh, int> Meshes = new ConcurentObjectDictionaryPool<ITriangleMesh, int>(100, triangleCount => new FixedTriangleMesh(triangleCount));

        public static readonly ConcurentObjectDictionaryPool<IList<ITriangleMesh>, MeshKey> Meshes2 = new ConcurentObjectDictionaryPool<IList<ITriangleMesh>, MeshKey>(3, key => Enumerable.Range(0, key.MeshCount).Select(o => new FixedTriangleMesh(key.TriangleCount)).ToArray());

        public static readonly ConcurentObjectDictionaryPool<IList<ITerrainGroup>, TerrainGroupKey> TerrainGroups = new ConcurentObjectDictionaryPool<IList<ITerrainGroup>, TerrainGroupKey>(3, key => Enumerable.Range(0, key.GroupsToUpdate).Select(o => new TerrainGroup(key.CellInGroupCount)).ToArray());

        public struct TerrainGroupKey
        {
            public readonly int GroupsToUpdate;
            public readonly Vector2i CellInGroupCount;

            public TerrainGroupKey(int groupsToUpdate, Vector2i cellInGroupCount)
            {
                GroupsToUpdate = groupsToUpdate;
                CellInGroupCount = cellInGroupCount;
            }
        }

        public struct MeshKey
        {
            public readonly int MeshCount;
            public readonly int TriangleCount;

            public MeshKey(int meshCount, int triangleCount)
            {
                MeshCount = meshCount;
                TriangleCount = triangleCount;
            }
        }
    }
}