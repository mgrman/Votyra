
using UnityEngine;
using Votyra.Core.Images;

namespace Votyra.Core.Images
{
    internal class TextureImageBehaviour : MonoBehaviour, IImage2fProvider
    {
        private Bounds _oldBounds = new Bounds();
        public Bounds Bounds = new Bounds();

        private Texture2D _oldTexture = null;
        public Texture2D Texture = null;

        private TextureImage _image = null;

        public IImage2f CreateImage()
        {

            return _image;

        }

        private bool _fieldsChanged = true;

        private void Start()
        {
            SetImage();
        }

        private void Update()
        {
            _fieldsChanged = false;
            if (_oldBounds != Bounds)
            {
                _oldBounds = Bounds;
                _fieldsChanged = _fieldsChanged || true;
            }
            if (_oldTexture != Texture)
            {
                _oldTexture = Texture;
                _fieldsChanged = _fieldsChanged || true;
            }
            if (_fieldsChanged)
            {
                SetImage();
            }
        }

        private void SetImage()
        {
            _image = new TextureImage(Bounds, Texture);
        }
    }
}
