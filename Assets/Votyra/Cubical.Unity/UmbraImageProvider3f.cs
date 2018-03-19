using System.Collections;
using System.Collections.Generic;
using Votyra.Core.Images;
using UnityEngine;
using Votyra.Core.Models;
using Votyra.Core;

namespace Votyra.Cubical.Unity
{
    public class UmbraImageProvider3f : IImage3fProvider
    {
        private IImage3f _image;

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
                        matrix[x, y] = (initialTexture.GetPixel(x, y).grayscale * scale.z);
                    }
                }
                _image = new UmbraImage3f(new MatrixImage2f(matrix, Rect2i.zero));
            }
            var initialMatrix = imageConfig.InitialData as IMatrix2<float>;
            if (initialMatrix != null)
            {
                var scale = imageConfig.InitialDataScale;

                int width = initialMatrix.size.x;
                int height = initialMatrix.size.y;

                var size = new Vector2i(width, height);
                var matrix = new LockableMatrix2<float>(size);

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        matrix[x, y] = initialMatrix[x, y] * scale.z;
                    }
                }
                _image = new UmbraImage3f(new MatrixImage2f(matrix, Rect2i.zero));
            }
        }

        public IImage3f CreateImage()
        {
            return _image;
        }
    }
}
