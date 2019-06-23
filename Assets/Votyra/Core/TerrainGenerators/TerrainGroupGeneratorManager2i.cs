using System;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core
{
    public abstract class TerrainGroupGeneratorManager2i : ITerrainGroupGeneratorManager2i
    {
        protected readonly Action<ITerrainMesh2f, Vector2i, IImage2f, IMask2e> _generateUnityMesh;
        protected Vector2i _group;
        protected readonly Vector2i _cellInGroupCount;
        protected readonly ITerrainMesh2f _pooledMesh;
        protected Range2i _range;

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

        protected bool IsStopped => _stopped;
        
        public ITerrainMesh2f Mesh => _pooledMesh;

        public void Update(ArcResource<IFrameData2i> context, Action<Vector2i, ITerrainMesh2f> onFinish)
        {
            _stopped = false;

            if (_updatedOnce && !context.Value.InvalidatedArea.Overlaps(_range))
            {
                context.Dispose();
                return ;
            }

            _updatedOnce = true;

            UpdateGroup(context, onFinish);
            return ;
        }

        public virtual void Stop()
        {
            _stopped = true;
            _updatedOnce = false;
        }

        protected abstract void UpdateGroup(ArcResource<IFrameData2i> context, Action<Vector2i, ITerrainMesh2f> onFinish);

        protected void UpdateTerrainMesh(IFrameData2i context)
        {
            _pooledMesh.Reset(Area3f.FromMinAndSize((_group * _cellInGroupCount).ToVector3f(context.RangeZ.Min), _cellInGroupCount.ToVector3f(context.RangeZ.Size)));
            _generateUnityMesh(_pooledMesh, _group, context.Image, context.Mask);
            _pooledMesh.FinalizeMesh();
        }
    }
}