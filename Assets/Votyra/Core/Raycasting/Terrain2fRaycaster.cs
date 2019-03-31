using System;
using Votyra.Core.Images;
using Votyra.Core.Logging;
using Votyra.Core.Models;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Utils;

namespace Votyra.Core.Raycasting
{
    public sealed class Terrain2fRaycaster : BaseGroupRaycaster
    {
        private readonly ITerrainGeneratorManager2i _manager;

        public Terrain2fRaycaster(ITerrainConfig terrainConfig, ITerrainGeneratorManager2i manager, ITerrainVertexPostProcessor terrainVertexPostProcessor = null)
            : base(terrainConfig, terrainVertexPostProcessor)
        {
            _manager = manager;
        }

        protected override RaycastResult RaycastGroup(Line2f line, Vector2i group, Ray3f cameraRay)
        {
            var mesh = _manager.GetMeshForGroup(group);

            if (mesh == null)
                return RaycastResult.NoHit;

            var vertices = mesh.Vertices;
            for (var i = 0; i < vertices.Count; i += 3)
            {
                var a = vertices[i];
                var b = vertices[i + 1];
                var c = vertices[i + 2];
                var triangle = new Triangle3f(a, b, c);
                var res = triangle.Intersect(cameraRay);
                if (res.HasValue)
                    return new RaycastResult(res.Value);
            }

            return RaycastResult.NoHit;
        }
    }
}