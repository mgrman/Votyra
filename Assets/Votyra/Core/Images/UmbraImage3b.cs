using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class UmbraImage3b : IImage3b
    {
        public UmbraImage3b(IImage2i imageA)
        {
            Image = imageA;
        }

        public IImage2i Image { get; private set; }

        public bool Sample(Vector3i point)
        {
            return Image.Sample(new Vector2i(point.X, point.Y)) - point.Z.CreateHeight() > Height.Difference.Zero;
        }

        public bool AnyData(Range3i range)
        {
            return true;
        }
    }
}