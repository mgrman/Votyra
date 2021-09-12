using System;
using System.Threading;
using UnityEngine;
using Votyra.Core.MeshUpdaters;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.Utils;

namespace Votyra.Core
{
    public class SyncTerrainGroupGeneratorManager2i : TerrainGroupGeneratorManager2i
    {
        public SyncTerrainGroupGeneratorManager2i(Vector2i cellInGroupCount, Func<GameObject> unityDataFactory,
            Vector2i group, CancellationToken token, IPooledTerrainMesh pooledMesh, ITerrainMesher terrainMesher,
            ITerrainMeshUpdater meshUpdater)
            : base(cellInGroupCount, unityDataFactory, group, token, pooledMesh, terrainMesher, meshUpdater)
        {
        }

        public override void Dispose()
        {
            base.Dispose();
            MeshUpdater.DestroyMesh(UnityData);
            UnityData.Destroy();
        }


        protected override void UpdateGroup()
        {
            if (Token.IsCancellationRequested)
                return;

            if (ContextToProcess != null)
            {
                UpdateGroup(ContextToProcess, Token);
                ContextToProcess = null;
            }
        }

        private void UpdateGroup(IFrameData2i context, CancellationToken token)
        {
            if (context == null)
                return;
            UpdateTerrainMesh(context);
            if (Token.IsCancellationRequested)
                return;
            UpdateUnityMesh(PooledMesh.Mesh);
        }
    }
}