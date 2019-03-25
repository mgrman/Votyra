using System;
using Votyra.Core.Images;
using Votyra.Core.Logging;
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
        private readonly Vector2i _cellInGroupCount;
        private float _directionXyMag;
        private IImage2f _image;
        private readonly ITerrainGeneratorManager2i _manager;
        private IMask2e _mask;
        private Vector2f _startXy;

        public Terrain2fRaycaster(IImage2fProvider image2FProvider, IMask2eProvider mask2eProvider, ITerrainConfig terrainConfig, ITerrainGeneratorManager2i manager, ITerrainVertexPostProcessor terrainVertexPostProcessor = null)
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
                StaticLogger.LogException(ex);
                return null;
            }
        }

        protected override Vector3f? RaycastCell(Line2f line, Vector2i cell)
        {
            var group = GetGroup(cell);
            var mesh = _manager.GetMeshForGroup(group);

            if (mesh == null)
                return RaycastCellUsingImage(line, cell);

            var boundsRange = mesh.MeshBounds.Z;
            var fromValue = GetRayValue(line.From);
            var toValue = GetRayValue(line.To);
            if (fromValue > boundsRange.Max && toValue > boundsRange.Max)
            {
                return null;
            }

            if (fromValue < boundsRange.Min && toValue < boundsRange.Min)
            {
                return null;
            }

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

            return null;
        }

        private Vector2i GetGroup(Vector2i cell)
        {
            var x = cell.X / _cellInGroupCount.X - ((cell.X < 0 && cell.X % _cellInGroupCount.X != 0) ? 1 : 0);
            var y = cell.Y / _cellInGroupCount.Y - ((cell.Y < 0 && cell.Y % _cellInGroupCount.Y != 0) ? 1 : 0);
            return new Vector2i(x, y);
        }

        protected Vector3f? RaycastCellUsingImage(Line2f line, Vector2i cell)
        {
            var imageValueFrom = GetLinearInterpolatedValue(_image, line.From);
            var imageValueTo = GetLinearInterpolatedValue(_image, line.To);

            var fromRayValue = GetRayValue(line.From);
            var toRayValue = GetRayValue(line.To);

            var x = (fromRayValue - imageValueFrom) / (imageValueTo - imageValueFrom - toRayValue + fromRayValue);
            if (x < 0 || x > 1)
                return null;

            var xy = line.From + (line.To - line.From) * x;
            return xy.ToVector3f(GetLinearInterpolatedValue(_image, xy));
        }

        private float GetRayValue(Vector2f point)
        {
            var p = (point - _startXy).Magnitude() / _directionXyMag;
            return _cameraRay.Origin.Z + _cameraRay.Direction.Z * p;
        }

        private float GetLinearInterpolatedValue(IImage2f image, Vector2f pos)
        {
            var pos_x0y0 = pos.FloorToVector2i();
            var fraction = pos - pos_x0y0;

            var pos_x0y1 = pos_x0y0 + new Vector2i(0, 1);
            var pos_x1y0 = pos_x0y0 + new Vector2i(1, 0);
            var pos_x1y1 = pos_x0y0 + new Vector2i(1, 1);

            var x0y0 = image.Sample(pos_x0y0);
            var x0y1 = image.Sample(pos_x0y1);
            var x1y0 = image.Sample(pos_x1y0);
            var x1y1 = image.Sample(pos_x1y1);

            return (1f - fraction.X) * (1f - fraction.Y) * x0y0 + fraction.X * (1f - fraction.Y) * x1y0 + (1f - fraction.X) * fraction.Y * x0y1 + fraction.X * fraction.Y * x1y1;
        }
    }
}