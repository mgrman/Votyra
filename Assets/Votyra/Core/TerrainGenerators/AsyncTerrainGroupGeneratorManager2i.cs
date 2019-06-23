using System;
using System.Threading.Tasks;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Queueing;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core
{
    public class AsyncTerrainGroupGeneratorManager2i : TerrainGroupGeneratorManager2i
    {
        private readonly TaskQueue<Data> _taskQueue;

        public AsyncTerrainGroupGeneratorManager2i(Vector2i cellInGroupCount, ITerrainMesh2f pooledMesh, Action<ITerrainMesh2f, Vector2i, IImage2f, IMask2e> generateUnityMesh)
            : base(cellInGroupCount, pooledMesh, generateUnityMesh)
        {
            _taskQueue = new TaskQueue<Data>("AsyncTerrainGroupGenerator", UpdateGroupAsync);
        }

        protected override void UpdateGroup(ArcResource<IFrameData2i> context, Action<Vector2i, ITerrainMesh2f> onFinish)
        {
            _taskQueue.QueueNew(new Data(context, onFinish));
        }

        private async Task UpdateGroupAsync(Data data)
        {
            if (data.Context.Value != null)
            {
                UpdateTerrainMesh(data.Context.Value);
                data.OnFinish?.Invoke(_group, Mesh);
            }
        }

        public override void Stop()
        {
            base.Stop();
            _taskQueue.Stop();
        }

        private class Data : IDisposable
        {
            public Data(ArcResource<IFrameData2i> context, Action<Vector2i, ITerrainMesh2f> onFinish)
            {
                Context = context;
                OnFinish = onFinish;
            }

            public ArcResource<IFrameData2i> Context { get; }

            public Action<Vector2i, ITerrainMesh2f> OnFinish { get; }

            public void Dispose()
            {
                Context?.Dispose();
            }
        }
    }
}