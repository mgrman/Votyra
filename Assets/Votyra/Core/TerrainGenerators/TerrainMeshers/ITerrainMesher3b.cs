using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public interface ITerrainMesher3b
    {
        void AddCell(Vector3i cellInGroup);

        IPooledTerrainMesh GetResultingMesh();

        void Initialize(IImage3b image);

        void InitializeGroup(Vector3i group);
    }
}