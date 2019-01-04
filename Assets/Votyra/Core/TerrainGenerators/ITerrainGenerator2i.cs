using System;
using System.Collections.Generic;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators
{
    public interface ITerrainGenerator2i
    {
        IPooledTerrainMesh Generate(Vector2i group, IImage2f image, IMask2e mask);
    }
}