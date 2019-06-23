using System;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core
{
    public interface ITerrainGroupGeneratorManager2i
    {
        void Stop();
        Vector2i Group { get; set; }
        ITerrainMesh2f Mesh { get; }
        void Update(ArcResource<IFrameData2i> context, Action<Vector2i, ITerrainMesh2f> onFinish);
    }
}