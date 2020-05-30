using System.Collections.Generic;
using Votyra.Core.Images.Constraints;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class AggregatableImage2f : IImage2fProvider, IEditableImage2f
    {
        private readonly ILayerEditableImageProvider editableImageProvider;
        private readonly ILayerConfig layerConfig;

        public AggregatableImage2f(ILayerConfig layerConfig, ILayerEditableImageProvider editableImageProvider, List<IImageConstraint2i> constraints)
        {
            this.layerConfig = layerConfig;
            this.editableImageProvider = editableImageProvider;

            this.editableImageProvider.Initialize(this.layerConfig.Layer, constraints);
        }

        public IEditableImageAccessor2f RequestAccess(Range2i area) => this.editableImageProvider.RequestAccess(this.layerConfig.Layer, area);

        public IImage2f CreateImage() => this.editableImageProvider.CreateImage(this.layerConfig.Layer);
    }
}
