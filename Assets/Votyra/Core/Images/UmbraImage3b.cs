using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class UmbraImage3b : IImage3b
    {
        public IImage2f Image { get; private set; }

        public UmbraImage3b(IImage2f imageA)
        {
            Image = imageA;
        }

        public bool Sample(Vector3i point)
        {
            return Image.Sample(new Vector2i(point.X, point.Y)) - point.Z > 0;
        }

        public bool AnyData(Range3i range)
        {
            return true;
        }
    }
}