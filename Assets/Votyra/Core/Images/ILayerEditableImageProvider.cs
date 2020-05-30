using System.Collections.Generic;
using Votyra.Core.Images.Constraints;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface ILayerEditableImageProvider
    {
        void Initialize(LayerId layer, List<IImageConstraint2I> constraints);

        IImage2F CreateImage(LayerId layer);

        IEditableImageAccessor2F RequestAccess(LayerId layer, Range2i area);
    }
}
