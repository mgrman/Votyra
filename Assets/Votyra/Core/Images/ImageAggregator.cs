using System;
using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Images.Constraints;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class ImageAggregator : ILayerEditableImageProvider, IImageConstraint2I
    {
        private readonly Dictionary<LayerId, EditableMatrixImage2F> idToSum = new Dictionary<LayerId, EditableMatrixImage2F>();
        private readonly Dictionary<LayerId, EditableMatrixImage2FCopy> idToThickness = new Dictionary<LayerId, EditableMatrixImage2FCopy>();
        private readonly IImageConfig imageConfig;

        private readonly List<LayerId> layerOrder = new List<LayerId>();
        private readonly Dictionary<IEditableImage2F, LayerId> thicknessToId = new Dictionary<IEditableImage2F, LayerId>();

        public ImageAggregator(IImageConfig imageConfig)
        {
            this.imageConfig = imageConfig;
        }

        public IEnumerable<int> Priorities { get; }

        void IImageConstraint2I.Initialize(IEditableImage2F image)
        {
        }

        IEnumerable<int> IImageConstraint2I.Priorities => new[]
        {
            int.MaxValue,
        };

        Range2i IImageConstraint2I.FixImage(IEditableImage2F image, float[,] editableMatrix, Range2i invalidatedImageArea, Direction direction)
        {
            var editedLayerId = this.thicknessToId[image];
            var index = this.layerOrder.IndexOf(editedLayerId);

            var sumAccesors = this.idToSum.ToDictionary(o => o.Key, o => o.Value.RequestAccess(invalidatedImageArea));
            var layerAccessors = this.idToThickness.ToDictionary(o => o.Key, o => o.Value.RequestAccess(invalidatedImageArea));

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
                        var value = editableMatrix[ix, iy];
                        var diff = value - sumLayer[i];
                        sumLayer[i] = editableMatrix[ix, iy];

                        for (var aboveLayerIndex = index + 1; aboveLayerIndex < this.layerOrder.Count; aboveLayerIndex++)
                        {
                            var valueAbove = sumAccesors[this.layerOrder[aboveLayerIndex]][i];
                            if (!float.IsNaN(valueAbove))
                            {
                                sumAccesors[this.layerOrder[aboveLayerIndex]][i] = valueAbove + diff;
                            }
                        }
                    }
                }
            }
            else
            {
                var sumLayer = sumAccesors[editedLayerId];
                var sumLayerBelow = sumAccesors[this.layerOrder[index - 1]];

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

                for (var ix = Math.Max(min.X, 1); ix < Math.Min(max.X, editableMatrix.GetUpperBound(0) - 1); ix++)
                {
                    for (var iy = Math.Max(min.Y, 1); iy < Math.Min(max.Y, editableMatrix.GetUpperBound(1) - 1); iy++)
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
                            var x0Y1 = editableMatrix[ix - 1, iy + 1]; // TODO: edge case of edge fliping logic
                            var x1Y0 = editableMatrix[ix + 1, iy - 1]; // TODO: edge case of edge fliping logic
                            if ((x0 == 0) && (x1 == 0) && (y0 == 0) && (y1 == 0) && (x0Y1 == 0) && (x1Y0 == 0))
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

                        for (var aboveLayerIndex = index + 1; aboveLayerIndex < this.layerOrder.Count; aboveLayerIndex++)
                        {
                            var valueAbove = sumAccesors[this.layerOrder[aboveLayerIndex]][i];
                            if (!float.IsNaN(valueAbove))
                            {
                                sumAccesors[this.layerOrder[aboveLayerIndex]][i] = valueAbove + diff;
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

        public void Initialize(LayerId layer, List<IImageConstraint2I> constraints)
        {
            var thicknessImage = new EditableMatrixImage2FCopy(this.imageConfig, constraints);
            this.thicknessToId[thicknessImage] = layer;
            this.idToThickness[layer] = thicknessImage;

            var sumImage = new EditableMatrixImage2F(this.imageConfig, new List<IImageConstraint2I>(), this.layerOrder.Count == 0 ? 0f : float.NaN);
            this.idToSum[layer] = sumImage;

            this.layerOrder.Add(layer);
        }

        public IImage2F CreateImage(LayerId layer) => this.idToSum[layer]
            .CreateImage();

        public IEditableImageAccessor2F RequestAccess(LayerId layer, Range2i area)
        {
            var layerAccesors = new SortedDictionary<LayerId, IEditableImageAccessor2F>();
            foreach (var image in this.idToThickness)
            {
                layerAccesors[image.Key] = image.Value.RequestAccess(area);
            }

            return new MatrixImageAccessor(layerAccesors[layer]);
        }

        private class MatrixImageAccessor : IEditableImageAccessor2F
        {
            public readonly IEditableImageAccessor2F LayerAccesors;

            public MatrixImageAccessor(IEditableImageAccessor2F layerAccesors)
            {
                this.LayerAccesors = layerAccesors;
            }

            public Range2i Area => this.LayerAccesors.Area;

            public float this[Vector2i pos]
            {
                get => this.LayerAccesors[pos];
                set
                {
                    var existingValue = this.LayerAccesors[pos];
                    var dif = value - existingValue;

                    this.LayerAccesors[pos] = value;
                }
            }

            public void Dispose()
            {
                this.LayerAccesors.Dispose();
            }
        }
    }
}
