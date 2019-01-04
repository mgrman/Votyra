using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Models;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.MeshUpdaters
{
    public interface IMeshUpdater
    {
        GameObject UpdateMesh(UnityMesh triangleMesh, GameObject unityData);
    }
}