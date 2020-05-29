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
        private readonly Dictionary<LayerId, EditableMatrixImage2fCopy> _idToThickness = new Dictionary<LayerId, EditableMatrixImage2fCopy>();
        private readonly Dictionary<LayerId, EditableMatrixImage2f> _idToSum = new Dictionary<LayerId, EditableMatrixImage2f>();
        private readonly Dictionary<IEditableImage2f, LayerId> _thicknessToId = new Dictionary<IEditableImage2f, LayerId>();

        private readonly List<LayerId> _layerOrder = new List<LayerId>();

        public ImageAggregator(IImageConfig imageConfig)
        {
            _imageConfig = imageConfig;
        }

        void IImageConstraint2i.Initialize(IEditableImage2f image)
        {
        }

        IEnumerable<int> IImageConstraint2i.Priorities => new[]
        {
            int.MaxValue
        };

        Range2i IImageConstraint2i.FixImage(IEditableImage2f image, float[,] editableMatrix, Range2i invalidatedImageArea, Direction direction)
        {
            LayerId editedLayerId = this._thicknessToId[image];
            var index = this._layerOrder.IndexOf(editedLayerId);

            var sumAccesors = this._idToSum.ToDictionary(o => o.Key, o => o.Value.RequestAccess(invalidatedImageArea));
            var layerAccessors = this._idToThickness.ToDictionary(o => o.Key, o => o.Value.RequestAccess(invalidatedImageArea));

            var min = invalidatedImageArea.Min;
            var max = invalidatedImageArea.Max;

            if (index == 0)
            {
                var sumLayer = sumAccesors[editedLayerId];
                for (var ix = Math.Max(min.X, 0); ix < Math.Min(max.X, editableMatrix.GetUpperBound(0)); ix++)
                {
                    for (var iy = Math.Max(min.Y, 0); iy < Math.Min(max.Y, editableMatrix.GetUpperBound(1)); iy++)
                    {
                        var i = new Vector2i(ix, iy);
                        var value= editableMatrix[ix, iy];
                        var diff = value - sumLayer[i];
                        sumLayer[i] = editableMatrix[ix, iy];

                        for (int aboveLayerIndex = index + 1; aboveLayerIndex < this._layerOrder.Count; aboveLayerIndex++)
                        {
                            var valueAbove = sumAccesors[this._layerOrder[aboveLayerIndex]][i];
                            if (!float.IsNaN(valueAbove))
                            {
                                sumAccesors[this._layerOrder[aboveLayerIndex]][i] = valueAbove + diff;
                            }
                        }
                    }
                }
            }
            else
            {
                var sumLayer = sumAccesors[editedLayerId];
                var sumLayerBelow = sumAccesors[this._layerOrder[index - 1]];
                
                // clip thickness to non-negative
                for (var ix = Math.Max(min.X, 0); ix < Math.Min(max.X, editableMatrix.GetUpperBound(0)); ix++)
                {
                    for (var iy = Math.Max(min.Y, 0); iy < Math.Min(max.Y, editableMatrix.GetUpperBound(1)); iy++)
                    {
                        var i = new Vector2i(ix, iy);

                        if (editableMatrix[ix, iy] < 0) 
                        {
                            editableMatrix[ix, iy] = 0;
                        }
                    }
                }

                for (var ix = Math.Max(min.X, 1); ix < Math.Min(max.X, editableMatrix.GetUpperBound(0)-1); ix++)
                {
                    for (var iy = Math.Max(min.Y, 1); iy < Math.Min(max.Y, editableMatrix.GetUpperBound(1)-1); iy++)
                    {
                        var i = new Vector2i(ix, iy);

                        var value = editableMatrix[ix, iy];

                        var sum = sumLayerBelow[i] + value;
                        var diff = sum - sumLayer[i];

                        if (value == 0)
                        {
                            var x0 = editableMatrix[ix - 1, iy];
                            var x1 = editableMatrix[ix + 1, iy];
                            var y0 = editableMatrix[ix, iy - 1];
                            var y1 = editableMatrix[ix, iy + 1];
                            var x0y1 = editableMatrix[ix- 1, iy + 1]; // TODO: edge case of edge fliping logic
                            var x1y0 = editableMatrix[ix + 1, iy - 1]; // TODO: edge case of edge fliping logic
                            if (x0 == 0 && x1 == 0 && y0 == 0 && y1 == 0 && x0y1 == 0 && x1y0 == 0)
                            {
                                sumLayer[i] = float.NaN;
                            }
                            else
                            {
                                sumLayer[i] = sum; // update current layer
                            }
                        }
                        else
                        {
                            sumLayer[i] = sum; // update current layer
                        }

                        for (int aboveLayerIndex = index + 1; aboveLayerIndex < this._layerOrder.Count; aboveLayerIndex++)
                        {
                            var valueAbove = sumAccesors[this._layerOrder[aboveLayerIndex]][i];
                            if (!float.IsNaN(valueAbove))
                            {
                                sumAccesors[this._layerOrder[aboveLayerIndex]][i] = valueAbove + diff;
                            }
                        }
                    }
                }
            }

            foreach (var accessor in sumAccesors)
            {
                accessor.Value.Dispose();
            }

            foreach (var accessor in layerAccessors)
            {
                accessor.Value.Dispose();
            }

            return invalidatedImageArea;
        }

        public void Initialize(LayerId layer, List<IImageConstraint2i> constraints)
        {

            var thicknessImage = new EditableMatrixImage2fCopy(_imageConfig, constraints);
            this._thicknessToId[thicknessImage] = layer;
            this._idToThickness[layer] = thicknessImage;
            
            var sumImage = new EditableMatrixImage2f(_imageConfig, new List<IImageConstraint2i>(), _layerOrder.Count ==0?0f:float.NaN);
            this._idToSum[layer] = sumImage;
            
            
            this._layerOrder.Add(layer);
        }

        public IImage2f CreateImage(LayerId layer) => this._idToSum[layer]
            .CreateImage();

        public IEditableImageAccessor2f RequestAccess(LayerId layer, Range2i area)
        {
            var layerAccesors = new SortedDictionary<LayerId, IEditableImageAccessor2f>();
            foreach (var image in this._idToThickness)
            {
                layerAccesors[image.Key] = image.Value.RequestAccess(area);
            }

            return new MatrixImageAccessor(layerAccesors[layer]);
        }

        private class MatrixImageAccessor : IEditableImageAccessor2f
        {
            public readonly IEditableImageAccessor2f _layerAccesors;

            public MatrixImageAccessor(IEditableImageAccessor2f layerAccesors)
            {
                _layerAccesors = layerAccesors;
            }

            public Range2i Area => _layerAccesors.Area;

            public float this[Vector2i pos]
            {
                get => _layerAccesors[pos];
                set
                {
                    var existingValue = _layerAccesors[pos];
                    var dif = value - existingValue;

                    _layerAccesors[pos] = value;
                }
            }

            public void Dispose()
            {
                _layerAccesors.Dispose();
            }
        }

        public IEnumerable<int> Priorities { get; }
    }
}
