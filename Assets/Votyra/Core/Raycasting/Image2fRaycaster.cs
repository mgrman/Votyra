using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.TerrainGenerators.TerrainMeshers;

namespace Votyra.Core.Raycasting
{
    public sealed class Image2fRaycaster : BaseCellRaycaster
    {
        private readonly IImage2fProvider image2fProvider;
        private Ray3f cameraRay;
        private float directionXyMag;
        private IImage2f image;
        private Vector2f startXy;

        public Image2fRaycaster(IImage2fProvider image2fProvider, ITerrainConfig terrainConfig, ITerrainVertexPostProcessor terrainVertexPostProcessor = null, IRaycasterAggregator raycasterAggregator = null)
            : base(terrainVertexPostProcessor, raycasterAggregator)
        {
            this.image2fProvider = image2fProvider;
        }

        public override Vector3f Raycast(Ray3f cameraRay)
        {
            this.image = this.image2fProvider.CreateImage();
            (this.image as IInitializableImage)?.StartUsing();

            this.cameraRay = cameraRay;

            this.startXy = cameraRay.XY()
                .Origin;
            this.directionXyMag = cameraRay.Direction.XY()
                .Magnitude();

            var result = base.Raycast(cameraRay);

            (this.image as IInitializableImage)?.FinishUsing();
            this.image = null;

            return result;
        }

        protected override Vector3f RaycastCell(Line2f line)
        {
            var imageValueFrom = this.GetLinearInterpolatedValue(this.image, line.From);
            var imageValueTo = this.GetLinearInterpolatedValue(this.image, line.To);

            var fromRayValue = this.GetRayValue(line.From);
            var toRayValue = this.GetRayValue(line.To);

            var x = (fromRayValue - imageValueFrom) / ((imageValueTo - imageValueFrom - toRayValue) + fromRayValue);
            if ((x < 0) || (x > 1))
            {
                return Vector3f.NaN;
            }

            var xy = line.From + ((line.To - line.From) * x);
            return xy.ToVector3f(this.GetLinearInterpolatedValue(this.image, xy));
        }

        protected override float RaycastCell(Vector2f point) => this.GetLinearInterpolatedValue(this.image, point);

        private float GetRayValue(Vector2f point)
        {
            var p = (point - this.startXy).Magnitude() / this.directionXyMag;
            return this.cameraRay.Origin.Z + (this.cameraRay.Direction.Z * p);
        }

        private float GetLinearInterpolatedValue(IImage2f image, Vector2f pos)
        {
            var posX0Y0 = pos.FloorToVector2i();
            var fraction = pos - posX0Y0;

            var posX0Y1 = posX0Y0 + new Vector2i(0, 1);
            var posX1Y0 = posX0Y0 + new Vector2i(1, 0);
            var posX1Y1 = posX0Y0 + new Vector2i(1, 1);

            var x0Y0 = image.Sample(posX0Y0);
            var x0Y1 = image.Sample(posX0Y1);
            var x1Y0 = image.Sample(posX1Y0);
            var x1Y1 = image.Sample(posX1Y1);

            return ((1f - fraction.X) * (1f - fraction.Y) * x0Y0) + (fraction.X * (1f - fraction.Y) * x1Y0) + ((1f - fraction.X) * fraction.Y * x0Y1) + (fraction.X * fraction.Y * x1Y1);
        }
    }
}
