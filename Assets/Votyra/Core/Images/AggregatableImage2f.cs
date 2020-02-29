using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Images.Constraints;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class AggregatableImage2f : IImage2fProvider, IEditableImage2f
    {
        private readonly ILayerConfig _layerConfig;
        private readonly ILayerEditableImageProvider _editableImageProvider;

        public AggregatableImage2f(ILayerConfig layerConfig, ILayerEditableImageProvider editableImageProvider, List<IImageConstraint2i> constraints)
        {
            _layerConfig = layerConfig;
            _editableImageProvider = editableImageProvider;
            
            _editableImageProvider.Initialize(_layerConfig.Layer, constraints);
        }

        public IImage2f CreateImage() => _editableImageProvider.CreateImage(_layerConfig.Layer);

        public IEditableImageAccessor2f RequestAccess(Range2i area) => _editableImageProvider.RequestAccess(_layerConfig.Layer, area);
    }
}