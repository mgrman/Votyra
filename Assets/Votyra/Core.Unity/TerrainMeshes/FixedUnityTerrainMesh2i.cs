using System;
using System.Linq;
using UnityEngine;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.TerrainMeshes
{
    public class FixedUnityTerrainMesh2i : ITerrainMeshWithFixedCapacity
    {
        private int _counter;

        public Bounds MeshBounds { get; private set; }

        public Func<Vector3f, Vector3f> VertexPostProcessor { get; private set; }
        public Func<Vector2f, Vector2f> UVAdjustor { get; private set; }
        public Vector3[] Vertices { get; private set; }
        public Vector3[] Normals { get; private set; }
        public Vector2[] UV { get; private set; }
        public int[] Indices { get; private set; }

        public int TriangleCount => _counter / 3;

        public int VertexCount => Vertices.Length;

        public int TriangleCapacity { get; private set; }

        public virtual void Initialize(int triangleCapacity)
        {
            TriangleCapacity = triangleCapacity;

            var pointCount = triangleCapacity * 3;

            Vertices = new Vector3[pointCount];
            UV = new Vector2[pointCount];
            Indices = Enumerable.Range(0, pointCount)
                .ToArray();
            Normals = new Vector3[pointCount];
        }

        public void Initialize(Func<Vector3f, Vector3f> vertexPostProcessor, Func<Vector2f, Vector2f> uvAdjustor)
        {
            VertexPostProcessor = vertexPostProcessor;
            UVAdjustor = uvAdjustor;
        }

        public void Reset(Area3f area)
        {
            MeshBounds = area.ToBounds();
            _counter = 0;
        }
        

        public void AddTriangle(Vector3f posA, Vector3f posB, Vector3f posC)
        {
            Vector3 posAu;
            Vector3 posBu;
            Vector3 posCu;
            if (VertexPostProcessor != null)
            {
                posAu = VertexPostProcessor(posA)
                    .ToVector3();
                posBu = VertexPostProcessor(posB)
                    .ToVector3();
                posCu = VertexPostProcessor(posC)
                    .ToVector3();
            }
            else
            {
                posAu = posA.ToVector3();
                posBu = posB.ToVector3();
                posCu = posC.ToVector3();
            }
            

            Vector2 uvAu;
            Vector2 uvBu;
            Vector2 uvCu;
            if (UVAdjustor != null)
            {
                uvAu = UVAdjustor(posA.XY)
                    .ToVector2();
                uvBu = UVAdjustor(posB.XY)
                    .ToVector2();
                uvCu = UVAdjustor(posC.XY)
                    .ToVector2();
            }
            else
            {
                uvAu = posA.XY.ToVector2();
                uvBu = posB.XY.ToVector2();
                uvCu = posC.XY.ToVector2();
            }

            var side1 = posBu - posAu;
            var side2 = posCu - posAu;
            var normal = Vector3.Cross(side1, side2)
                .normalized;

            Vertices[_counter] = posAu;
            UV[_counter] = uvAu;
            Normals[_counter] = normal;
            _counter++;

            Vertices[_counter] = posBu;
            UV[_counter] = uvBu;
            Normals[_counter] = normal;
            _counter++;

            Vertices[_counter] = posCu;
            UV[_counter] = uvCu;
            Normals[_counter] = normal;
            _counter++;
        }

        public void FinalizeMesh()
        {
            if (_counter != VertexCount)
            {
                for (var i = _counter; i < VertexCount; i++)
                {
                    Vertices[i] = Vector3.zero;
                    UV[i] = Vector2.zero;
                    Normals[i] = Vector3.zero;
                }

                Debug.LogWarning($"Mesh was not fully filled. Expected {VertexCount} points, got {_counter} points!");
            }
        }
    }
}