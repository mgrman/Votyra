using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.TerrainGenerators.TerrainMeshers;

namespace Votyra.Core.Raycasting
{
    public sealed class Image2fRaycaster : BaseRaycaster
    {
        private readonly IImage2fProvider _image2FProvider;
        private readonly IMask2eProvider _mask2EProvider;
        private Ray3f _cameraRay;
        private float _directionXyMag;
        private IImage2f _image;
        private IMask2e _mask;
        private Vector2f _startXy;

        public Image2fRaycaster(IImage2fProvider image2FProvider, IMask2eProvider mask2eProvider, ITerrainVertexPostProcessor terrainVertexPostProcessor = null)
            : base(terrainVertexPostProcessor)
        {
            _image2FProvider = image2FProvider;
            _mask2EProvider = mask2eProvider;
        }

        public override Vector2f? Raycast(Ray3f cameraRay)
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

        protected override Vector2f? RaycastCell(Line2f line, Vector2i cell)
        {
            var imageValueFrom = GetLinearInterpolatedValue(_image, line.From);
            var imageValueTo = GetLinearInterpolatedValue(_image, line.To);

            var fromRayValue = GetRayValue(line.From);
            var toRayValue = GetRayValue(line.To);

            var x = (fromRayValue - imageValueFrom) / (imageValueTo - imageValueFrom - toRayValue + fromRayValue);
            if (x < 0 || x > 1)
                return null;

            return line.From + (line.To - line.From) * x;
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