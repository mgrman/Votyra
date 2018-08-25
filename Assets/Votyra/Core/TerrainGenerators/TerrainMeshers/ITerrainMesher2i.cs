using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public interface ITerrainMesher2i
    {
        void AddCell(Vector2i cellInGroup);

        IPooledTerrainMesh GetResultingMesh();

        void Initialize(IImage2i image, IMask2e mask);

        void InitializeGroup(Vector2i group);
    }
}