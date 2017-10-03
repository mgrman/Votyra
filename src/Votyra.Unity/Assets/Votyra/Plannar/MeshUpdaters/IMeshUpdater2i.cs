using System.Collections.Generic;
using Votyra.Core.Models;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Plannar.MeshUpdaters
{
    public interface IMeshUpdater2i
    {
        IReadOnlySet<Vector2i> ExistingGroups { get; }

        void UpdateMesh(IMeshContext options, IReadOnlyDictionary<Vector2i, ITerrainMesh2i> terrainMeshes, IEnumerable<Vector2i> toKeepGroups);
    }
}
