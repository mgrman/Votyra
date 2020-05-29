using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class ImageConfig : IImageConfig
    {
        public ImageConfig([ConfigInject("imageSize"),]
            Vector3i imageSize)
        {
            this.ImageSize = imageSize;
        }

        public Vector3i ImageSize { get; }

        public static bool operator ==(ImageConfig a, ImageConfig b) => a?.Equals(b) ?? b?.Equals(a) ?? true;

        public static bool operator !=(ImageConfig a, ImageConfig b) => !(a == b);

        public override bool Equals(object obj)
        {
            if ((obj == null) || (this.GetType() != obj.GetType()))
            {
                return false;
            }

            var that = obj as ImageConfig;

            return this.ImageSize == that.ImageSize;
        }

        public override int GetHashCode() => this.ImageSize.GetHashCode();
    }
}
