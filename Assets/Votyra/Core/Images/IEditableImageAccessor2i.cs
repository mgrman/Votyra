using System;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface IEditableImageAccessor2i : IDisposable
    {
        Range2i Area { get; }

        Height1i this[Vector2i pos] { get; set; }
    }
}