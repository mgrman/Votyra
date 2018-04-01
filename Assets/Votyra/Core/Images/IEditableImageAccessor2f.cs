using System;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface IEditableImageAccessor2f : IDisposable
    {
        Range2i Area { get; }

        float this[Vector2i pos] { get; set; }
    }
}