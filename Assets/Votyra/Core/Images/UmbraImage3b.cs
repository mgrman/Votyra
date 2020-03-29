using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class UmbraImage3b : IImage3b
    {
        public UmbraImage3b(IImage2f imageA)
        {
            Image = imageA;
        }

        public IImage2f Image { get; }

        public bool Sample(Vector3i point) => Image.Sample(new Vector2i(point.X, point.Y)) - point.Z > 0;

        public bool AnyData(Range3i range) => true;
    }
}
