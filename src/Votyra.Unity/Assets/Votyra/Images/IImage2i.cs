using UnityEngine;
using Votyra.Common.Models;

namespace Votyra.Images
{
    public interface IImage2i
    {
        Rect InvalidatedArea { get; }

        Range2i RangeZ { get; }

        int Sample(Vector2i point);
    }
}