using UnityEngine;
using Votyra.Models;

namespace Votyra.Images
{
    public interface IImage2i
    {
        Range2i RangeZ { get; }

        int Sample(Vector2i point);
    }
}