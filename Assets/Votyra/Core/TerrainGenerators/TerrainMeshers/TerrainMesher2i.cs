using System;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;
using Zenject;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public class TerrainMesher2i : ITerrainMesher2i
    {
        protected const int QuadToTriangles = 2;
        protected readonly Vector2i _cellInGroupCount;
        protected readonly ITerrainVertexPostProcessor _vertexPostProcessor;
        protected readonly ITerrainUVPostProcessor _uvPostProcessor;

        protected Vector3f _bounds_size;
        protected Vector2i _groupPosition;
        protected IImage2i _image;
        protected IMask2e _mask;
        protected ITerrainMesh _mesh;
        protected Height _minZ;
        protected IPooledTerrainMesh _pooledMesh;

        public TerrainMesher2i(ITerrainConfig terrainConfig, [InjectOptional] ITerrainVertexPostProcessor vertexPostProcessor, [InjectOptional] ITerrainUVPostProcessor uvPostProcessor)
        {
            _vertexPostProcessor = vertexPostProcessor;
            _uvPostProcessor = uvPostProcessor;
            _cellInGroupCount = terrainConfig.CellInGroupCount.XY;
        }

        protected virtual int QuadsPerCell => 1;
        protected virtual int TrianglesPerCell => QuadsPerCell * QuadToTriangles;
        protected virtual int TriangleCount => _cellInGroupCount.AreaSum * TrianglesPerCell;

        protected virtual Func<Vector3f?, Vector3f?> PostProcessVertices { get; } = null;

        public virtual void AddCell(Vector2i cellInGroup)
        {
            Vector2i cell = cellInGroup + _groupPosition;

            Vector2i position = _groupPosition + cellInGroup;

            var data = _image.SampleCell(cell);
            var mask = _mask.SampleCell(cell);

            _mesh.AddQuad(position.ToVector2f(), data, mask, PostProcessVertices);
        }

        public IPooledTerrainMesh GetResultingMesh()
        {
            _pooledMesh.FinalizeMesh();
            return _pooledMesh;
        }

        public virtual void Initialize(IImage2i image, IMask2e mask)
        {
            _image = image;
            _mask = mask;

            this._minZ = _image.RangeZ.Min;
            this._bounds_size = new Vector2f(_cellInGroupCount.X, _cellInGroupCount.Y)
                .ToVector3f(_image.RangeZ.Size);
        }

        public void InitializeGroup(Vector2i group)
        {
            var bounds = Range3f.FromMinAndSize(new Vector2f((group.X * _cellInGroupCount.X), (group.Y * _cellInGroupCount.Y))
                .ToVector3f(_minZ),
                _bounds_size);

            this._groupPosition = _cellInGroupCount * group;

            this._pooledMesh = PooledTerrainMeshWithFixedCapacityContainer<FixedTerrainMesh2i>.CreateDirty(this.TriangleCount);
            // this._pooledMesh = PooledTerrainMeshContainer<ExpandingTerrainMesh>.CreateDirty();
            this._mesh = this._pooledMesh.Mesh;
            _mesh.Clear(bounds, _vertexPostProcessor == null ? (Func<Vector3f, Vector3f>)null : _vertexPostProcessor.PostProcessVertex, _uvPostProcessor == null ? (Func<Vector2f, Vector2f>)null : _uvPostProcessor.ProcessUV);
        }
    }
}