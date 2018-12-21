using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Profiling;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Utils;

namespace Votyra.Core.MeshUpdaters
{
    public interface ITerrainMeshConverter
    {
        UnityMesh GetUnityMesh(ITerrainMesh votyraMesh, UnityMesh existingUnityMesh);
    }
}