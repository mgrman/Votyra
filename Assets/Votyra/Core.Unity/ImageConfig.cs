using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class ImageConfig : IImageConfig
    {
        public ImageConfig([ConfigInject("imageSize")] Vector2i imageSize)
        {
            ImageSize = imageSize;
        }

        public Vector2i ImageSize { get; }

        public static bool operator ==(ImageConfig a, ImageConfig b) => a?.Equals(b) ?? b?.Equals(a) ?? true;

        public static bool operator !=(ImageConfig a, ImageConfig b) => !(a == b);

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var that = obj as ImageConfig;

            return ImageSize == that.ImageSize;
        }

        public override int GetHashCode() => ImageSize.GetHashCode();
    }
}