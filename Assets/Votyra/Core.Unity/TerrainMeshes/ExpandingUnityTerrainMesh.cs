using System;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.TerrainMeshes
{
    public class ExpandingUnityTerrainMesh : ITerrainMesh
    {
        public ExpandingUnityTerrainMesh()
        {
            Vertices = new List<Vector3>();
            UV = new List<Vector2>();
            Indices = new List<int>();
            Normals = new List<Vector3>();
        }

        public Bounds MeshBounds { get; private set; }
        public Func<Vector3f, Vector3f> VertexPostProcessor { get; private set; }
        public Func<Vector2f, Vector2f> UVAdjustor { get; private set; }
        public List<Vector3> Vertices { get; }
        public List<Vector3> Normals { get; }
        public List<Vector2> UV { get; }
        public List<int> Indices { get; }

        public int TriangleCount { get; private set; }
        public int VertexCount { get; private set; }

        public void Clear(Area3f meshBounds, Func<Vector3f, Vector3f> vertexPostProcessor, Func<Vector2f, Vector2f> uvAdjustor)
        {
            MeshBounds = meshBounds.ToBounds();
            VertexPostProcessor = vertexPostProcessor;
            UVAdjustor = uvAdjustor;
            TriangleCount = 0;
            VertexCount = 0;
            Vertices.Clear();
            UV.Clear();
            Indices.Clear();
            Normals.Clear();
        }

        public void AddTriangle(Vector3f posA, Vector3f posB, Vector3f posC)
        {
            if (VertexPostProcessor != null)
            {
                posA = VertexPostProcessor(posA);
                posB = VertexPostProcessor(posB);
                posC = VertexPostProcessor(posC);
            }

            var side1 = posB - posA;
            var side2 = posC - posA;
            var normal = Vector3f.Cross(side1, side2)
                .Normalized;

            Indices.Add(VertexCount);
            Vertices.Add(posA.ToVector3());
            UV.Add((UVAdjustor?.Invoke(posA.XY) ?? posA.XY).ToVector2());
            Normals.Add(normal.ToVector3());
            VertexCount++;

            Indices.Add(VertexCount);
            Vertices.Add(posB.ToVector3());
            UV.Add((UVAdjustor?.Invoke(posB.XY) ?? posB.XY).ToVector2());
            Normals.Add(normal.ToVector3());
            VertexCount++;

            Indices.Add(VertexCount);
            Vertices.Add(posC.ToVector3());
            UV.Add((UVAdjustor?.Invoke(posC.XY) ?? posC.XY).ToVector2());
            Normals.Add(normal.ToVector3());
            VertexCount++;

            TriangleCount++;
        }

        public void FinalizeMesh()
        {
        }
    }
}