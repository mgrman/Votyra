using System;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.ImageSamplers;
using Votyra.Core.TerrainGenerators;
using Votyra.Core.Images;

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

        protected IImage2f _image;
        protected Vector2i _groupPosition;
        protected float _minZ;
        protected Vector3f _bounds_size;
        protected IPooledTerrainMesh2i _pooledMesh;
        protected ITerrainMesh2i _mesh;

        public void Initialize(IImage2f image)
        {
            _image = image;

            this._minZ = _image.RangeZ.min;
            this._bounds_size = new Vector3f(_cellInGroupCount.x, _cellInGroupCount.y, _image.RangeZ.Size);
        }

        public void InitializeGroup(Vector2i group)
        {
            var bounds = new Rect3f(new Vector3f
             (
                  (group.x * _cellInGroupCount.x),
                  (group.y * _cellInGroupCount.y),
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
            var pos_x0y0 = new Vector3f(position.x, position.y, data.x0y0);
            var pos_x0y1 = new Vector3f(position.x, position.y + 1, data.x0y1);
            var pos_x1y0 = new Vector3f(position.x + 1, position.y, data.x1y0);
            var pos_x1y1 = new Vector3f(position.x + 1, position.y + 1, data.x1y1);

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