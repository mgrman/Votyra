using System.Collections.Generic;
using System.Linq;
using Votyra.Common.Models;
using Votyra.Images;
using Votyra.Models;
using Votyra.Common.Utils;
using UnityEngine;

namespace Votyra.Unity.Images
{
    internal class MaxtrixImageBehaviour : MonoBehaviour, IImage2iProvider
    {
        public Texture2D InitialValueTexture;
        public float InitialValueScale;

        private Matrix<int> _editableMatrix;

        private Rect2i? _invalidatedArea;

        private readonly List<LockableMatrix<int>> _readonlyMatrices = new List<LockableMatrix<int>>();

        private MatrixImage _image = null;

        public IImage2i CreateImage()
        {
            if (_invalidatedArea.HasValue)
            {
                // Debug.LogFormat("Update readonlyCount:{0}", _readonlyMatrices.Count);

                var readonlyMatrix = _readonlyMatrices.FirstOrDefault(o => !o.IsLocked);
                if (readonlyMatrix == null)
                {
                    readonlyMatrix = new LockableMatrix<int>(_editableMatrix.size);
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

                // Debug.LogError($"_readonlyMatrices: {_readonlyMatrices.Count}");

                var oldImage = _image;
                oldImage?.Dispose();

                _image = new MatrixImage(readonlyMatrix, _invalidatedArea.Value);
                _invalidatedArea = null;
            }
            return _image;
        }

        private void Start()
        {
            if (InitialValueTexture == null)
            {
                var size = new Vector2i(10, 10);
                _editableMatrix = new Matrix<int>(size);
                _invalidatedArea = new Rect2i(0, 0, 10, 10);
            }
            else
            {
                var texture = InitialValueTexture;
                var size = new Vector2i(texture.width, texture.height);
                _editableMatrix = new Matrix<int>(size);
                _invalidatedArea = new Rect2i(0, 0, size.x, size.y);

                for (int x = 0; x < texture.width; x++)
                {
                    for (int y = 0; y < texture.height; y++)
                    {
                        _editableMatrix[x, y] = (int)(texture.GetPixel(x, y).grayscale * InitialValueScale);
                        if (_editableMatrix[x, y] != 0)
                        {

                        }
                    }
                }
            }
        }

        private void Update()
        {
        }

        public void SetByOffsetValue(Vector2i pos, int value)
        {
            if (!pos.IsAsIndexContained(_editableMatrix.size))
            {
                Debug.LogWarningFormat("Position {0} is outside of bounds!", pos);
                return;
            }

            SetValue(pos, _editableMatrix[pos] + value);
        }

        public int GetValue(Vector2i pos)
        {
            if (!pos.IsAsIndexContained(_editableMatrix.size))
            {
                Debug.LogWarningFormat("Position {0} is outside of bounds!", pos);
                return 0;
            }

            // Debug.LogFormat("Setting value at {0} to {1} readonlyCount:{2}", pos, value, _readonlyMatrices.Count);

            return _editableMatrix[pos];
        }

        public void SetValue(Vector2i pos, int value)
        {
            if (!pos.IsAsIndexContained(_editableMatrix.size))
            {
                Debug.LogWarningFormat("Position {0} is outside of bounds!", pos);
                return;
            }

            // Debug.LogFormat("Setting value at {0} to {1} readonlyCount:{2}", pos, value, _readonlyMatrices.Count);

            _editableMatrix[pos] = value;

            var changedArea = new Rect2i(pos.x, pos.y, 1, 1);
            _invalidatedArea = _invalidatedArea?.CombineWith(changedArea) ?? changedArea;
        }
    }
}