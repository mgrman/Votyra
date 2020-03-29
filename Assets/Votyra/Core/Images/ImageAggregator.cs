using System;
using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Images.Constraints;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class ImageAggregator : ILayerEditableImageProvider, IImageConstraint2i
    {
        private readonly IImageConfig _imageConfig;
        private readonly SortedDictionary<LayerId, EditableMatrixImage2f> _images = new SortedDictionary<LayerId, EditableMatrixImage2f>();
        private readonly SortedDictionary<LayerId, EditableMatrixMask2e> _masks = new SortedDictionary<LayerId, EditableMatrixMask2e>();
        private MatrixImageAccessor _lastLayerImage;

        public ImageAggregator(IImageConfig imageConfig)
        {
            _imageConfig = imageConfig;
        }

        void IImageConstraint2i.Initialize(IEditableImage2f image)
        {
        }

        IEnumerable<int> IImageConstraint2i.Priorities => new[]
        {
            int.MinValue,
            int.MaxValue
        };

        Range2i IImageConstraint2i.FixImage(IEditableImage2f image, float[,] editableMatrix, Range2i invalidatedImageArea, Direction direction)
        {
            var layer = _images.Where(o => o.Value == image)
                .Select(o => o.Key)
                .FirstOrDefault();

            if (layer == _images.First()
                .Key)
            {
                return invalidatedImageArea;
            }

            var belowLayer = _images.TakeWhile(o => o.Key < layer)
                .Last()
                .Key;
            var belowImageAccesor = _lastLayerImage._layerAccesors[belowLayer];

            invalidatedImageArea = invalidatedImageArea.CombineWith(belowImageAccesor.Area);

            var min = invalidatedImageArea.Min;
            var max = invalidatedImageArea.Max;
            for (var ix = Math.Max(min.X, 0); ix < Math.Min(max.X, editableMatrix.GetUpperBound(0)); ix++)
            {
                for (var iy = Math.Max(min.Y, 0); iy < Math.Min(max.Y, editableMatrix.GetUpperBound(1)); iy++)
                {
                    if (editableMatrix[ix, iy] < belowImageAccesor[new Vector2i(ix, iy)])
                    {
                        editableMatrix[ix, iy] = belowImageAccesor[new Vector2i(ix, iy)];
                    }
                }
            }

            return invalidatedImageArea;
        }

        public void Initialize(LayerId layer, List<IImageConstraint2i> constraints)
        {
            _images[layer] = new EditableMatrixImage2f(_imageConfig, constraints);
            _masks[layer] = new EditableMatrixMask2e(_imageConfig);
        }

        public IImage2f CreateImage(LayerId layer) => _images[layer]
            .CreateImage();

        public IMask2e CreateMask(LayerId layer) => _masks[layer]
            .CreateMask();

        public IEditableImageAccessor2f RequestAccess(LayerId layer, Range2i area)
        {
            var layerAccesors = new SortedDictionary<LayerId, IEditableImageAccessor2f>();
            foreach (var image in _images)
            {
                layerAccesors[image.Key] = image.Value.RequestAccess(area);
            }

            var layerMaskAccesors = new SortedDictionary<LayerId, IEditableMaskAccessor2e>();
            foreach (var mask in _masks)
            {
                layerMaskAccesors[mask.Key] = mask.Value.RequestAccess(area);
            }

            _lastLayerImage = new MatrixImageAccessor(layer, layerAccesors, layerMaskAccesors);
            return _lastLayerImage;
        }

        private class MatrixImageAccessor : IEditableImageAccessor2f
        {
            public readonly LayerId _layer;
            public readonly SortedDictionary<LayerId, IEditableImageAccessor2f> _layerAccesors;
            public SortedDictionary<LayerId, IEditableMaskAccessor2e> _layerMaskAccesors;

            public MatrixImageAccessor(LayerId layer, SortedDictionary<LayerId, IEditableImageAccessor2f> layerAccesors, SortedDictionary<LayerId, IEditableMaskAccessor2e> layerMaskAccesors)
            {
                _layer = layer;
                _layerAccesors = layerAccesors;
                _layerMaskAccesors = layerMaskAccesors;
            }

            public Range2i Area => _layerAccesors[_layer]
                .Area;

            public float this[Vector2i pos]
            {
                get => _layerAccesors[_layer][pos];
                set
                {
                    var existingValue = _layerAccesors[_layer][pos];
                    var dif = value - existingValue;

                    _layerAccesors[_layer][pos] = value;
                    foreach (var aboveLayer in _layerAccesors.SkipWhile(o => o.Key <= _layer))
                    {
                        aboveLayer.Value[pos] = aboveLayer.Value[pos] + dif;
                    }
                }
            }

            public void Dispose()
            {
                foreach (var layer in _layerAccesors)
                {
                    layer.Value.Dispose();
                }
            }
        }
    }
}
