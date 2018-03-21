using System;
using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Utils;
using Votyra.Core.Images.Constraints;
using Votyra.Core.ImageSamplers;
using Zenject;

namespace Votyra.Core.Images
{
    public class EditableMatrixImage3f : IImage3fProvider, IEditableImage3f
    {
        private readonly Matrix3<float> _editableMatrix;

        private Rect3i? _invalidatedArea;

        private readonly List<LockableMatrix3<float>> _readonlyMatrices = new List<LockableMatrix3<float>>();

        private MatrixImage3f _image = null;

        private IImageConstraint3i _constraint;

        public EditableMatrixImage3f([InjectOptional] IImageConstraint3i constraint, IImageConfig imageConfig)
        {
            _constraint = constraint;
            _editableMatrix = new Matrix3<float>(imageConfig.ImageSize);
        }

        public IImage3f CreateImage()
        {
            if (_invalidatedArea == Rect3i.zero)
            {
                _image?.Dispose();
                _image = new MatrixImage3f(_image.Image, Rect3i.zero);
            }
            else if (_invalidatedArea.HasValue || _image == null)
            {
                _invalidatedArea = _invalidatedArea ?? _editableMatrix.size.ToRect3i();
                // Debug.LogFormat("Update readonlyCount:{0}", _readonlyMatrices.Count);

                var readonlyMatrix = _readonlyMatrices.FirstOrDefault(o => !o.IsLocked);
                if (readonlyMatrix == null)
                {
                    readonlyMatrix = new LockableMatrix3<float>(_editableMatrix.size);
                    _readonlyMatrices.Add(readonlyMatrix);
                }

                //sync
                for (int x = 0; x < _editableMatrix.size.x; x++)
                {
                    for (int y = 0; y < _editableMatrix.size.y; y++)
                    {
                        for (int z = 0; z < _editableMatrix.size.z; z++)
                        {
                            readonlyMatrix[x, y, z] = _editableMatrix[x, y, z];
                        }
                    }
                }

                // Debug.LogError($"_readonlyMatrices: {_readonlyMatrices.Count}");

                _image?.Dispose();
                _image = new MatrixImage3f(readonlyMatrix, _invalidatedArea.Value);
                _invalidatedArea = Rect3i.zero;
            }
            return _image;
        }

        public IEditableImageAccessor3f RequestAccess(Rect3i areaRequest)
        {
            return new MatrixImageAccessor(this, areaRequest);
        }

        private void FixImage(Rect3i invalidatedImageArea, Direction direction)
        {
            _invalidatedArea = _invalidatedArea?.CombineWith(invalidatedImageArea) ?? invalidatedImageArea;

            if (_constraint == null)
            {
                return;
            }

            var newInvalidatedImageArea = _constraint.FixImage(_editableMatrix, invalidatedImageArea, direction);

            _invalidatedArea = _invalidatedArea?.CombineWith(newInvalidatedImageArea) ?? newInvalidatedImageArea;
        }

        private class EditableImageWrapper : IImage3f
        {
            private readonly Matrix3<float> _editableMatrix;

            public EditableImageWrapper(Matrix3<float> editableMatrix)
            {
                _editableMatrix = editableMatrix;
            }

            public Rect3i InvalidatedArea => Rect3i.zero;

            public float Sample(Vector3i point)
            {
                if (point.IsAsIndexContained(_editableMatrix.size))
                {
                    return _editableMatrix[point];
                }
                else
                {
                    return 0;
                }
            }
        }

        private class MatrixImageAccessor : IEditableImageAccessor3f
        {
            private readonly float[,,] _editableMatrix;
            private float _changeCounter = 0;
            public Rect3i Area { get; }

            public float this[Vector3i pos]
            {
                get { return _editableMatrix[pos.x, pos.y, pos.z]; }
                set
                {
                    _changeCounter += value - _editableMatrix[pos.x, pos.y, pos.z];
                    _editableMatrix[pos.x, pos.y, pos.z] = value;
                }
            }

            private readonly EditableMatrixImage3f _editableImage;

            public MatrixImageAccessor(EditableMatrixImage3f editableImage, Rect3i area)
            {
                _editableMatrix = editableImage._editableMatrix.NativeMatrix;
                _editableImage = editableImage;
                Area = area.IntersectWith(editableImage._editableMatrix.size.ToRect3i());
            }

            public void Dispose()
            {
                this._editableImage.FixImage(Area, _changeCounter > 0 ? Direction.Up : (_changeCounter < 0 ? Direction.Down : Direction.Unknown));
            }
        }
    }
}