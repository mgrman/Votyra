using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Images.Constraints;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class AggregatableMask2e : IMask2eProvider
    {
        private readonly ILayerConfig _layerConfig;
        private readonly ILayerEditableImageProvider _editableImageProvider;

        public AggregatableMask2e(ILayerConfig layerConfig, ILayerEditableImageProvider editableImageProvider, List<IImageConstraint2i> constraints)
        {
            _layerConfig = layerConfig;
            _editableImageProvider = editableImageProvider;
        }
        
        public IMask2e CreateMask() => _editableImageProvider.CreateMask(_layerConfig.Layer);
    }
}