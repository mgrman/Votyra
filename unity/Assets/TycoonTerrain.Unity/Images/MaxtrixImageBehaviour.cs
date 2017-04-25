using System.Collections.Generic;
using System.Linq;
using TycoonTerrain.Common.Models;
using TycoonTerrain.Images;
using TycoonTerrain.Models;
using UnityEngine;

namespace TycoonTerrain.Unity.Images
{
    internal class MaxtrixImageBehaviour : MonoBehaviour, IImage2iProvider
    {
        public Texture2D InitialValueTexture;
        public float InitialValueScale;

        private LockableMatrix<int> _editableMatrix;

        private readonly List<LockableMatrix<int>> _readonlyMatrices = new List<LockableMatrix<int>>();


        private MatrixImage _image = null;
        public IImage2i CreateImage()
        {
            return _image;
        }
        
        private bool _fieldsChanged = true;

        private void Start()
        {
            if (InitialValueTexture == null)
            {
                var size = new Vector2i(10, 10);
                _editableMatrix = new LockableMatrix<int>(size);
            }
            else
            {
                var texture = InitialValueTexture;
                var size = new Vector2i(texture.width, texture.height);
                _editableMatrix = new LockableMatrix<int>(size);

                for (int x = 0; x < texture.width; x++)
                {
                    for (int y = 0; y < texture.height; y++)
                    {
                        _editableMatrix[x, y] = (int)(texture.GetPixel(x, y).grayscale * InitialValueScale);
                    }
                }
            }
        }

        private void Update()
        {
            
            if (_fieldsChanged )
            {
                Debug.LogFormat("Update readonlyCount:{0} fieldsChanged:{1}", _readonlyMatrices.Count, _fieldsChanged);

                var readonlyMatrix = _readonlyMatrices.FirstOrDefault(o => !o.IsLocked);
                if (readonlyMatrix == null)
                {
                    readonlyMatrix= new LockableMatrix<int>(_editableMatrix.size);
                    _readonlyMatrices.Add(readonlyMatrix);
                }

                //sync
                for (int x = 0; x < _editableMatrix.size.x; x++)
                {
                    for (int y = 0; y < _editableMatrix.size.y; y++)
                    {
                        readonlyMatrix[x, y] = _editableMatrix[x, y];
                    }
                }

                _image = new MatrixImage(readonlyMatrix);
            }
            
            _fieldsChanged = false;
        }

        public void SetValue(Vector2i pos, int value)
        {
            
                Debug.LogWarningFormat("Setting value at {0} to {1} readonlyCount:{2} fieldsChanged:{3}", pos,value, _readonlyMatrices.Count, _fieldsChanged);

            _editableMatrix[pos] = value;

            _fieldsChanged = true;
        }

    }
}