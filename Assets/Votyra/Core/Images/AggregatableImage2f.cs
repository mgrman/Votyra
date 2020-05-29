using System.Collections.Generic;
using Votyra.Core.Images.Constraints;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class AggregatableImage2f : IImage2fProvider, IEditableImage2f
    {
        private readonly ILayerEditableImageProvider _editableImageProvider;
        private readonly ILayerConfig _layerConfig;

        public AggregatableImage2f(ILayerConfig layerConfig, ILayerEditableImageProvider editableImageProvider, List<IImageConstraint2i> constraints)
        {
            this._layerConfig = layerConfig;
            this._editableImageProvider = editableImageProvider;

            this._editableImageProvider.Initialize(this._layerConfig.Layer, constraints);
        }

        public IEditableImageAccessor2f RequestAccess(Range2i area) => this._editableImageProvider.RequestAccess(this._layerConfig.Layer, area);

        public IImage2f CreateImage() => this._editableImageProvider.CreateImage(this._layerConfig.Layer);
    }
}
