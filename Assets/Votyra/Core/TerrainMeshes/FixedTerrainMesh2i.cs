using System.Linq;
using UnityEngine;
using Votyra.Core.Models;

namespace Votyra.Core.TerrainMeshes
{
    public class FixedTerrainMesh2i : ITerrainMeshWithFixedCapacity
    {
        private int _counter;
        public int[] Indices { get; private set; }
        public Range3f MeshBounds { get; private set; }
        public Vector3f[] Normals { get; private set; }
        public int PointCount => Vertices.Length;
        public int TriangleCapacity { get; private set; }
        public int TriangleCount => _counter / 3;
        public Vector2f[] UV { get; private set; }
        public Vector3f[] Vertices { get; private set; }

        public void AddTriangle(Vector3f posA, Vector3f posB, Vector3f posC)
        {
            var side1 = posB - posA;
            var side2 = posC - posA;
            var normal = Vector3f.Cross(side1, side2).Normalized;

            Vertices[_counter] = posA;
            UV[_counter] = new Vector2f(posA.X, posA.Y);
            Normals[_counter] = normal;
            Indices[_counter] = _counter;
            _counter++;

            Vertices[_counter] = posB;
            UV[_counter] = new Vector2f(posB.X, posB.Y);
            Normals[_counter] = normal;
            Indices[_counter] = _counter;
            _counter++;

            Vertices[_counter] = posC;
            UV[_counter] = new Vector2f(posC.X, posC.Y);
            Normals[_counter] = normal;
            Indices[_counter] = _counter;
            _counter++;
        }

        public void Clear(Range3f meshBounds)
        {
            MeshBounds = meshBounds;
            _counter = 0;
        }

        public void FinalizeMesh()
        {
            if (_counter != PointCount)
            {
                for (int i = _counter; i < PointCount; i++)
                {
                    Vertices[i] = Vertices[_counter % 3];
                    UV[i] = UV[_counter % 3];
                    Normals[i] = Normals[_counter % 3];
                    Indices[_counter] = Indices[_counter % 3];
                }
                Debug.LogWarning($"Mesh was not fully filled. Expected {PointCount} points, got {_counter} points!");
            }
        }

        public virtual void Initialize(int triangleCapacity)
        {
            TriangleCapacity = triangleCapacity;

            var pointCount = triangleCapacity * 3;

            Vertices = new Vector3f[pointCount];
            UV = new Vector2f[pointCount];
            Indices = Enumerable.Range(0, pointCount).ToArray();
            Normals = new Vector3f[pointCount];
        }
    }
}