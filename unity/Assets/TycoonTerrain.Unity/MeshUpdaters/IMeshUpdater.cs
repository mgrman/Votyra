using System.Collections.Generic;

namespace TycoonTerrain.Unity.MeshUpdaters
{
    public interface IMeshUpdater
    {
        void UpdateMesh(MeshOptions options, IList<TerrainMeshers.TriangleMesh.ITriangleMesh> terrainMeshes);
    }
}