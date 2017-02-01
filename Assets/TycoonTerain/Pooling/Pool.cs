using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class Pool
{
    public static readonly ConcurentObjectPool<List<Vector2i>> Vector2iListPool = new ConcurentObjectPool<List<Vector2i>>(5,()=>new List<Vector2i>(0));
    public static readonly ConcurentObjectDictionaryPool<ITriangleMesh, int> Meshes = new ConcurentObjectDictionaryPool<ITriangleMesh, int>(100,triangleCount=>new FixedTriangleMesh(triangleCount));

    public static readonly ConcurentObjectDictionaryPool<IList<ITriangleMesh>, MeshKey> Meshes2 = new ConcurentObjectDictionaryPool<IList<ITriangleMesh>, MeshKey>(3, key => Enumerable.Range(0, key.MeshCount).Select(o => new FixedTriangleMesh(key.TriangleCount)).ToArray());


    public struct MeshKey
    {
        public readonly int MeshCount;
        public readonly int TriangleCount;

        public MeshKey(int meshCount,int triangleCount)
        {
            MeshCount = meshCount;
            TriangleCount = triangleCount;
        }
    }
}

