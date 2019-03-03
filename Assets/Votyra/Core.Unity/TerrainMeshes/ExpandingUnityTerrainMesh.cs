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

        public Bounds MeshBounds =>
            Area3f.FromMinAndMax(MeshBoundsXY.Min.ToVector3f(_minZ), MeshBoundsXY.Max.ToVector3f(_maxZ))
                .ToBounds();

        public Area2f MeshBoundsXY { get; private set; }

        private float _minZ;

        private float _maxZ;
        private Func<Vector3f, Vector3f> VertexPostProcessor { get; set; }
        private Func<Vector2f, Vector2f> UVAdjustor { get; set; }

        public List<Vector3> Vertices { get; }
        public List<Vector3> Normals { get; }
        public List<Vector2> UV { get; }
        public List<int> Indices { get; }

        public int TriangleCount { get; private set; }
        public int VertexCount { get; private set; }

        public void Initialize(Func<Vector3f, Vector3f> vertexPostProcessor, Func<Vector2f, Vector2f> uvAdjustor)
        {
            VertexPostProcessor = vertexPostProcessor;
            UVAdjustor = uvAdjustor;
        }

        public void Reset(Area3f area)
        {
            MeshBoundsXY = Area2f.FromMinAndMax(area.Min.XY(), area.Max.XY());
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

            var uvA = posA.XY();
            var uvB = posB.XY();
            var uvC = posC.XY();
            if (UVAdjustor != null)
            {
                uvA = UVAdjustor(posA.XY());
                uvB = UVAdjustor(posB.XY());
                uvC = UVAdjustor(posC.XY());
            }

            var side1 = posB - posA;
            var side2 = posC - posA;
            var normal = Vector3fUtils.Cross(side1, side2)
                .Normalized();

            _minZ = TriangleCount == 0 ? posA.Z : Math.Min(_minZ, posA.Z);
            _minZ = Math.Min(_minZ, posB.Z);
            _minZ = Math.Min(_minZ, posC.Z);
            _maxZ = TriangleCount == 0 ? posA.Z : Math.Max(_maxZ, posA.Z);
            _maxZ = Math.Max(_maxZ, posB.Z);
            _maxZ = Math.Max(_maxZ, posC.Z);

            unsafe
            {
                Indices.Add(VertexCount);
                Vertices.Add(*(Vector3*)&posA);
                UV.Add(*(Vector2*)&uvA);
                Normals.Add(*(Vector3*)&normal);
                VertexCount++;

                Indices.Add(VertexCount);
                Vertices.Add(*(Vector3*)&posB);
                UV.Add(*(Vector2*)&uvB);
                Normals.Add(*(Vector3*)&normal);
                VertexCount++;

                Indices.Add(VertexCount);
                Vertices.Add(*(Vector3*)&posC);
                UV.Add(*(Vector2*)&uvC);
                Normals.Add(*(Vector3*)&normal);
                VertexCount++;
            }

            TriangleCount++;
        }

        public void FinalizeMesh()
        {
        }
    }
}