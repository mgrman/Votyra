using System;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface IEditableMaskAccessor2e : IDisposable
    {
        Range2i Area { get; }

        MaskValues this[Vector2i pos] { get; set; }
    }
}
