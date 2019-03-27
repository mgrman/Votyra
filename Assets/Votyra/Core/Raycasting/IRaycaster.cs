using Votyra.Core.Models;

namespace Votyra.Core.Raycasting
{
    public interface IRaycaster
    {
        Vector3f Raycast(Ray3f cameraRay);
    }
}