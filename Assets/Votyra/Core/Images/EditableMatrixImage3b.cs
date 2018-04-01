using System;
using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Utils;
using Votyra.Core.Images.Constraints;
using Votyra.Core.ImageSamplers;
using Zenject;
using Votyra.Core.Logging;

namespace Votyra.Core.Images
{
    public class EditableMatrixImage3b : IImage3bProvider, IEditableImage3b
    {
        private readonly Matrix3<bool> _editableMatrix;

        private Rect3i? _invalidatedArea;

        private readonly List<LockableMatrix3<bool>> _readonlyMatrices = new List<LockableMatrix3<bool>>();

        private MatrixImage3b _image = null;

        private IImageConstraint3b _constraint;

        public EditableMatrixImage3b([InjectOptional] IImageConstraint3b constraint, IImageConfig imageConfig)
        {
            _constraint = constraint;
            _editableMatrix = new Matrix3<bool>(imageConfig.ImageSize);
        }

        public IImage3b CreateImage()
        {
            if (_invalidatedArea == Rect3i.zero)
            {
                _image?.Dispose();
                _image = new MatrixImage3b(_image.Image, Rect3i.zero);
            }
            else if (_invalidatedArea.HasValue || _image == null)
            {
                _invalidatedArea = _invalidatedArea ?? _editableMatrix.size.ToRect3i();
                // Debug.LogFormat("Update readonlyCount:{0}", _readonlyMatrices.Count);

                var readonlyMatrix = _readonlyMatrices.FirstOrDefault(o => !o.IsLocked);
                if (readonlyMatrix == null)
                {
                    readonlyMatrix = new LockableMatrix3<bool>(_editableMatrix.size);
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
                _image = new MatrixImage3b(readonlyMatrix, _invalidatedArea.Value);
                _invalidatedArea = Rect3i.zero;
            }
            return _image;
        }

        public IEditableImageAccessor3b RequestAccess(Rect3i areaRequest)
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
            LoggerFactoryExtensions.factory.Create(this).LogMessage("newInvalidatedImageArea:" + newInvalidatedImageArea);

            _invalidatedArea = _invalidatedArea?.CombineWith(newInvalidatedImageArea) ?? newInvalidatedImageArea;
            LoggerFactoryExtensions.factory.Create(this).LogMessage("_invalidatedArea:" + _invalidatedArea);
        }

        private class EditableImageWrapper : IImage3b
        {
            private readonly Matrix3<bool> _editableMatrix;

            public EditableImageWrapper(Matrix3<bool> editableMatrix)
            {
                _editableMatrix = editableMatrix;
            }

            public Rect3i InvalidatedArea => Rect3i.zero;

            public bool Sample(Vector3i point)
            {
                if (point.IsAsIndexContained(_editableMatrix.size))
                {
                    return _editableMatrix[point];
                }
                else
                {
                    return false;
                }
            }
        }

        private class MatrixImageAccessor : IEditableImageAccessor3b
        {
            private readonly bool[,,] _editableMatrix;
            private int _changeCounter = 0;
            private bool _changed = true;
            public Rect3i Area { get; }

            public bool this[Vector3i pos]
            {
                get { return _editableMatrix[pos.x, pos.y, pos.z]; }
                set
                {
                    if (value && !_editableMatrix[pos.x, pos.y, pos.z])
                    {
                        _changeCounter += 1;
                        _changed = true;
                    }
                    if (!value && _editableMatrix[pos.x, pos.y, pos.z])
                    {
                        _changeCounter -= 1;
                        _changed = true;
                    }
                    _editableMatrix[pos.x, pos.y, pos.z] = value;
                }
            }

            private readonly EditableMatrixImage3b _editableImage;

            public MatrixImageAccessor(EditableMatrixImage3b editableImage, Rect3i area)
            {
                _editableMatrix = editableImage._editableMatrix.NativeMatrix;
                _editableImage = editableImage;
                Area = area.IntersectWith(editableImage._editableMatrix.size.ToRect3i());
            }

            public void Dispose()
            {
                if (!_changed)
                    return;

                this._editableImage.FixImage(Area, _changeCounter > 0 ? Direction.Up : (_changeCounter < 0 ? Direction.Down : Direction.Unknown));
            }
        }
    }
}