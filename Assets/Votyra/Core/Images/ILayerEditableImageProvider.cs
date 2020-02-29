using System.Collections.Generic;
using Votyra.Core.Images.Constraints;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface ILayerEditableImageProvider
    {
        void Initialize(int layer, List<IImageConstraint2i> constraints);
        IImage2f CreateImage(int layer);
        IEditableImageAccessor2f RequestAccess(int layer,Range2i area);
    }
}