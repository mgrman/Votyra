using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Votyra.Models;

namespace Votyra.Core.TerrainMeshes
{
    public class FixedTerrainMesh2i : ITerrainMesh2i
    {
        public Bounds MeshBounds { get; private set; }
        public Vector3[] Vertices { get; private set; }
        public Vector3[] Normals { get; private set; }
        public Vector2[] UV { get; private set; }
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

            Vertices = new Vector3[PointCount];
            UV = new Vector2[PointCount];
            Indices = Enumerable.Range(0, PointCount).ToArray();
            Normals = new Vector3[PointCount];
        }

        public void Clear(Bounds meshBounds)
        {
            MeshBounds = meshBounds;
            _counter = 0;
        }


        public void AddTriangle(Vector3 posA, Vector3 posB, Vector3 posC)
        {
            var side1 = posB - posA;
            var side2 = posC - posA;
            var normal = Vector3.Cross(side1, side2).normalized;

            Vertices[_counter] = posA;
            UV[_counter] = new Vector2(posA.x, posA.y);
            Normals[_counter] = normal;
            _counter++;

            Vertices[_counter] = posB;
            UV[_counter] = new Vector2(posB.x, posB.y);
            Normals[_counter] = normal;
            _counter++;

            Vertices[_counter] = posC;
            UV[_counter] = new Vector2(posC.x, posC.y);
            Normals[_counter] = normal;
            _counter++;

        }
    }
}
