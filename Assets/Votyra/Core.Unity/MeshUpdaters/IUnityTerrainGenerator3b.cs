using System.Collections.Generic;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators
{
    public interface IUnityTerrainGenerator3b
    {
        UnityMesh Generate(Vector3i group,IImage3b image);
    }
}