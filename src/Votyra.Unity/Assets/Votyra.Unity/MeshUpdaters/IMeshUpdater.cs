using System.Collections.Generic;
using Votyra.Models;
using Votyra.TerrainGenerators.TerrainMeshers.TerrainMeshes;

namespace Votyra.Unity.MeshUpdaters
{
    public interface IMeshUpdater
    {
        IReadOnlySet<Vector2i> ExistingGroups { get; }

        void UpdateMesh(IMeshContext options, IReadOnlyDictionary<Vector2i, ITerrainMesh2> terrainMeshes, IEnumerable<Vector2i> toKeepGroups);
    }
}