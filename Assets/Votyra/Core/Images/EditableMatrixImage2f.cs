using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Images.Constraints;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class EditableMatrixImage2f : IImage2fProvider, IEditableImage2f
    {
        private readonly IImageConstraint2i[] _constraints;
        private readonly float[,] _editableMatrix;

        private readonly IImage2fPostProcessor _image2fPostProcessor;

        private readonly List<MatrixImage2f> _readonlyMatrices = new List<MatrixImage2f>();
        private Area1f _editableRangeZ;
        private Range2i? _invalidatedArea;
        private MatrixImage2f _preparedImage;

        public EditableMatrixImage2f(IImageConfig imageConfig, List<IImageConstraint2i> constraints)
        {
            _constraints = constraints.SelectMany(o => o.Priorities.Select(p => new {p, o}))
                .OrderBy(o => o.p)
                .Select(o => o.o)
                .ToArray();
            foreach (var constraint in _constraints)
            {
                constraint.Initialize(this);
            }

            _editableMatrix = new float[imageConfig.ImageSize.X, imageConfig.ImageSize.Y];
            _editableRangeZ = Area1f.FromMinAndMax(_editableMatrix[0, 0], _editableMatrix[0, 0]);
        }

        private MatrixImage2f PreparedImage
        {
            get => _preparedImage;
            set
            {
                _preparedImage?.FinishUsing();
                _preparedImage = value;
                _preparedImage?.StartUsing();
            }
        }

        public IEditableImageAccessor2f RequestAccess(Range2i areaRequest) => new MatrixImageAccessor(this, areaRequest);

        public IImage2f CreateImage()
        {
            if (_invalidatedArea == Range2i.Zero && PreparedImage.InvalidatedArea == Range2i.Zero)
            {
            }
            else if (_invalidatedArea.HasValue || PreparedImage == null)
            {
                _invalidatedArea = _invalidatedArea ?? _editableMatrix.Range();

                PreparedImage = GetNotUsedImage();
                PreparedImage.UpdateImage(_editableMatrix, _editableRangeZ);
                PreparedImage.UpdateInvalidatedArea(_invalidatedArea.Value);
                _invalidatedArea = Range2i.Zero;
            }

            return PreparedImage;
        }

        private MatrixImage2f GetNotUsedImage()
        {
            MatrixImage2f image = null;
            for (var i = 0; i < _readonlyMatrices.Count; i++)
            {
                var o = _readonlyMatrices[i];
                if (!o.IsBeingUsed)
                {
                    image = o;
                    break;
                }
            }

            if (image == null)
            {
                image = new MatrixImage2f(_editableMatrix.Size());
                _readonlyMatrices.Add(image);
            }

            return image;
        }

        private void FixImage(Range2i invalidatedImageArea, Direction direction)
        {
            if (_invalidatedArea == null)
            {
                invalidatedImageArea = _invalidatedArea.Value.CombineWith(invalidatedImageArea);
            }

            _invalidatedArea = invalidatedImageArea;

            

            for (int i = 0; i < _constraints.Length; i++)
            {
                var newInvalidatedImageArea = _constraints[i]
                    .FixImage(this, _editableMatrix, invalidatedImageArea, direction);
                invalidatedImageArea = invalidatedImageArea.CombineWith(newInvalidatedImageArea);
                _invalidatedArea = invalidatedImageArea;
            }
        }

        private class MatrixImageAccessor : IEditableImageAccessor2f
        {
            private readonly EditableMatrixImage2f _editableImage;
            private readonly float[,] _editableMatrix;
            private float _changeCounter;
            private Area1f _editableRangeZ;

            public MatrixImageAccessor(EditableMatrixImage2f editableImage, Range2i area)
            {
                _editableMatrix = editableImage._editableMatrix;
                _editableImage = editableImage;
                Area = area.IntersectWith(editableImage._editableMatrix.Range());
            }

            public Range2i Area { get; }

            public float this[Vector2i pos]
            {
                get => _editableMatrix[pos.X, pos.Y];
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
                _editableImage._editableRangeZ = _editableRangeZ;
                var direction = _changeCounter > 0 ? Direction.Up : _changeCounter < 0 ? Direction.Down : Direction.Unknown;
                _editableImage.FixImage(Area, direction);
            }
        }
    }
}