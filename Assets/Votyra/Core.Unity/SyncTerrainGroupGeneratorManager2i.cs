using System;
using System.Threading;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Unity;

namespace Votyra.Core
{
    public class SyncTerrainGroupGeneratorManager2i : TerrainGroupGeneratorManager2i
    {
        public SyncTerrainGroupGeneratorManager2i(Vector2i cellInGroupCount, ITerrainGameObject gameObjectPool, ITerrainMesh pooledMesh, Action<ITerrainMesh, Vector2i, IImage2f, IMask2e> generateUnityMesh)
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

            if (context.Value != null)
            {
                UpdateTerrainMesh(context.Value);
                if (IsStopped)
                {
                    context.Dispose();
                    return;
                }
                UpdateUnityMesh();
            }

            context.Dispose();
        }
    }
}