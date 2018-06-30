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
        protected readonly IImageSampler2i _imageSampler;
        protected readonly Vector2i _cellInGroupCount;
        protected readonly int _triangleCount;

        protected virtual int TrianglesPerCell => 2;

        protected IImage2i _image;
        protected Vector2i _groupPosition;
        protected float _minZ;
        protected Vector3f _bounds_size;
        protected IPooledTerrainMesh _pooledMesh;
        protected ITerrainMesh _mesh;

        public TerrainMesher2i(ITerrainConfig terrainConfig, IImageSampler2i imageSampler)
        {
            _imageSampler = imageSampler;
            _cellInGroupCount = terrainConfig.CellInGroupCount.XY;
            _triangleCount = _cellInGroupCount.AreaSum * TrianglesPerCell;
        }

        public void Initialize(IImage2i image)
        {
            _image = image;

            this._minZ = _image.RangeZ.Min;
            this._bounds_size = new Vector3f(_cellInGroupCount.X, _cellInGroupCount.Y, _image.RangeZ.Size);
        }

        public void InitializeGroup(Vector2i group)
        {
            var bounds = Range3f.FromMinAndSize(new Vector3f
             (
                  (group.X * _cellInGroupCount.X),
                  (group.Y * _cellInGroupCount.Y),
                 _minZ
             ), _bounds_size);

            this._groupPosition = _cellInGroupCount * group;

            this._pooledMesh = PooledTerrainMeshWithFixedCapacityContainer<FixedTerrainMesh2i>.CreateDirty(this._triangleCount);
            // this._pooledMesh = PooledTerrainMeshContainer<ExpandingTerrainMesh>.CreateDirty();
            this._mesh = this._pooledMesh.Mesh;
            _mesh.Clear(bounds);
        }

        public virtual void AddCell(Vector2i cellInGroup)
        {
            Vector2i cell = cellInGroup + _groupPosition;

            Vector2i position = _groupPosition + cellInGroup;

            SampledData2i data = _imageSampler.Sample(_image, cell);

            _mesh.AddQuad(position, data);
        }

        public IPooledTerrainMesh GetResultingMesh()
        {
            _pooledMesh.FinalizeMesh();
            return _pooledMesh;
        }
    }
}