using System.Collections.Generic;
using Votyra.Core.Images.Constraints;

namespace Votyra.Core.Images
{
    public class AggregatableMask2e : IMask2eProvider
    {
        private readonly ILayerEditableImageProvider _editableImageProvider;
        private readonly ILayerConfig _layerConfig;

        public AggregatableMask2e(ILayerConfig layerConfig, ILayerEditableImageProvider editableImageProvider, List<IImageConstraint2i> constraints)
        {
            _layerConfig = layerConfig;
            _editableImageProvider = editableImageProvider;
        }

        public IMask2e CreateMask() => _editableImageProvider.CreateMask(_layerConfig.Layer);
    }
}
