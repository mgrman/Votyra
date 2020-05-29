using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.TerrainGenerators.TerrainMeshers;

namespace Votyra.Core.Raycasting
{
    public sealed class Image2fRaycaster : BaseCellRaycaster
    {
        private readonly IImage2fProvider _image2FProvider;
        private Ray3f _cameraRay;
        private float _directionXyMag;
        private IImage2f _image;
        private Vector2f _startXy;

        public Image2fRaycaster(IImage2fProvider image2FProvider, ITerrainConfig terrainConfig, ITerrainVertexPostProcessor terrainVertexPostProcessor = null, IRaycasterAggregator raycasterAggregator = null)
            : base(terrainVertexPostProcessor, raycasterAggregator)
        {
            this._image2FProvider = image2FProvider;
        }

        public override Vector3f Raycast(Ray3f cameraRay)
        {
            this._image = this._image2FProvider.CreateImage();
            (this._image as IInitializableImage)?.StartUsing();

            this._cameraRay = cameraRay;

            this._startXy = cameraRay.XY()
                .Origin;
            this._directionXyMag = cameraRay.Direction.XY()
                .Magnitude();

            var result = base.Raycast(cameraRay);

            (this._image as IInitializableImage)?.FinishUsing();
            this._image = null;

            return result;
        }

        protected override Vector3f RaycastCell(Line2f line)
        {
            var imageValueFrom = this.GetLinearInterpolatedValue(this._image, line.From);
            var imageValueTo = this.GetLinearInterpolatedValue(this._image, line.To);

            var fromRayValue = this.GetRayValue(line.From);
            var toRayValue = this.GetRayValue(line.To);

            var x = (fromRayValue - imageValueFrom) / ((imageValueTo - imageValueFrom - toRayValue) + fromRayValue);
            if ((x < 0) || (x > 1))
            {
                return Vector3f.NaN;
            }

            var xy = line.From + ((line.To - line.From) * x);
            return xy.ToVector3f(this.GetLinearInterpolatedValue(this._image, xy));
        }

        protected override float RaycastCell(Vector2f point) => this.GetLinearInterpolatedValue(this._image, point);

        private float GetRayValue(Vector2f point)
        {
            var p = (point - this._startXy).Magnitude() / this._directionXyMag;
            return this._cameraRay.Origin.Z + (this._cameraRay.Direction.Z * p);
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

            return ((1f - fraction.X) * (1f - fraction.Y) * x0y0) + (fraction.X * (1f - fraction.Y) * x1y0) + ((1f - fraction.X) * fraction.Y * x0y1) + (fraction.X * fraction.Y * x1y1);
        }
    }
}
