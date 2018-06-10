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

        public TerrainMesher2i(ITerrainConfig terrainConfig, IImageSampler2i imageSampler)
        {
            _imageSampler = imageSampler;
            _cellInGroupCount = terrainConfig.CellInGroupCount.XY;
        }

        protected IImage2i _image;
        protected Vector2i _groupPosition;
        protected float _minZ;
        protected Vector3f _bounds_size;
        protected IPooledTerrainMesh2i _pooledMesh;
        protected ITerrainMesh2i _mesh;

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

            this._pooledMesh = PooledTerrainMesh2iContainer<FixedTerrainMesh2i>.CreateDirty(_cellInGroupCount);
            this._mesh = this._pooledMesh.Mesh;
            _mesh.Clear(bounds);
        }

        public virtual void AddCell(Vector2i cellInGroup)
        {
            Vector2i cell = cellInGroup + _groupPosition;

            Vector2i position = _groupPosition + cellInGroup;

            SampledData2i data = _imageSampler.Sample(_image, cell);

            // Debug.Log($"{minusXres_x1y0} {minusXres_x1y1}");
            var pos_x0y0 = new Vector3f(position.X, position.Y, data.x0y0);
            var pos_x0y1 = new Vector3f(position.X, position.Y + 1, data.x0y1);
            var pos_x1y0 = new Vector3f(position.X + 1, position.Y, data.x1y0);
            var pos_x1y1 = new Vector3f(position.X + 1, position.Y + 1, data.x1y1);

            _mesh.AddQuad(pos_x0y0, pos_x0y1, pos_x1y0, pos_x1y1, IsFlipped(data));
        }

        protected bool IsFlipped(SampledData2i sampleData)
        {
            var difMain = Math.Abs(sampleData.x0y0 - sampleData.x1y1);
            var difMinor = Math.Abs(sampleData.x1y0 - sampleData.x0y1);
            bool flip;
            if (difMain == difMinor)
            {
                var sumMain = sampleData.x0y0 + sampleData.x1y1;
                var sumMinor = sampleData.x1y0 + sampleData.x0y1;
                flip = sumMain < sumMinor;
            }
            else
            {
                flip = difMain < difMinor;
            }
            return flip;
        }

        public IPooledTerrainMesh2i GetResultingMesh()
        {
            return _pooledMesh;
        }
    }
}