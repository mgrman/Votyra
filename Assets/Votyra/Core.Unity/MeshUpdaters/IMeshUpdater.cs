using System.Collections.Generic;
using Votyra.Core.Models;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.MeshUpdaters
{
    public interface IMeshUpdater<TKey>
    {
        IReadOnlySet<TKey> ExistingGroups { get; }

        void UpdateMesh(IMeshContext options, IReadOnlyDictionary<TKey, ITerrainMesh> terrainMeshes, IReadOnlySet<TKey> toKeepGroups);
    }
}