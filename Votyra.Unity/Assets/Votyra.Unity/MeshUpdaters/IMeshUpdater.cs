using System.Collections.Generic;

namespace Votyra.Unity.MeshUpdaters
{
    public interface IMeshUpdater
    {
        void UpdateMesh(MeshOptions options, IList<TerrainMeshers.TriangleMesh.ITriangleMesh> terrainMeshes);
    }
}