using UnityEngine;
using Votyra.Models;

namespace Votyra.Images
{
    public interface IImage3b
    {
        bool Sample(Vector3i point);
    }
}