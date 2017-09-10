using UnityEngine;
using Votyra.Models;

namespace Votyra.Images
{
    public interface IImage3f
    {
        float Sample(Vector3i point);
    }
}