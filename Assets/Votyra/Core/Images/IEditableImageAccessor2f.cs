using System;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface IEditableImageAccessor2F : IDisposable
    {
        Range2i Area { get; }

        float this[Vector2i pos] { get; set; }
    }
}
