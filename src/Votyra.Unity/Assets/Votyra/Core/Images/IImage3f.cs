using UnityEngine;
using Votyra.Models;

namespace Votyra.Core.Images
{
    public interface IImage3f
    {
        float Sample(Vector3i point);
    }
}
