using System;
using System.Threading.Tasks;
using UniRx.Async;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Queueing;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Unity;

namespace Votyra.Core
{
    public class AsyncTerrainGroupGeneratorManager2i : TerrainGroupGeneratorManager2i
    {
        private readonly TaskQueue<ArcResource<IFrameData2i>> _taskQueue;

        public AsyncTerrainGroupGeneratorManager2i(Vector2i cellInGroupCount, ITerrainGameObject gameObjectPool, ITerrainMesh pooledMesh, Action<ITerrainMesh, Vector2i, IImage2f, IMask2e> generateUnityMesh)
            : base(cellInGroupCount, gameObjectPool, pooledMesh, generateUnityMesh)
        {
            _taskQueue = new TaskQueue<ArcResource<IFrameData2i>>("AsyncTerrainGroupGenerator",UpdateGroupAsync);
        }

        protected override void UpdateGroup(ArcResource<IFrameData2i> context)
        {
            _taskQueue.QueueNew(context);
        }

        private async Task UpdateGroupAsync(ArcResource<IFrameData2i> context)
        {
            UpdateTerrainMesh(context.Value);
            if (IsStopped)
                return;
            await UniTask.SwitchToMainThread();
            if (IsStopped)
                return;
            UpdateUnityMesh();
        }

        public override void Stop()
        {
            base.Stop();
            _taskQueue.Stop();
            if (_gameObjectPool.IsInitialized)
            {
                _gameObjectPool.SetActiveAsync(false);
            }
        }
    }
}