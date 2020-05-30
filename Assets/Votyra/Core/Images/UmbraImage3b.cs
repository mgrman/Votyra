using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class UmbraImage3B : IImage3B
    {
        public UmbraImage3B(IImage2F imageA)
        {
            this.Image = imageA;
        }

        public IImage2F Image { get; }

        public bool Sample(Vector3i point) => (this.Image.Sample(new Vector2i(point.X, point.Y)) - point.Z) > 0;

        public bool AnyData(Range3i range) => true;
    }
}
