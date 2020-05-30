using System;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface IEditableImageAccessor3B : IDisposable
    {
        Range3i Area { get; }

        bool this[Vector3i pos] { get; set; }
    }
}
