using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Models;
using Votyra.Models.ObjectPool;
using Votyra.TerrainGenerators.TerrainMeshers.TerrainMeshes;

namespace Votyra.Unity.Assets.Votyra.Pooling
{
    public class PooledTerrainMeshContainer<T> : IPooledTerrainMesh
        where T : ITerrainMesh2i, new()
    {
        public T Mesh { get; }

        public Vector2i CellInGroupCount => Mesh.CellInGroupCount;

        ITerrainMesh2i IPooledTerrainMesh.Mesh => Mesh;

        private static readonly bool IsDisposable = typeof(IDisposable).IsAssignableFrom(typeof(T));

        private static readonly ConcurentObjectDictionaryPool<PooledTerrainMeshContainer<T>, Vector2i> Pool = new ConcurentObjectDictionaryPool<PooledTerrainMeshContainer<T>, Vector2i>(5, (CellInGroupCount) => new PooledTerrainMeshContainer<T>(CellInGroupCount));

        private PooledTerrainMeshContainer(Vector2i CellInGroupCount)
        {
            Mesh = new T();
            Mesh.Initialize(CellInGroupCount);
        }

        public static PooledTerrainMeshContainer<T> CreateDirty(Vector2i CellInGroupCount)
        {
            var obj = Pool.GetObject(CellInGroupCount);
            return obj;
        }

        public void Dispose()
        {
            if (IsDisposable)
            {
                (Mesh as IDisposable)?.Dispose();
            }
            Pool.ReturnObject(this, this.CellInGroupCount);
        }

        public void Clear(Bounds meshBounds)
        {
            Mesh.Clear(meshBounds);
        }

        public void AddQuad(Vector2i cellInGroup, Vector3i x0y0, Vector3i x0y1, Vector3i x1y0, Vector3i x1y1, bool flipSides)
        {
            Mesh.AddQuad(cellInGroup, x0y0, x0y1, x1y0, x1y1, flipSides);
        }

        public void AddWallX(Vector2i cellInGroup, Vector3i a, Vector3i b, Vector3i b_lower, Vector3i a_lower)
        {
            Mesh.AddWallX(cellInGroup, a, b, b_lower, a_lower);
        }

        public void AddWallY(Vector2i cellInGroup, Vector3i a, Vector3i b, Vector3i b_lower, Vector3i a_lower)
        {
            Mesh.AddWallY(cellInGroup, a, b, b_lower, a_lower);
        }

        public void Initialize(Vector2i CellInGroupCount)
        {
            throw new InvalidOperationException("Cannot initialize pooled mesh.");
        }
    }
}