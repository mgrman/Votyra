using System;
using System.Threading;
using UnityEngine;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Unity.MeshUpdaters;

namespace Votyra.Core.Unity.TerraingGroupGenerator
{
    public abstract class TerrainGroupGeneratorManager2i : ITerrainGroupGeneratorManager2i
    {
        protected readonly CancellationTokenSource Cts;
        protected readonly ITerrainMesher TerrainMesher;
        protected readonly Vector2i Group;
        protected readonly IPooledTerrainMesh PooledMesh;
        protected readonly Range2i Range;
        protected readonly CancellationToken Token;
        private readonly Vector2i _cellInGroupCount;
        protected readonly Func<GameObject> UnityDataFactory;

        private IFrameData2i _contextToProcess;

        protected GameObject UnityData;

        private bool _updatedOnce;
        protected ITerrainMeshUpdater MeshUpdater;

        public TerrainGroupGeneratorManager2i(Vector2i cellInGroupCount, Func<GameObject> unityDataFactory,
            Vector2i group, CancellationToken token, IPooledTerrainMesh pooledMesh, ITerrainMesher terrainMesher,
            ITerrainMeshUpdater meshUpdater)
        {
            _cellInGroupCount = cellInGroupCount;
            UnityDataFactory = unityDataFactory;
            Group = group;
            Range = Range2i.FromMinAndSize(group * cellInGroupCount, cellInGroupCount);
            Cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            Token = Cts.Token;

            PooledMesh = pooledMesh;
            TerrainMesher = terrainMesher;
            MeshUpdater = meshUpdater;
        }

        protected IFrameData2i ContextToProcess
        {
            get => _contextToProcess;
            set
            {
                _contextToProcess?.Deactivate();
                _contextToProcess = value;
                _contextToProcess?.Activate();
            }
        }

        public void Update(IFrameData2i context)
        {
            if (Token.IsCancellationRequested)
                return;

            if (_updatedOnce && !context.InvalidatedArea.Overlaps(Range))
                return;
            _updatedOnce = true;

            ContextToProcess = context;

            UpdateGroup();
        }

        public virtual void Dispose()
        {
            ContextToProcess = null;
            Cts.Cancel();
            PooledMesh.Dispose();
        }

        protected IFrameData2i GetFrameDataWithOwnership()
        {
            var contextToProcess = _contextToProcess;
            _contextToProcess = null;
            return contextToProcess;
        }

        protected abstract void UpdateGroup();


        protected void UpdateTerrainMesh(IFrameData2i context)
        {
            PooledMesh.Mesh.Reset(Area3f.FromMinAndSize((Group * _cellInGroupCount).ToVector3f(context.RangeZ.Min),
                _cellInGroupCount.ToVector3f(context.RangeZ.Size)));
            TerrainMesher.UpdateMesh(PooledMesh.Mesh, Group, context.Image);
            PooledMesh.Mesh.FinalizeMesh();
        }

        protected void UpdateUnityMesh(ITerrainMesh unityMesh)
        {
            if (UnityData == null)
                UnityData = UnityDataFactory();

            MeshUpdater.SetUnityMesh(unityMesh, UnityData);
        }
    }
}