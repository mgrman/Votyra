
using UnityEngine;
using Votyra.Core.Images;

namespace Votyra.Core.Images
{
    internal class CombineImageBehaviour : MonoBehaviour, IImage2fProvider
    {
        private MonoBehaviour _oldImageA;
        public MonoBehaviour ImageA = null;

        private MonoBehaviour _oldImageB = null;
        public MonoBehaviour ImageB = null;

        private CombineImage2f.Operations _oldOperation = CombineImage2f.Operations.Add;
        public CombineImage2f.Operations Operation = CombineImage2f.Operations.Add;


        private CombineImage2f _image = null;
        public IImage2f CreateImage()
        {

            SetImage();
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
            if (_oldImageA != ImageA)
            {
                _oldImageA = ImageA;
                _fieldsChanged = _fieldsChanged || true;
            }
            if (_oldImageB != ImageB)
            {
                _oldImageB = ImageB;
                _fieldsChanged = _fieldsChanged || true;
            }
            if (_oldOperation != Operation)
            {
                _oldOperation = Operation;
                _fieldsChanged = _fieldsChanged || true;
            }
            if (_fieldsChanged)
            {
                SetImage();
            }
        }

        private void SetImage()
        {
            _image = new CombineImage2f((ImageA as IImage2fProvider).CreateImage(), (ImageB as IImage2fProvider).CreateImage(), Operation);
        }
    }
}
