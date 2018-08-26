using Votyra.Core.Models;
using Zenject;

namespace Votyra.Core.Images
{
    public class ImageConfig : IImageConfig
    {
        public ImageConfig([Inject(Id = "imageSize")]Vector3i imageSize)
        {
            ImageSize = imageSize;
        }

        public Vector3i ImageSize { get; }

        public static bool operator ==(ImageConfig a, ImageConfig b)
        {
            return a?.Equals(b) ?? b?.Equals(a) ?? true;
        }

        public static bool operator !=(ImageConfig a, ImageConfig b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            var that = obj as ImageConfig;

            return this.ImageSize == that.ImageSize;
        }

        public override int GetHashCode()
        {
            return this.ImageSize.GetHashCode();
        }
    }
}