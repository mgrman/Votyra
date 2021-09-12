using UnityEngine;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.Unity.MeshUpdaters
{
    public interface ITerrainMeshUpdater
    {
        void SetUnityMesh(ITerrainMesh triangleMesh, GameObject unityData);
        void DestroyMesh(GameObject unityData);
    }
}