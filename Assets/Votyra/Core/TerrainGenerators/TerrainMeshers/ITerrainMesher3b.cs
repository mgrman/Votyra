using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public interface ITerrainMesher3B
    {
        void AddCell(Vector3i cellInGroup);

        void Initialize(IImage3B image);

        void InitializeGroup(Vector3i group, IGeneralMesh pooledTerrainMesh);
    }
}
