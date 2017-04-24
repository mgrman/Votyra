using TycoonTerrain.Images;
using UnityEngine;

namespace TycoonTerrain.Unity.Images
{
    internal class TextureImageBehaviour : MonoBehaviour, IImage2iProvider
    {
        private Bounds _oldBounds;
        public Bounds Bounds;
        
        private Texture2D _oldTexture = null;
        public Texture2D Texture;

        private IImage2i _image = null;

        public IImage2i Image
        {
            get
            {
                //if (_image == null)
                SetImage();
                return _image;
            }
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