using System;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;
using Zenject;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public class DynamicTerrainMesher2f : ITerrainMesher2f
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

        public DynamicTerrainMesher2f(ITerrainConfig terrainConfig, [InjectOptional] ITerrainVertexPostProcessor vertexPostProcessor, [InjectOptional] ITerrainUVPostProcessor uvPostProcessor)
        {
            _vertexPostProcessor = vertexPostProcessor;
            _uvPostProcessor = uvPostProcessor;
            _cellInGroupCount = terrainConfig.CellInGroupCount.XY;
        }

        protected virtual int QuadsPerCell => 1;
        protected virtual int TrianglesPerCell => QuadsPerCell * QuadToTriangles;
        protected virtual int TriangleCount => _cellInGroupCount.AreaSum * TrianglesPerCell;

        public virtual Range2i AdjustAreaOfInfluenceOfInvalidatedArea(Range2i invalidatedArea)
        {
            return invalidatedArea.ExtendBothDirections(1);
        }

        public void AddGroup()
        {
            var samples = _image.SampleArea(Range2i.FromMinAndSize(_groupPosition, _cellInGroupCount+Vector2i.One));

            for (int ix = 0; ix < _cellInGroupCount.X; ix++)
            {
                for (int iy = 0; iy < _cellInGroupCount.Y; iy++)
                {
                    var localCell = new Vector2i(ix, iy);
                    Vector2i cell = localCell+ _groupPosition;

                    var x0y0 = new Vector3f(cell.X + 0, cell.Y + 0, samples[ix+ 0, iy+ 0].RawValue);
                    var x0y1 = new Vector3f(cell.X + 0, cell.Y + 1, samples[ix+ 0, iy+ 1].RawValue);
                    var x1y0 = new Vector3f(cell.X + 1, cell.Y + 0, samples[ix+ 1, iy+ 0].RawValue);
                    var x1y1 = new Vector3f(cell.X + 1, cell.Y + 1, samples[ix+ 1, iy+ 1].RawValue);

                    _mesh.AddQuad(x0y0,x0y1,x1y0,x1y1);
                }
            }
            
            samples.Dispose();
        }


        public IPooledTerrainMesh GetResultingMesh(Vector2i group, IImage2f image, IMask2e mask)
        {
            _image = image;
            _mask = mask;

            this._minZ = _image.RangeZ.Min;
            this._bounds_size = new Vector2f(_cellInGroupCount.X, _cellInGroupCount.Y).ToVector3f(_image.RangeZ.Size);

            var bounds = Area3f.FromMinAndSize(new Vector2f((group.X * _cellInGroupCount.X), (group.Y * _cellInGroupCount.Y)).ToVector3f(_minZ), _bounds_size);

            this._groupPosition = _cellInGroupCount * group;

            this._pooledMesh = PooledTerrainMeshContainer<ExpandingUnityTerrainMesh>.CreateDirty();
            // this._pooledMesh = PooledTerrainMeshContainer<ExpandingTerrainMesh>.CreateDirty();
            this._mesh = this._pooledMesh.Mesh;
            _mesh.Clear(bounds, _vertexPostProcessor == null ? (Func<Vector3f, Vector3f>) null : _vertexPostProcessor.PostProcessVertex, _uvPostProcessor == null ? (Func<Vector2f, Vector2f>) null : _uvPostProcessor.ProcessUV);

            AddGroup();
            
            _pooledMesh.FinalizeMesh();
            return _pooledMesh;
        }

    }
}