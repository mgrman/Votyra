using TycoonTerrain.Images;
using UnityEngine;

namespace TycoonTerrain.Unity.Images
{
    internal class TextureImageBehaviour : MonoBehaviour, IImage2iProvider
    {
        private int _oldMin = 0;
        public int Min = 0;

        private int _oldMax = 1;
        public int Max = 1;

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
            if (_oldMin != Min)
            {
                _oldMin = Min;
                _fieldsChanged = _fieldsChanged || true;
            }
            if (_oldMax != Max)
            {
                _oldMax = Max;
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
            _image = new TextureImage(new Common.Models.Range2i(Min, Max), Texture);
        }
    }
}