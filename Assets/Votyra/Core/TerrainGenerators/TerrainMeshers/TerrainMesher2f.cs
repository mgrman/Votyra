using System;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;
using Zenject;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public class TerrainMesher2f : ITerrainMesher2f
    {
        protected const int QuadToTriangles = 2;
        protected readonly Vector2i _cellInGroupCount;
        protected readonly ITerrainVertexPostProcessor _vertexPostProcessor;
        protected readonly ITerrainUVPostProcessor _uvPostProcessor;

        protected Vector3f _bounds_size;
        protected Vector2i _groupPosition;
        protected IImage2f _image;
        protected IMask2e _mask;
        protected ITerrainMesh _mesh;
        protected Height1f _minZ;
        protected IPooledTerrainMesh _pooledMesh;

        public TerrainMesher2f(ITerrainConfig terrainConfig, [InjectOptional] ITerrainVertexPostProcessor vertexPostProcessor, [InjectOptional] ITerrainUVPostProcessor uvPostProcessor)
        {
            _vertexPostProcessor = vertexPostProcessor;
            _uvPostProcessor = uvPostProcessor;
            _cellInGroupCount = terrainConfig.CellInGroupCount.XY;
        }

        public virtual Range2i AdjustAreaOfInfluenceOfInvalidatedArea(Range2i invalidatedArea)
        {
            return invalidatedArea.ExtendBothDirections(1);
        }

        protected virtual int QuadsPerCell => 1;
        protected virtual int TrianglesPerCell => QuadsPerCell * QuadToTriangles;
        protected virtual int TriangleCount => _cellInGroupCount.AreaSum * TrianglesPerCell;

        public virtual void AddCell(Vector2i cellInGroup)
        {
            Vector2i cell = cellInGroup + _groupPosition;

            Vector2i position = _groupPosition + cellInGroup;

            var data = _image.SampleCell(cell);
            var mask = _mask.SampleCell(cell);

            _mesh.AddQuad(position.ToVector2f(), data, mask);
        }


        public IPooledTerrainMesh GetResultingMesh(Vector2i group,IImage2f image, IMask2e mask)
        {
            _image = image;
            _mask = mask;

            this._minZ = _image.RangeZ.Min;
            this._bounds_size = new Vector2f(_cellInGroupCount.X, _cellInGroupCount.Y).ToVector3f(_image.RangeZ.Size);
            var bounds = Area3f.FromMinAndSize(new Vector2f((group.X * _cellInGroupCount.X), (group.Y * _cellInGroupCount.Y)).ToVector3f(_minZ), _bounds_size);

            this._groupPosition = _cellInGroupCount * group;

            this._pooledMesh = PooledTerrainMeshWithFixedCapacityContainer<FixedTerrainMesh2i>.CreateDirty(this.TriangleCount);
            // this._pooledMesh = PooledTerrainMeshContainer<ExpandingTerrainMesh>.CreateDirty();
            this._mesh = this._pooledMesh.Mesh;
            _mesh.Clear(bounds, _vertexPostProcessor == null ? (Func<Vector3f, Vector3f>) null : _vertexPostProcessor.PostProcessVertex, _uvPostProcessor == null ? (Func<Vector2f, Vector2f>) null : _uvPostProcessor.ProcessUV);


            _cellInGroupCount.ToRange2i().ForeachPointExlusive(AddCell);
            _pooledMesh.FinalizeMesh();
            return _pooledMesh;
        }
    }
}