using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Images.Constraints;
using Votyra.Core.Models;
using Zenject;

namespace Votyra.Core.Images
{
    public class EditableMatrixImage2i : IImage2fProvider, IEditableImage2i
    {
        private readonly Matrix2<float> _editableMatrix;
        private Area1f _editableRangeZ;

        private readonly List<LockableMatrix2<float>> _readonlyMatrices = new List<LockableMatrix2<float>>();
        private Range2i? _invalidatedArea;
        private MatrixImage2i _image = null;

        private IImageConstraint2i _constraint;

        public EditableMatrixImage2i([InjectOptional] IImageConstraint2i constraint, IImageConfig imageConfig)
        {
            _constraint = constraint;
            _editableMatrix = new Matrix2<float>(imageConfig.ImageSize.XY);
            _editableRangeZ = new Area1f(_editableMatrix[0,0], _editableMatrix[0, 0]);
        }

        public IImage2f CreateImage()
        {
            if (_invalidatedArea == Range2i.Zero)
            {
                _image?.Dispose();
                _image = new MatrixImage2i(_image.Image, Range2i.Zero,_image.RangeZ);
            }
            else if (_invalidatedArea.HasValue || _image == null)
            {
                _invalidatedArea = _invalidatedArea ?? _editableMatrix.Size.ToRange2i();
                // Debug.LogFormat("Update readonlyCount:{0}", _readonlyMatrices.Count);

                var readonlyMatrix = _readonlyMatrices.FirstOrDefault(o => !o.IsLocked);
                if (readonlyMatrix == null)
                {
                    readonlyMatrix = new LockableMatrix2<float>(_editableMatrix.Size);
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
                _image = new MatrixImage2i(readonlyMatrix, _invalidatedArea.Value,_editableRangeZ);
                _invalidatedArea = Range2i.Zero;
            }
            return _image;
        }

        public IEditableImageAccessor2f RequestAccess(Range2i areaRequest)
        {
            return new MatrixImageAccessor(this, areaRequest);
        }

        private void FixImage(Range2i invalidatedImageArea, Direction direction)
        {
            _invalidatedArea = _invalidatedArea?.CombineWith(invalidatedImageArea) ?? invalidatedImageArea;

            if (_constraint == null)
            {
                return;
            }

            var newInvalidatedImageArea = _constraint.FixImage(_editableMatrix, invalidatedImageArea, direction);
            _invalidatedArea = _invalidatedArea?.CombineWith(newInvalidatedImageArea) ?? newInvalidatedImageArea;
        }


        // private static Range1hi CalculateRangeZ(LockableMatrix2<Height1i> values)
        // {
        //     Height1i min = Height1i.MaxValue;
        //     Height1i max = Height1i.MinValue;
        //     values.ForeachPointExlusive(i =>
        //     {
        //         Height1i val = values[i];
        //
        //         min = Height1i.Min(min, val);
        //         max = Height1i.Max(max, val);
        //     });
        //     return Height1i.Range(min, max);
        // }

        private class MatrixImageAccessor : IEditableImageAccessor2f
        {
            private readonly float[,] _editableMatrix;
            private readonly EditableMatrixImage2i _editableImage;
            private Area1f _editableRangeZ;
            private float _changeCounter = 0f;

            public MatrixImageAccessor(EditableMatrixImage2i editableImage, Range2i area)
            {
                _editableMatrix = editableImage._editableMatrix.NativeMatrix;
                _editableImage = editableImage;
                Area = area.IntersectWith(editableImage._editableMatrix.Size.ToRange2i());
            }

            public Range2i Area { get; }

            public float this[Vector2i pos]
            {
                get
                {
                    return _editableMatrix[pos.X, pos.Y];
                }
                set
                {
                    var existingValue = _editableMatrix[pos.X, pos.Y];
                    _changeCounter += value - existingValue;
                    _editableMatrix[pos.X, pos.Y] = value;
                    _editableRangeZ = _editableRangeZ.UnionWith(value);
                }
            }

            public void Dispose()
            {
                this._editableImage._editableRangeZ = _editableRangeZ;
                var direction = _changeCounter > 0 ? Direction.Up : (_changeCounter < 0 ? Direction.Down : Direction.Unknown);
                this._editableImage.FixImage(Area, direction);
                
            }
        }
    }
}