using System;
using System.Threading;
using Votyra.Core.Images;
using Votyra.Core.MeshUpdaters;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Unity;

namespace Votyra.Core
{
    public abstract class TerrainGroupGeneratorManager2i : ITerrainGroupGeneratorManager2i
    {
        protected readonly Action<ITerrainMesh, Vector2i, IImage2f, IMask2e> _generateUnityMesh;
        protected Vector2i _group;
        protected readonly Vector2i _cellInGroupCount;
        protected readonly ITerrainMesh _pooledMesh;
        protected Range2i _range;
        protected readonly ITerrainGameObject _gameObjectPool;

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

        public TerrainGroupGeneratorManager2i(Vector2i cellInGroupCount, ITerrainGameObject gameObject, ITerrainMesh pooledMesh, Action<ITerrainMesh, Vector2i, IImage2f, IMask2e> generateUnityMesh)
        {
            _gameObjectPool = gameObject;
            _cellInGroupCount = cellInGroupCount;

            _pooledMesh = pooledMesh;
            _generateUnityMesh = generateUnityMesh;
        }

        protected bool IsStopped => _stopped;

        public ITerrainGameObject TerrainGameObject => _gameObjectPool;
        public ITerrainMesh Mesh => _pooledMesh;

        public void Update(ArcResource<IFrameData2i> context)
        {
            _stopped = false;

            if (_updatedOnce && !context.Value.InvalidatedArea.Overlaps(_range))
            {
                context.Dispose();
                return;
            }

            _updatedOnce = true;

            UpdateGroup(context);
        }

        public virtual void Stop()
        {
            _stopped = true;
            _updatedOnce = false;
        }

        protected abstract void UpdateGroup(ArcResource<IFrameData2i> context);

        protected void UpdateTerrainMesh(IFrameData2i context)
        {
            _pooledMesh.Reset(Area3f.FromMinAndSize((_group * _cellInGroupCount).ToVector3f(context.RangeZ.Min), _cellInGroupCount.ToVector3f(context.RangeZ.Size)));
            _generateUnityMesh(_pooledMesh, _group, context.Image, context.Mask);
            _pooledMesh.FinalizeMesh();
        }

        protected void UpdateUnityMesh()
        {
            if (IsStopped)
            {
                return;
            }

            if (!_gameObjectPool.IsInitialized)
            {
                _gameObjectPool.Initialize();
            }

            _gameObjectPool.SetActive(true);

            _pooledMesh.SetUnityMesh(_gameObjectPool);
        }
    }
}