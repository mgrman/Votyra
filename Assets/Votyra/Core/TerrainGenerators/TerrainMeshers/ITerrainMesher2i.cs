using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public interface ITerrainMesher2i
    {
        void AddCell(Vector2i cellInGroup);

        IPooledTerrainMesh2i GetResultingMesh();

        void Initialize(IImage2f image);

        void InitializeGroup(Vector2i group);
    }
}