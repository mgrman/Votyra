using UnityEngine;
using Votyra.Models;

namespace Votyra.Images
{
    public interface IImage3b
    {
        Rect3i InvalidatedArea { get; }

        bool Sample(Vector3i point);
    }
}