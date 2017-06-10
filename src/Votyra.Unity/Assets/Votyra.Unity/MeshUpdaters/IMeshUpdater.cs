using System.Collections.Generic;
using Votyra.Common.Models;

namespace Votyra.Unity.MeshUpdaters
{
    public interface IMeshUpdater
    {
        IEnumerable<Vector2i> ExistingGroups { get; }

        void UpdateMesh(IMeshContext options, IReadOnlyDictionary<Vector2i, TerrainMeshers.TriangleMesh.ITerrainMesh> terrainMeshes, IEnumerable<Vector2i> toKeepGroups);
    }
}