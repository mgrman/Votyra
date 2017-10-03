using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Votyra.Core.TerrainMeshes
{
    public class ExpandingTerrainMesh : ITerrainMesh
    {
        public Bounds MeshBounds { get; private set; }
        public List<Vector3> Vertices { get; }
        public List<Vector3> Normals { get; }
        public List<Vector2> UV { get; }
        public List<int> Indices { get; }

        public int TriangleCount { get; private set; }
        public int PointCount { get; private set; }

        public ExpandingTerrainMesh()
        {
            Vertices = new List<Vector3>();
            UV = new List<Vector2>();
            Indices = new List<int>();
            Normals = new List<Vector3>();
        }

        public void Clear(Bounds meshBounds)
        {
            MeshBounds = meshBounds;
            TriangleCount = 0;
            PointCount = 0;
            Vertices.Clear();
            UV.Clear();
            Indices.Clear();
            Normals.Clear();
        }

        public void AddTriangle(Vector3 posA, Vector3 posB, Vector3 posC)
        {
            var side1 = posB - posA;
            var side2 = posC - posA;
            var normal = Vector3.Cross(side1, side2).normalized;

            Indices.Add(PointCount);
            Vertices.Add(posA);
            UV.Add(new Vector2(posA.x, posA.y));
            Normals.Add(normal);
            PointCount++;

            Indices.Add(PointCount);
            Vertices.Add(posB);
            UV.Add(new Vector2(posB.x, posB.y));
            Normals.Add(normal);
            PointCount++;

            Indices.Add(PointCount);
            Vertices.Add(posC);
            UV.Add(new Vector2(posC.x, posC.y));
            Normals.Add(normal);
            PointCount++;

            TriangleCount++;
        }
    }
}
