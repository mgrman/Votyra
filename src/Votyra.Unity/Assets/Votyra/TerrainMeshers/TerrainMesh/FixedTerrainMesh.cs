using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Votyra.Common.Models;

namespace Votyra.TerrainMeshers.TriangleMesh
{
    public class FixedTerrainMesh : ITerrainMesh
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

        public FixedTerrainMesh()
        {

        }

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
        }

        public void AddQuad(Vector2i cellInGroup, Vector3 x0y0, Vector3 x0y1, Vector3 x1y0, Vector3 x1y1, bool flipSides)
        {
            int cellIndex = cellInGroup.x + cellInGroup.y * CellInGroupCount.x;
            int quadIndex = cellIndex * 3;
            int triIndex = quadIndex * 2;
            if (flipSides)
            {

                AddTriangle(triIndex, x0y0, x1y0, x1y1);
                AddTriangle(triIndex + 1, x1y1, x0y1, x0y0);
            }
            else
            {
                AddTriangle(triIndex, x0y0, x1y0, x0y1);
                AddTriangle(triIndex + 1, x1y0, x1y1, x0y1);
            }
        }

        public void AddWallX(Vector2i cellInGroup, Vector3 a, Vector3 b, Vector3 b_lower, Vector3 a_lower)
        {
            int cellIndex = cellInGroup.x + cellInGroup.y * CellInGroupCount.x;
            int quadIndex = cellIndex * 3 + 1;
            int triIndex = quadIndex * 2;

            AddTriangle(triIndex, a, b, b_lower);
            AddTriangle(triIndex + 1, a, b_lower, a_lower);
        }

        public void AddWallY(Vector2i cellInGroup, Vector3 a, Vector3 b, Vector3 b_lower, Vector3 a_lower)
        {
            int cellIndex = cellInGroup.x + cellInGroup.y * CellInGroupCount.x;
            int quadIndex = cellIndex * 3 + 2;
            int triIndex = quadIndex * 2;
            AddTriangle(triIndex, a, b, b_lower);
            AddTriangle(triIndex + 1, a, b_lower, a_lower);
        }

        private void AddTriangle(int index, Vector3 posA, Vector3 posB, Vector3 posC)
        {
            Vertices[index * 3 + 0] = posA;
            Vertices[index * 3 + 1] = posB;
            Vertices[index * 3 + 2] = posC;

            UV[index * 3 + 0] = new Vector2(posA.x, posA.y);
            UV[index * 3 + 1] = new Vector2(posB.x, posB.y);
            UV[index * 3 + 2] = new Vector2(posC.x, posC.y);

            var side1 = posB - posA;
            var side2 = posC - posA;
            var normal = Vector3.Cross(side1, side2).normalized;
            Normals[index * 3 + 0] = normal;
            Normals[index * 3 + 1] = normal;
            Normals[index * 3 + 2] = normal;
        }
    }
}