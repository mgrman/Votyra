using System.Linq;
using Votyra.Core.Models;

namespace Votyra.Core.TerrainMeshes
{
    public class FixedTerrainMesh2i : ITerrainMesh2i
    {
        public Rect3f MeshBounds { get; private set; }
        public Vector3f[] Vertices { get; private set; }
        public Vector3f[] Normals { get; private set; }
        public Vector2f[] UV { get; private set; }
        public int[] Indices { get; private set; }

        public Vector2i CellInGroupCount { get; private set; }
        public int CellCount { get; private set; }
        public int QuadCount { get; private set; }
        public int TriangleCount { get; private set; }
        public int PointCount { get; private set; }

        private int _counter;

        public virtual void Initialize(Vector2i cellInGroupCount)
        {
            CellInGroupCount = cellInGroupCount;
            CellCount = CellInGroupCount.AreaSum;
            QuadCount = CellCount * 3;
            TriangleCount = QuadCount * 2;
            PointCount = TriangleCount * 3;

            Vertices = new Vector3f[PointCount];
            UV = new Vector2f[PointCount];
            Indices = Enumerable.Range(0, PointCount).ToArray();
            Normals = new Vector3f[PointCount];
        }

        public void Clear(Rect3f meshBounds)
        {
            MeshBounds = meshBounds;
            _counter = 0;
        }

        public void AddTriangle(Vector3f posA, Vector3f posB, Vector3f posC)
        {
            var side1 = posB - posA;
            var side2 = posC - posA;
            var normal = Vector3f.Cross(side1, side2).Normalized;

            Vertices[_counter] = posA;
            UV[_counter] = new Vector2f(posA.X, posA.Y);
            Normals[_counter] = normal;
            _counter++;

            Vertices[_counter] = posB;
            UV[_counter] = new Vector2f(posB.X, posB.Y);
            Normals[_counter] = normal;
            _counter++;

            Vertices[_counter] = posC;
            UV[_counter] = new Vector2f(posC.X, posC.Y);
            Normals[_counter] = normal;
            _counter++;
        }
    }
}