using System;
using System.Threading;
using System.Threading.Tasks;
using UniRx.Async;
using UnityEngine;
using Votyra.Core.MeshUpdaters;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.Utils;

namespace Votyra.Core
{
    public class AsyncTerrainGroupGeneratorManager2i : TerrainGroupGeneratorManager2i
    {
        private Task _activeTask = Task.CompletedTask;

        public AsyncTerrainGroupGeneratorManager2i(Vector2i cellInGroupCount, Func<GameObject> unityDataFactory,
            Vector2i group, CancellationToken token, IPooledTerrainMesh pooledMesh, ITerrainMesher terrainMesher,
            ITerrainMeshUpdater meshUpdater)
            : base(cellInGroupCount, unityDataFactory, group, token, pooledMesh, terrainMesher, meshUpdater)
        {
        }

        protected override void UpdateGroup()
        {
            if (Token.IsCancellationRequested)
                return;

            if (!_activeTask.IsCompleted || ContextToProcess == null)
                return;

            var context = GetFrameDataWithOwnership();

            _activeTask = Task.Run(async () =>
            {
                try
                {
                    await UpdateGroupAsync(context, Token);
                }
                finally
                {
                    context?.Deactivate();
                }
            });
            _activeTask.ConfigureAwait(false);

            _activeTask.ContinueWith(t => UpdateGroup());
        }


        private async Task UpdateGroupAsync(IFrameData2i context, CancellationToken token)
        {
            if (context == null)
                return;
            UpdateTerrainMesh(context);
            if (Token.IsCancellationRequested)
                return;
            await UniTask.SwitchToMainThread();
            if (Token.IsCancellationRequested)
                return;
            UpdateUnityMesh(PooledMesh.Mesh);
        }


        public override void Dispose()
        {
            base.Dispose();
            MainThreadUtils.RunOnMainThreadAsync(() =>
            {
                MeshUpdater.DestroyMesh(UnityData);
                UnityData.Destroy();
            });
        }
    }
}