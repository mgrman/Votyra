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
        private Line2f _raycastLine;
        private Vector2f _rayDirection;
        private Vector2i _fromGroup;
        private Vector2i _toGroup;
        private Ray3f _cameraRay;
        private IImage2f _image;
        private readonly ITerrainGeneratorManager2i _manager;
        private IMask2e _mask;
        private bool _wasValidMesh;

        public Terrain2fRaycaster(ITerrainConfig terrainConfig, ITerrainGeneratorManager2i manager, ITerrainVertexPostProcessor terrainVertexPostProcessor = null)
            : base(terrainConfig, terrainVertexPostProcessor)
        {
            _manager = manager;
        }

        public override Vector3f Raycast(Ray3f cameraRay)
        {
            try
            {
                _wasValidMesh = false;
                _cameraRay = cameraRay;

                return base.Raycast(cameraRay);
            }
            catch (StopException)
            {
                return Vector3f.NaN;
            }
        }

        protected override Vector3f RaycastGroup(Line2f line, Vector2i group)
        {
            var mesh = _manager.GetMeshForGroup(group);

            if (mesh == null && _wasValidMesh)
            {
                throw new StopException();
            }

            if (mesh == null)
                return Vector3f.NaN;

            _wasValidMesh = true;
            var vertices = mesh.Vertices;
            for (var i = 0; i < vertices.Count; i += 3)
            {
                var a = vertices[i];
                var b = vertices[i + 1];
                var c = vertices[i + 2];
                var triangle = new Triangle3f(a, b, c);
                var res = triangle.Intersect(_cameraRay);
                if (res.HasValue)
                    return res.Value;
            }

            return Vector3f.NaN;
        }

        private class StopException : Exception
        {
        }
    }
}