using System;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core
{
    public class SyncTerrainGroupGeneratorManager2i : TerrainGroupGeneratorManager2i
    {
        public SyncTerrainGroupGeneratorManager2i(Vector2i cellInGroupCount, ITerrainMesh2f pooledMesh, Action<ITerrainMesh2f, Vector2i, IImage2f, IMask2e> generateUnityMesh)
            : base(cellInGroupCount, pooledMesh, generateUnityMesh)
        {
        }

        protected override void UpdateGroup(ArcResource<IFrameData2i> context, Action<Vector2i, ITerrainMesh2f> onFinish)
        {
            if (IsStopped)
            {
                context.Dispose();
                return;
            }

            if (context.Value != null)
            {
                UpdateTerrainMesh(context.Value);
            }

            context.Dispose();
            onFinish?.Invoke(_group,Mesh);
        }
    }
}