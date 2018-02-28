using UnityEngine;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.Images
{
    public class TextureImageBehaviour : MonoBehaviour, IImage2fProvider
    {
        private Vector3 _oldScale = new Vector3();
        public Vector3 Scale = new Vector3();

        private Texture2D _oldTexture = null;
        public Texture2D Texture = null;

        private MatrixImage2f _image = null;

        public IImage2f CreateImage()
        {
            return _image;
        }

        private bool _fieldsChanged = true;

        private void Start()
        {
            UpdateImage();
        }

        private void Update()
        {
            _fieldsChanged = false;
            if (_oldScale != Scale)
            {
                _fieldsChanged = _fieldsChanged || true;
            }
            if (_oldTexture != Texture)
            {
                _fieldsChanged = _fieldsChanged || true;
            }
            if (_fieldsChanged)
            {
                UpdateImage();
            }
        }

        public void UpdateImage()
        {
            _oldScale = Scale;
            _oldTexture = Texture;
            _image = new MatrixImage2f(GetInitialState(Texture, Scale), Rect2i.All);
        }

        private LockableMatrix<float> GetInitialState(Texture2D texture, Vector3 scale)
        {
            int textureWidth = texture.width;
            int textureHeight = texture.height;

            var matrixSize = new Vector2i((textureWidth * scale.x).RoundToInt(), (textureHeight * scale.y).RoundToInt());
            var matrix = new LockableMatrix<float>(matrixSize);

            for (int x = 0; x < matrixSize.x; x++)
            {
                for (int y = 0; y < matrixSize.y; y++)
                {
                    // matrix[x, y] = texture.GetPixelBilinear((x / scale.x) / textureWidth, (y / scale.y) / textureHeight).grayscale * scale.z; 
                    float imageX = (x / scale.x) / textureWidth;
                    float imageY = (y / scale.y) / textureHeight;
                    Color pixelColor = texture.GetPixelBilinear(imageX, imageY);
                    float pixelValue = pixelColor.grayscale * scale.z;
                    if (pixelValue > 0.1f)
                    {
                        Debug.Log($"pixelValue{pixelValue}");
                    }
                    matrix[x, y] = pixelValue;
                }
            }
            return matrix;
        }
    }
}