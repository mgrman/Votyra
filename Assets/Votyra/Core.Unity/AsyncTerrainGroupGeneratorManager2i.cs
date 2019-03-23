using System;
using System.Threading;
using System.Threading.Tasks;
using UniRx.Async;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Unity;

namespace Votyra.Core
{
    public class AsyncTerrainGroupGeneratorManager2i : TerrainGroupGeneratorManager2i
    {
        private Task _activeTask = Task.CompletedTask;

        private ArcResource<IFrameData2i> _queuedUpdate = null;

        public AsyncTerrainGroupGeneratorManager2i(Vector2i cellInGroupCount, ITerrainGameObject gameObjectPool, ITerrainMesh pooledMesh, Action<ITerrainMesh, Vector2i, IImage2f, IMask2e> generateUnityMesh)
            : base(cellInGroupCount, gameObjectPool, pooledMesh, generateUnityMesh)
        {
        }

        protected override void UpdateGroup(ArcResource<IFrameData2i> context)
        {
            if (IsStopped)
            {
                context.Dispose();
                return;
            }

            if (!_activeTask.IsCompleted)
            {
                if (_queuedUpdate != null)
                {
                    _queuedUpdate.Dispose();
                }

                _queuedUpdate = context;
                return;
            }

            _activeTask = Task.Run(async () =>
            {
                try
                {
                    await UpdateGroupAsync(context);
                }
                finally
                {
                    context.Dispose();
                }
            });

            if (_queuedUpdate != null)
            {
                UpdateGroup(_queuedUpdate);
            }
        }

        private async Task UpdateGroupAsync(ArcResource<IFrameData2i> context)
        {
            if (context == null)
                return;
            UpdateTerrainMesh(context.Value);
            if (IsStopped)
                return;
            await UniTask.SwitchToMainThread();
            if (IsStopped)
                return;
            UpdateUnityMesh();
        }
    }
}