using System.Collections.Generic;
using Votyra.Core.Images.Constraints;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class AggregatableImage2F : IImage2FProvider, IEditableImage2F
    {
        private readonly ILayerEditableImageProvider editableImageProvider;
        private readonly ILayerConfig layerConfig;

        public AggregatableImage2F(ILayerConfig layerConfig, ILayerEditableImageProvider editableImageProvider, List<IImageConstraint2I> constraints)
        {
            this.layerConfig = layerConfig;
            this.editableImageProvider = editableImageProvider;

            this.editableImageProvider.Initialize(this.layerConfig.Layer, constraints);
        }

        public IEditableImageAccessor2F RequestAccess(Range2i area) => this.editableImageProvider.RequestAccess(this.layerConfig.Layer, area);

        public IImage2F CreateImage() => this.editableImageProvider.CreateImage(this.layerConfig.Layer);
    }
}
