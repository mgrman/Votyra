using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Unity;

namespace Votyra.Core
{
    public interface ITerrainGroupGeneratorManager2i
    {
        void Stop();
        Vector2i Group { get; set; }
        ITerrainGameObject TerrainGameObject { get; }
        ITerrainMesh Mesh { get; }
        void Update(ArcResource<IFrameData2i> context);
    }
}