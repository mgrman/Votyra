using UnityEngine;
using Votyra.Core;
using Votyra.Core.Images;
using Votyra.Core.Models;

namespace Votyra.Cubical.Unity
{
    public class UmbraImageProvider3f : IImage3bProvider
    {
        private IImage3b _image;

        public UmbraImageProvider3f(IInitialImageConfig imageConfig)
        {
            var initialTexture = imageConfig.InitialData as Texture2D;
            if (initialTexture != null)
            {
                var scale = imageConfig.InitialDataScale;

                int width = initialTexture.width;
                int height = initialTexture.height;

                var size = new Vector2i(width, height);
                var matrix = new LockableMatrix2<float>(size);

                for (int ix = 0; ix < width; ix++)
                {
                    for (int iy = 0; iy < height; iy++)
                    {
                        var i = new Vector2i(ix, iy);
                        matrix[i] = (initialTexture.GetPixel(ix, iy).grayscale * scale.Z);
                    }
                }
                _image = new UmbraImage3b(new MatrixImage2f(matrix, Rect2i.Zero));
            }
            var initialMatrix = imageConfig.InitialData as IMatrix2<float>;
            if (initialMatrix != null)
            {
                var scale = imageConfig.InitialDataScale;

                int width = initialMatrix.Size.X;
                int height = initialMatrix.Size.Y;

                var size = new Vector2i(width, height);
                var matrix = new LockableMatrix2<float>(size);

                for (int ix = 0; ix < width; ix++)
                {
                    for (int iy = 0; iy < height; iy++)
                    {
                        var i = new Vector2i(ix, iy);
                        matrix[i] = initialMatrix[i] * scale.Z;
                    }
                }
                _image = new UmbraImage3b(new MatrixImage2f(matrix, Rect2i.Zero));
            }
        }

        public IImage3b CreateImage()
        {
            return _image;
        }
    }
}