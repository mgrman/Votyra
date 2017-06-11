using Votyra.Models;
using UnityEngine;

namespace Votyra.Images
{
    public interface IImage2
    {
        Rect InvalidatedArea { get; }

        Range2 RangeZ { get; }

        float Sample(Vector2 point);
    }
}