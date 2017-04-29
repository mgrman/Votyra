using Votyra.Common.Models;
using UnityEngine;

namespace Votyra.Images
{
    public interface IImage2
    {
        bool IsAnimated { get; }

        Range2 RangeZ { get; }

        float Sample(Vector2 point, float time);
    }
}