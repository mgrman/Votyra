using Votyra.Core.Models;
using Votyra.Core.TerrainGenerators.TerrainMeshers;

namespace Votyra.Core.Raycasting
{
    public sealed class Terrain2fRaycaster : BaseGroupRaycaster
    {
        private readonly ITerrainRepository2i manager;

        public Terrain2fRaycaster(ITerrainConfig terrainConfig, ITerrainRepository2i manager, ITerrainVertexPostProcessor terrainVertexPostProcessor = null, IRaycasterAggregator raycasterAggregator = null)
            : base(terrainConfig, terrainVertexPostProcessor, raycasterAggregator)
        {
            this.manager = manager;
        }

        protected override Vector3f RaycastGroup(Vector2i group, Ray3f cameraRay)
        {
            var mesh = this.manager.TryGetValue(group);

            if (mesh == null)
            {
                return Vector3f.NaN;
            }

            var vertices = mesh.Vertices;
            for (var i = 0; i < vertices.Count; i += 3)
            {
                var a = vertices[i];
                var b = vertices[i + 1];
                var c = vertices[i + 2];
                var triangle = new Triangle3f(a, b, c);
                var res = triangle.Intersect(cameraRay);
                if (res.NoNan())
                {
                    return res;
                }
            }

            return Vector3f.NaN;
        }

        protected override float RaycastGroup(Vector2i group, Vector2f posXy)
        {
            var mesh = this.manager.TryGetValue(group);

            if (mesh == null)
            {
                return Vector1f.NaN;
            }

            var vertices = mesh.Vertices;
            for (var i = 0; i < vertices.Count; i += 3)
            {
                var a = vertices[i];
                var b = vertices[i + 1];
                var c = vertices[i + 2];
                var triangle = new Triangle3f(a, b, c);
                var res = triangle.BarycentricCoords(posXy);
                if (res.NoNan())
                {
                    return res;
                }
            }

            return Vector1f.NaN;
        }
    }
}
