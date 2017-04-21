using System;
using System.Collections.Generic;
using System.Linq;
using TycoonTerrain.Common.Models;
using UnityEngine;

namespace TycoonTerrain.TerrainMeshers.TriangleMesh
{
    public class FixedTriangleMesh : ITriangleMesh
    {
        public Bounds MeshBounds { get; private set; }
        public Vector3[] Vertices { get; private set; }
        public Vector3[] Normals { get; private set; }
        public Vector2[] UV { get; private set; }
        public int[] Indices { get; private set; }

        public int PointCount { get { return Vertices.Length; } }
        public int TriangleCount { get { return PointCount / 3; } }

        #region ITriangleMesh

        Vector3[] ITriangleMesh.Vertices { get { return Vertices; } }
        Vector3[] ITriangleMesh.Normals { get { return Normals; } }
        Vector2[] ITriangleMesh.UV { get { return UV; } }
        int[] ITriangleMesh.Indices { get { return Indices; } }

        #endregion ITriangleMesh

        public FixedTriangleMesh(Bounds meshBounds, int triangleCount)
        {
            Clear(triangleCount);
            Clear(meshBounds);
        }

        public FixedTriangleMesh(int triangleCount)
        {
            Clear(triangleCount);
        }

        public FixedTriangleMesh(Bounds meshBounds, int triangleCount, IList<Vector3> vertices,
            IList<Vector3> normals,
            IList<Vector2> uv,
            IList<int> indices)
        {
            int pointCount = triangleCount * 3;
            if (pointCount != vertices.Count || pointCount != normals.Count || pointCount != uv.Count || pointCount != indices.Count)
            {
                throw new InvalidOperationException();
            }

            MeshBounds = meshBounds;
            Vertices = vertices.ToArray();
            UV = uv.ToArray();
            Indices = indices.ToArray();
            Normals = normals.ToArray();
        }

        public void Clear(int triangleCount)
        {
            int pointCount = triangleCount * 3;

            Vertices = new Vector3[pointCount];
            UV = new Vector2[pointCount];
            Indices = Enumerable.Range(0, pointCount).ToArray();
            Normals = new Vector3[pointCount];
        }

        public void Clear(Bounds meshBounds)
        {
            MeshBounds = meshBounds;
        }

        public void Add(int index, Vector3 posA, Vector3 posB, Vector3 posC)
        {
            Vertices[index * 3 + 0] = posA;
            Vertices[index * 3 + 1] = posB;
            Vertices[index * 3 + 2] = posC;

            //var size = _meshBounds.size;
            //_uv[index * 3 + 0] = new Vector2(posA.x / size.x, posA.y / size.y);
            //_uv[index * 3 + 1] = new Vector2(posB.x / size.x, posB.y / size.y);
            //_uv[index * 3 + 2] = new Vector2(posC.x / size.x, posC.y / size.y);

            //var side1 = posB - posA;
            //var side2 = posC - posA;
            //var normal = Vector3.Cross(side1, side2).normalized;
            //_normals[index * 3 + 0] = normal;
            //_normals[index * 3 + 1] = normal;
            //_normals[index * 3 + 2] = normal;
        }

        public void FinilizeMesh()
        {
        }
    }
}