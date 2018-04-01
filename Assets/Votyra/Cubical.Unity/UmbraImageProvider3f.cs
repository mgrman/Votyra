using System.Collections;
using System.Collections.Generic;
using Votyra.Core.Images;
using UnityEngine;
using Votyra.Core.Models;
using Votyra.Core;

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

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        matrix[x, y] = (initialTexture.GetPixel(x, y).grayscale * scale.Z);
                    }
                }
                _image = new UmbraImage3b(new MatrixImage2f(matrix, Rect2i.Zero));
            }
            var initialMatrix = imageConfig.InitialData as IMatrix2<float>;
            if (initialMatrix != null)
            {
                var scale = imageConfig.InitialDataScale;

                int width = initialMatrix.size.X;
                int height = initialMatrix.size.Y;

                var size = new Vector2i(width, height);
                var matrix = new LockableMatrix2<float>(size);

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        matrix[x, y] = initialMatrix[x, y] * scale.Z;
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
