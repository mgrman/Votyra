using System.Collections.Generic;
using Votyra.Core.Images.Constraints;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface ILayerEditableImageProvider
    {
        void Initialize(LayerId layer, List<IImageConstraint2i> constraints);
        IImage2f CreateImage(LayerId layer);
        IMask2e CreateMask(LayerId layer);
        IEditableImageAccessor2f RequestAccess(LayerId layer,Range2i area);
    }
}