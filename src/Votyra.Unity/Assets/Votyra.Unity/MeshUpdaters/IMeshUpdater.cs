using System.Collections.Generic;
using Votyra.Common.Models;

namespace Votyra.Unity.MeshUpdaters
{
    public interface IMeshUpdater
    {
        void UpdateMesh(MeshOptions options, IReadOnlyDictionary<Vector2i, TerrainMeshers.TriangleMesh.ITriangleMesh> terrainMeshes);
    }
}