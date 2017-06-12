using Votyra.Images;
using UnityEngine;

namespace Votyra.Unity.Images
{
    internal class CombineImageBehaviour : MonoBehaviour, IImage2iProvider
    {
        private MonoBehaviour _oldImageA;
        public MonoBehaviour ImageA = null;

        private MonoBehaviour _oldImageB = null;
        public MonoBehaviour ImageB = null;

        private CombineImage2i.Operations _oldOperation = CombineImage2i.Operations.Add;
        public CombineImage2i.Operations Operation = CombineImage2i.Operations.Add;


        private CombineImage2i _image = null;
        public IImage2i CreateImage()
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
            _image = new CombineImage2i((ImageA as IImage2iProvider).CreateImage(), (ImageB as IImage2iProvider).CreateImage(), Operation);
        }
    }
}