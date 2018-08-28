using System;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public class TerrainMesher2i : ITerrainMesher2i
    {
        protected const int QuadToTriangles = 2;
        protected readonly Vector2i _cellInGroupCount;
        protected readonly IImageSampler2i _imageSampler;

        protected Vector3f _bounds_size;
        protected Vector2i _groupPosition;
        protected IImage2i _image;
        protected IMask2e _mask;
        protected ITerrainMesh _mesh;
        protected Height _minZ;
        protected IPooledTerrainMesh _pooledMesh;

        public TerrainMesher2i(ITerrainConfig terrainConfig, IImageSampler2i imageSampler)
        {
            _imageSampler = imageSampler;
            _cellInGroupCount = terrainConfig.CellInGroupCount.XY;
        }

        protected virtual int QuadsPerCell => 1;
        protected virtual int TrianglesPerCell => QuadsPerCell * QuadToTriangles;
        protected virtual int TriangleCount => _cellInGroupCount.AreaSum * TrianglesPerCell;

        public virtual void AddCell(Vector2i cellInGroup)
        {
            Vector2i cell = cellInGroup + _groupPosition;

            Vector2i position = _groupPosition + cellInGroup;

            var data = _imageSampler.Sample(_image, cell);
            var mask = _imageSampler.Sample(_mask, cell);

            _mesh.AddQuad(position, data, mask);
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
            _mesh.Clear(bounds);
        }
    }
}