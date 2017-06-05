using System.Collections.Generic;
using Votyra.Common.Models;

namespace Votyra.Unity.MeshUpdaters
{
    public interface IMeshUpdater
    {
        IEnumerable<Vector2i> ExistingGroups { get; }

        void UpdateMesh(MeshOptions options, IReadOnlyDictionary<Vector2i, TerrainMeshers.TriangleMesh.ITriangleMesh> terrainMeshes, IEnumerable<Vector2i> toKeepGroups);
    }
}