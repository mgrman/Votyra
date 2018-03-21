using System;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface IEditableImage3f
    {
        IEditableImageAccessor3f RequestAccess(Rect3i area);

    }
}