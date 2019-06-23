using System;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core
{
    public class TerrainGroupGeneratorManager2i : ITerrainGroupGeneratorManager2i
    {
        private readonly Action<ITerrainMesh2f, Vector2i, IImage2f, IMask2e> _generateUnityMesh;
        private readonly Vector2i _cellInGroupCount;
        private readonly ITerrainMesh2f _pooledMesh;
        
        private Range2i _range;
        private Vector2i _group;

        public Vector2i Group
        {
            get => _group;
            set
            {
                if (value == _group)
                    return;

                _group = value;
                _range = Range2i.FromMinAndSize(value * _cellInGroupCount, _cellInGroupCount);
            }
        }

        private bool _updatedOnce;
        private bool _stopped;

        public TerrainGroupGeneratorManager2i(Vector2i cellInGroupCount, ITerrainMesh2f pooledMesh, Action<ITerrainMesh2f, Vector2i, IImage2f, IMask2e> generateUnityMesh)
        {
            _cellInGroupCount = cellInGroupCount;

            _pooledMesh = pooledMesh;
            _generateUnityMesh = generateUnityMesh;
        }

        public ITerrainMesh2f Mesh => _pooledMesh;

        public void Update(ArcResource<IFrameData2i> context, Action<Vector2i> onFinish)
        {
            if (_updatedOnce && !context.Value.InvalidatedArea.Overlaps(_range))
            {
                context.Dispose();
                return;
            }

            _updatedOnce = true;

            if (context.Value != null)
            {
                _pooledMesh.Reset(Area3f.FromMinAndSize((_group * _cellInGroupCount).ToVector3f(context.Value.RangeZ.Min), _cellInGroupCount.ToVector3f(context.Value.RangeZ.Size)));
                _generateUnityMesh(_pooledMesh, _group, context.Value.Image, context.Value.Mask);
                _pooledMesh.FinalizeMesh();
            }

            context.Dispose();
            onFinish?.Invoke(_group);
        }
    }
}