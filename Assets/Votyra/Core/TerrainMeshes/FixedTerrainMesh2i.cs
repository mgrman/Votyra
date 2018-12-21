using System.Linq;
using UnityEngine;
using Votyra.Core.Models;

namespace Votyra.Core.TerrainMeshes
{
    public class FixedTerrainMesh2i : ITerrainMeshWithFixedCapacity
    {
        private int _counter;
        public Range3f MeshBounds { get; private set; }
        public Vector3f Offset { get; private set; }
        public Vector3f[] Vertices { get; private set; }
        public Vector3f[] Normals { get; private set; }
        public Vector2f[] UV { get; private set; }
        public int[] Indices { get; private set; }

        public int TriangleCount => _counter / 3;

        public int VertexCount => Vertices.Length;

        public int TriangleCapacity { get; private set; }

        public Vector3f this[int point] => Vertices[point];

        public virtual void Initialize(int triangleCapacity)
        {
            TriangleCapacity = triangleCapacity;

            var pointCount = triangleCapacity * 3;

            Vertices = new Vector3f[pointCount];
            UV = new Vector2f[pointCount];
            Indices = Enumerable.Range(0, pointCount).ToArray();
            Normals = new Vector3f[pointCount];
        }

        public void Clear(Range3f meshBounds, Vector3f offset)
        {
            MeshBounds = meshBounds;
            Offset = offset;
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

        public void FinalizeMesh()
        {
            if (_counter != VertexCount)
            {
                for (int i = _counter; i < VertexCount; i++)
                {
                    Vertices[i] = Vector3f.Zero;
                    UV[i] = Vector2f.Zero;
                    Normals[i] = Vector3f.Zero;
                }
                Debug.LogWarning($"Mesh was not fully filled. Expected {VertexCount} points, got {_counter} points!");
            }
        }
    }
}