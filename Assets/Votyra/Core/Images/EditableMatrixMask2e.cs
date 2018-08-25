using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Images.Constraints;
using Votyra.Core.Models;
using Zenject;

namespace Votyra.Core.Images
{
    public class EditableMatrixMask2e : IMask2eProvider, IEditableMask2e
    {
        private readonly Matrix2<MaskValues> _editableMatrix;

        private readonly List<LockableMatrix2<MaskValues>> _readonlyMatrices = new List<LockableMatrix2<MaskValues>>();
        private MatrixMask2e _image = null;
        private Range2i? _invalidatedArea;

        public EditableMatrixMask2e(IImageConfig imageConfig)
        {
            _editableMatrix = new Matrix2<MaskValues>(imageConfig.ImageSize.XY);
        }

        public IMask2e CreateMask()
        {
            if (_invalidatedArea == Range2i.Zero)
            {
                _image?.Dispose();
                _image = new MatrixMask2e(_image.Image, Range2i.Zero);
            }
            else if (_invalidatedArea.HasValue || _image == null)
            {
                _invalidatedArea = _invalidatedArea ?? _editableMatrix.Size.ToRange2i();
                // Debug.LogFormat("Update readonlyCount:{0}", _readonlyMatrices.Count);

                var readonlyMatrix = _readonlyMatrices.FirstOrDefault(o => !o.IsLocked);
                if (readonlyMatrix == null)
                {
                    readonlyMatrix = new LockableMatrix2<MaskValues>(_editableMatrix.Size);
                    _readonlyMatrices.Add(readonlyMatrix);
                }

                //sync
                _editableMatrix
                    .ForeachPointExlusive(i =>
                    {
                        readonlyMatrix[i] = _editableMatrix[i];
                    });

                // Debug.LogError($"_readonlyMatrices: {_readonlyMatrices.Count}");

                _image?.Dispose();
                _image = new MatrixMask2e(readonlyMatrix, _invalidatedArea.Value);
                _invalidatedArea = Range2i.Zero;
            }
            return _image;
        }

        private void UpdateImage(Range2i invalidatedImageArea)
        {
            _invalidatedArea = _invalidatedArea?.CombineWith(invalidatedImageArea) ?? invalidatedImageArea;
        }

        public IEditableMaskAccessor2e RequestAccess(Range2i areaRequest)
        {
            return new MatrixImageAccessor(this, areaRequest);
        }

        private class MatrixImageAccessor : IEditableMaskAccessor2e
        {
            private readonly EditableMatrixMask2e _editableImage;
            private readonly MaskValues[,] _editableMatrix;

            public MatrixImageAccessor(EditableMatrixMask2e editableImage, Range2i area)
            {
                _editableMatrix = editableImage._editableMatrix.NativeMatrix;
                _editableImage = editableImage;
                Area = area.IntersectWith(editableImage._editableMatrix.Size.ToRange2i());
            }

            public Range2i Area { get; }

            public MaskValues this[Vector2i pos]
            {
                get
                {
                    return _editableMatrix[pos.X, pos.Y];
                }
                set
                {
                    _editableMatrix[pos.X, pos.Y] = value;
                }
            }

            public void Dispose()
            {
                this._editableImage.UpdateImage(Area);
            }
        }
    }
}