using System;
using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Utils;

namespace Votyra.Core.Raycasting
{
    public sealed class Terrain2fRaycaster : BaseRaycaster
    {
        private readonly IImage2fProvider _image2FProvider;
        private readonly IMask2eProvider _mask2EProvider;
        private Ray3f _cameraRay;
        private float _directionXyMag;
        private IImage2f _image;
        private IMask2e _mask;
        private Vector2f _startXy;
        private Vector2i _cellInGroupCount;
        private TerrainGeneratorManager2i _manager;

        public Terrain2fRaycaster(IImage2fProvider image2FProvider, IMask2eProvider mask2eProvider, ITerrainConfig terrainConfig, TerrainGeneratorManager2i manager, ITerrainVertexPostProcessor terrainVertexPostProcessor = null)
            : base(terrainVertexPostProcessor)
        {
            _image2FProvider = image2FProvider;
            _mask2EProvider = mask2eProvider;
            _manager = manager;
            _cellInGroupCount = terrainConfig.CellInGroupCount.XY();
        }

        public override Vector3f? Raycast(Ray3f cameraRay)
        {
            try
            {
                _image = _image2FProvider.CreateImage();

                (_image as IInitializableImage)?.StartUsing();
                _mask = _mask2EProvider.CreateMask();
                (_mask as IInitializableImage)?.StartUsing();

                _cameraRay = cameraRay;

                _startXy = cameraRay.XY()
                    .Origin;
                _directionXyMag = cameraRay.Direction.XY()
                    .Magnitude();

                var result = base.Raycast(cameraRay);

                (_image as IInitializableImage)?.FinishUsing();
                _image = null;
                (_mask as IInitializableImage)?.FinishUsing();
                _mask = null;

                return result;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }

        protected override Vector3f? RaycastCell(Line2f line, Vector2i cell)
        {
            var group = cell / _cellInGroupCount;
            var pooledMesh = _manager.GetMeshForGroup(group);
            var mesh = pooledMesh?.Mesh as FixedUnityTerrainMesh2i;

            if (mesh == null)
            {
                return null;
            }

            foreach (var triangle in mesh.GetTriangles(cell % _cellInGroupCount))
            {
                var res = triangle.Intersect(_cameraRay);
                if (res.HasValue)
                {
                    return res.Value;
                }
            }

            return null;
        }
    }
}