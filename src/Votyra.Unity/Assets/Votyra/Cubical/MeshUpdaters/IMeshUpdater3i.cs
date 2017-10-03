using System.Collections.Generic;
using Votyra.Models;
using Votyra.TerrainGenerators.TerrainMeshers.TerrainMeshes;

namespace Votyra.Cubical.MeshUpdaters
{
    public interface IMeshUpdater3i
    {
        IReadOnlySet<Vector3i> ExistingGroups { get; }

        void UpdateMesh(IMeshContext options, IReadOnlyDictionary<Vector3i, ITerrainMesh> terrainMeshes, IEnumerable<Vector3i> toKeepGroups);
    }
}
